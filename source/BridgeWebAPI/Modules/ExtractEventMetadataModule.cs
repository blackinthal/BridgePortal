using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Bridge.Domain;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Factories;
using Bridge.WebAPI.Providers;
using Dds.Net;
using ElmahExtensions;
using WebGrease.Css.Extensions;
using SysEventType = Bridge.Domain.StaticModels.SysEventType;
using SysPlayer = Bridge.Domain.StaticModels.SysPlayer;

namespace Bridge.WebAPI.Modules
{
    public class ExtractEventMetadataModule
    {
        private readonly IEventProvider _provider;
        private readonly UrlProviderFactory _urlProviderFactory;
        private readonly ContractScoreCalculatorModule _scoreCalculator;
        private readonly ComputeOptimalScoreModule _optimalContract;

        public List<string> Errors;
        public ExtractEventMetadataModule(IEventProvider provider, UrlProviderFactory urlProviderFactory, ContractScoreCalculatorModule scoreCalculator, ComputeOptimalScoreModule optimalContract)
        {
            _provider = provider;
            _urlProviderFactory = urlProviderFactory;
            _scoreCalculator = scoreCalculator;
            _optimalContract = optimalContract;

            Errors = new List<string>();
        }

        public ImportEvent ExtractEventMetadata(DateTime selectedDate)
        {
            var urlProvider = _urlProviderFactory.GetUrlProvider(selectedDate);
            var tempFilePath = _provider.ReadEventPBNData(urlProvider.GetUrl(selectedDate));

            var command = ProcessPbnFile(tempFilePath);

            command.Date = selectedDate;
            command.Name = string.Format("Locomotiva {0}", selectedDate.ToShortDateString());
            command.ProcessId = Guid.NewGuid();
            command.SysEventTypeId = selectedDate.DayOfWeek == DayOfWeek.Tuesday
                ? (int)SysEventType.Percentages
                : (int)SysEventType.IMP;

            _provider.CleanUp(tempFilePath);

            return command;
        }

        private ImportEvent ProcessPbnFile(string filePath)
        {
            var result = new ImportEvent();
            var currentDeal = new DealMetadata();
            var currentState = ParseState.ReadingDeal;

            var identifiedPairs = 0;
            var identifiedDealResults = 0;
            var totalScoreTableHeaders = new Dictionary<string, int>();
            var scoreTableHeaders = new Dictionary<string, int>();

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("% <META  name=PairCount")) { result.NoOfPairs = Int32.Parse(ExtractValue(line)); continue;}
                    if (line.StartsWith("% <META  name=BoardCount")) {result.NoOfBoards = Int32.Parse(ExtractValue(line)); continue;}
                    if (line.StartsWith("% <META  name=RoundCount")) { result.NoOfRounds = Int32.Parse(ExtractValue(line)); continue; }
                    if (line.StartsWith("[Board"))
                    {
                        var deal = new DealMetadata {Index = Int32.Parse(ExtractValue(line))};
                        result.Deals.Add(deal);
                        currentDeal = deal;
                        currentState = ParseState.ReadingDeal;
                        identifiedDealResults = 0;
                        continue;
                    }
                    if (line.StartsWith("[Deal "))
                    {
                        currentDeal.PBNRepresentation = ExtractValue(line); 
                        currentDeal.HandViewerInput = ConvertPBNToHandViewerInput(currentDeal.PBNRepresentation, currentDeal.Index, (SysVulnerabilityEnum)currentDeal.SysVulnerabilityId);
                        CalculateMakeableContracts(currentDeal);
                        CalculateOptimalContract(currentDeal);
                        continue;
                    }
                    if (line.StartsWith("[Vulnerable"))
                    {
                        currentDeal.SysVulnerabilityId = (int)Enum.Parse(typeof(SysVulnerabilityEnum),ExtractValue(line), true); 
                        continue;
                    }
                    if (currentState == ParseState.ReadingTotalScoreTable && identifiedPairs < result.NoOfPairs)
                    {
                        result.Pairs.Add(ExtractPairMetadata(line, totalScoreTableHeaders));
                        identifiedPairs++;
                        continue;
                    }
                    if (currentState == ParseState.ReadingDealScoreTable && identifiedDealResults < result.NoOfRounds)
                    {
                        var duplicateDeal = ExtractDuplicateDealMetadata(line, currentDeal, scoreTableHeaders);
                        if (duplicateDeal != null)
                        {
                            currentDeal.DealResults.Add(duplicateDeal);
                        }
                            
                        identifiedDealResults++;
                        continue;
                    }
                    if (line.StartsWith("[TotalScoreTable"))
                    {
                        totalScoreTableHeaders = ParseTableHeader(ExtractValue(line));
                        currentState = ParseState.ReadingTotalScoreTable; continue;
                    }
                    if (line.StartsWith("[ScoreTable"))
                    {
                        scoreTableHeaders = ParseTableHeader(ExtractValue(line));
                        currentState = ParseState.ReadingDealScoreTable;
                    }
                }
            }
            return result;
        }

        private void CalculateOptimalContract(DealMetadata currentDeal)
        {
            var contract = _optimalContract.ComputeOptimalContract(currentDeal.PBNRepresentation, (SysVulnerabilityEnum) currentDeal.SysVulnerabilityId);

            currentDeal.BestContract = contract.Item2.Notation();
            currentDeal.BestContractDisplay = contract.Item2.Display();
            currentDeal.BestContractResult = contract.Item1;
            currentDeal.BestContractDeclarer = contract.Item2.PlayerPosition.ConvertToSysPlayer();
        }

        private static void CalculateMakeableContracts(DealMetadata deal)
        {
            var contracts = DoubleDummyModule.CalculateMakeableContracts(deal.PBNRepresentation);

            contracts.ForEach(contract => deal.MakeableContracts.Add(new MakeableContractMetadata
            {
                Contract = contract.Display(),
                Declarer = contract.PlayerPosition.ConvertToSysPlayer(),
                Denomination = contract.Trump.Order,
                Level = contract.Value,
                HandViewerInput = deal.HandViewerInput + ConvertToPbnBiddingSequence(contract, contract.PlayerPosition.ConvertToSysPlayer())
            }));
        }

        /// <summary>
        /// We use LIN format ex: http://www.bridgebase.com/tools/handviewer.html?lin=md|1S2389JHTD3JC237KA,S7TH4QKD678TC4569,S456KAH25D25KACJQ,|rh||ah|Board%207|sv|e|
        /// </summary>
        /// <param name="pbnRepresentation"></param>
        /// <param name="boardNo"></param>
        /// <param name="vulnerability"></param>
        /// <returns></returns>
        public string ConvertPBNToHandViewerInput(string pbnRepresentation, int boardNo, SysVulnerabilityEnum vulnerability)
        {
            var split = pbnRepresentation.Split(':');

            var dealer = split[0].ToLower();
            var hands = split[1].Split(' ');
            var order = new List<string> { "s", "w", "n", "e" };
            var suitOrder = new List<string> {"S", "H", "D", "C"};

            var indexOfDeclarer = order.IndexOf(dealer);
            var parsedHands = new string[4];

            var sb = new StringBuilder("http://www.bridgebase.com/tools/handviewer.html?lin=md|");

            sb.Append(string.Format("{0}",(indexOfDeclarer + 1)));
          
            for (var i = indexOfDeclarer; i < indexOfDeclarer + 4; i++)
            {
                var currentHand = hands[i - indexOfDeclarer];
                var suits = currentHand.Split('.');

                var hand = new StringBuilder();

                for (var j = 0; j < 4; j ++)
                {
                    hand.Append(suitOrder[j]).Append(suits[j]);
                }

                parsedHands[i % 4] = hand.ToString();
            }

            for (var i = 0; i < 4; i++)
            {
                sb.Append(string.Format("{0},", parsedHands[i]));
            }

            sb.Append(string.Format("|rh||ah|Board {0}|sv|", boardNo));
            //It takes values of n, e, b, and - 
            switch (vulnerability)
            {
                case SysVulnerabilityEnum.All:
                    sb.Append("b|"); break;
                case SysVulnerabilityEnum.EW:
                    sb.Append("e|"); break;
                case SysVulnerabilityEnum.NS:
                    sb.Append("n|"); break;
                case SysVulnerabilityEnum.None:
                    sb.Append("-|"); break;
            }

            return sb.ToString();
        }

        public static string ExtractValue(string line)
        {
            var start = line.IndexOf('"');
            var end = line.LastIndexOf('"');

            return line.Substring(start + 1, end - start - 1);
        }

        public static PairMetadata ExtractPairMetadata(string line, Dictionary<string,int> columnHeaders)
        {
            var pair = new PairMetadata();
            var values = ParseTableLine(line, columnHeaders);

            pair.Name = values["Names"];
            pair.Rank = Int32.Parse(values["Rank"]);
            pair.Score = decimal.Parse(values.ContainsKey("TotalPercentage") ? values["TotalPercentage"] : values["TotalScoreIMP"]);
            pair.PairId = Int32.Parse(values["PairId"]);

            var playerNames = pair.Name.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);

            pair.Player1Name = playerNames[0];
            pair.Player2Name = playerNames[1];
 
            return pair;
        }

        public DuplicateDealMetadata ExtractDuplicateDealMetadata(string line, DealMetadata deal, Dictionary<string,int> columnHeaders)
        {
            try
            {
                var duplicateDeal = new DuplicateDealMetadata();
                var values = ParseTableLine(line, columnHeaders);

                duplicateDeal.NSPairIndex = Int32.Parse(values["PairId_NS"]);
                duplicateDeal.EWPairIndex = Int32.Parse(values["PairId_EW"]);
                duplicateDeal.Contract = values["Contract"];

                SysPlayer declarer;
                duplicateDeal.Declarer = Enum.TryParse(values["Declarer"], true, out declarer) ? (int)declarer : (int)SysPlayer.N;

                if (duplicateDeal.Contract == "-")
                    return null;

                var position = PlayerPosition.North;
                switch (duplicateDeal.Declarer)
                {
                    case (int) SysPlayer.S:
                        position = PlayerPosition.South;
                        break;
                    case (int) SysPlayer.E:
                        position = PlayerPosition.East;
                        break;
                    case (int) SysPlayer.W:
                        position = PlayerPosition.West;
                        break;
                }

                var contract = new Contract(duplicateDeal.Contract, position);

                int tricks;
                duplicateDeal.Result = Int32.TryParse(values["Result"], out tricks)
                    ? _scoreCalculator.CalculateScore(contract, tricks, (SysVulnerabilityEnum)deal.SysVulnerabilityId)
                    : _scoreCalculator.CalculateScore(contract, values["Result"], (SysVulnerabilityEnum)deal.SysVulnerabilityId);

                duplicateDeal.NSPercentage = Int32.Parse(values.ContainsKey("Percentage_NS") ? values["Percentage_NS"] : values["IMP_NS"]);
                duplicateDeal.EWPercentage = Int32.Parse(values.ContainsKey("Percentage_EW") ? values["Percentage_EW"] : values["IMP_EW"]);
                duplicateDeal.ContractDisplay = contract.Display();
                duplicateDeal.HandViewerInput = deal.HandViewerInput + ConvertToPbnBiddingSequence(contract, duplicateDeal.Declarer);

                return duplicateDeal;
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
                return null;
            }
        }

        public enum ParseState
        {
            ReadingDeal,
            ReadingTotalScoreTable,
            ReadingDealScoreTable
        }

        public static Dictionary<string, int> ParseTableHeader(string header)
        {
            var parseResult = new Dictionary<string, int>();
            var columns = header.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            columns.ForEach(column =>
            {
                var regex = new Regex("[0-9]+");
                var columnMeta = column.Split(new[] {'\\'});
                var columnTitle = columnMeta[0];
                var valueLength = Int32.Parse(regex.Match(columnMeta[1]).Value);

                parseResult.Add(columnTitle, valueLength);
            });

            return parseResult;
        }

        public static Dictionary<string, string> ParseTableLine(string line, Dictionary<string,int> columnConfig)
        {
            var parseResult = new Dictionary<string, string>();
            var position = 0;
            columnConfig.ForEach(pair =>
            {
                var value = line.Substring(position, pair.Value).Trim().Replace("\"",string.Empty);
                position += pair.Value + 1;
                parseResult.Add(pair.Key, value);
            });

            return parseResult;
        }

        private static string ConvertToPbnBiddingSequence(Contract c, int dealerId)
        {
            var declaredId = c.PlayerPosition.ConvertToSysPlayer();

            if (dealerId <= 0 || declaredId <= 0)
                return string.Empty;

            var sb = new StringBuilder("|b");

            for (var i = dealerId; i < dealerId + 4; i++)
            {
                sb.Append("|mb");
                var currentPlayer = (i%4);
                if (currentPlayer == 0)
                    currentPlayer = 4;
                if (currentPlayer == declaredId)
                {
                    sb.Append(string.Format("|{0}", c.Notation()));
                    break;
                }
                sb.Append("|p");
            }

            if (c.Doubled)
                sb.Append("|mb|x");
            if (c.Redoubled)
                sb.Append("|mb|xx");

            sb.Append("|mb|p|mb|p|mb|p|");
            return sb.ToString();
        }
    }
}