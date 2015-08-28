using System;
using System.Collections.Generic;
using System.IO;
using Bridge.Domain;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Factories;
using Bridge.WebAPI.Modules.Helpers;
using Bridge.WebAPI.Modules.StaticModels;
using Bridge.WebAPI.Providers;
using Dds.Net;
using ElmahExtensions;
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
                    if (line.StartsWith("% <META  name=PairCount")) { result.NoOfPairs = Int32.Parse(ParsePBNHelpers.ExtractValue(line)); continue;}
                    if (line.StartsWith("% <META  name=BoardCount")) {result.NoOfBoards = Int32.Parse(ParsePBNHelpers.ExtractValue(line)); continue;}
                    if (line.StartsWith("% <META  name=RoundCount")) { result.NoOfRounds = Int32.Parse(ParsePBNHelpers.ExtractValue(line)); continue; }
                    if (line.StartsWith("[Board"))
                    {
                        var deal = new DealMetadata {Index = Int32.Parse(ParsePBNHelpers.ExtractValue(line))};
                        result.Deals.Add(deal);
                        currentDeal = deal;
                        currentState = ParseState.ReadingDeal;
                        identifiedDealResults = 0;
                        continue;
                    }
                    if (line.StartsWith("[Dealer "))
                    {
                        var dealer = ParsePBNHelpers.ExtractValue(line);
                        currentDeal.Dealer = ParsePBNHelpers.GetPlayerIdFromString(dealer);
                        continue;
                    }
                    if (line.StartsWith("[Deal "))
                    {
                        currentDeal.PBNRepresentation = ParsePBNHelpers.ExtractValue(line); 
                        currentDeal.HandViewerInput = ParsePBNHelpers.ConvertPBNToHandViewerInput(currentDeal.PBNRepresentation, currentDeal.Index, (SysVulnerabilityEnum)currentDeal.SysVulnerabilityId);
                        CalculateMakeableContracts(currentDeal);
                        CalculateOptimalContract(currentDeal);
                        continue;
                    }
                    if (line.StartsWith("[Vulnerable"))
                    {
                        currentDeal.SysVulnerabilityId = (int)Enum.Parse(typeof(SysVulnerabilityEnum),ParsePBNHelpers.ExtractValue(line), true); 
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
                        totalScoreTableHeaders = ParsePBNHelpers.ParseTableHeader(ParsePBNHelpers.ExtractValue(line));
                        currentState = ParseState.ReadingTotalScoreTable; continue;
                    }
                    if (line.StartsWith("[ScoreTable"))
                    {
                        scoreTableHeaders = ParsePBNHelpers.ParseTableHeader(ParsePBNHelpers.ExtractValue(line));
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
            currentDeal.BestContractHandViewerInput = currentDeal.HandViewerInput + ParsePBNHelpers.ConvertToPbnBiddingSequence(contract.Item2, currentDeal.Dealer);
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
                HandViewerInput = deal.HandViewerInput + ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, deal.Dealer)
            }));
        }

        public static PairMetadata ExtractPairMetadata(string line, Dictionary<string,int> columnHeaders)
        {
            var pair = new PairMetadata();
            var values = ParsePBNHelpers.ParseTableLine(line, columnHeaders);

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
                var values = ParsePBNHelpers.ParseTableLine(line, columnHeaders);

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
                duplicateDeal.HandViewerInput = deal.HandViewerInput + ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, deal.Dealer);
                duplicateDeal.Tricks = values["Result"];

                return duplicateDeal;
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
                return null;
            }
        }

    }
}