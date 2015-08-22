using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Bridge.Domain;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Factories;
using Bridge.WebAPI.Providers;
using ElmahExtensions;
using WebGrease.Css.Extensions;

namespace Bridge.WebAPI.Modules
{
    public class ExtractEventMetadataModule
    {
        private readonly IEventProvider _provider;
        private readonly UrlProviderFactory _urlProviderFactory;
        private readonly ContractScoreCalculatorModule _scoreCalculator;

        public List<string> Errors;
        public ExtractEventMetadataModule(IEventProvider provider, UrlProviderFactory urlProviderFactory, ContractScoreCalculatorModule scoreCalculator)
        {
            _provider = provider;
            _urlProviderFactory = urlProviderFactory;
            _scoreCalculator = scoreCalculator;

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
                    if (line.StartsWith("[Deal")){ currentDeal.PBNRepresentation = ExtractValue(line); continue; }
                    if (line.StartsWith("[Vulnerable")){ currentDeal.SysVulnerabilityId = (int)Enum.Parse(typeof(SysVulnerabilityEnum),ExtractValue(line), true); continue;}
                    if (currentState == ParseState.ReadingTotalScoreTable && identifiedPairs < result.NoOfPairs)
                    {
                        result.Pairs.Add(ExtractPairMetadata(line, totalScoreTableHeaders));
                        identifiedPairs++;
                        continue;
                    }
                    if (currentState == ParseState.ReadingDealScoreTable && identifiedDealResults < result.NoOfRounds)
                    {
                        var duplicateDeal = ExtractDuplicateDealMetadata(line, (SysVulnerabilityEnum) currentDeal.SysVulnerabilityId, scoreTableHeaders);
                        if(duplicateDeal != null)
                            currentDeal.DealResults.Add(duplicateDeal);
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

        public DuplicateDealMetadata ExtractDuplicateDealMetadata(string line, SysVulnerabilityEnum dealVulnerability, Dictionary<string,int> columnHeaders)
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
                    case (int) SysPlayer.E:
                    case (int) SysPlayer.W:
                        position = PlayerPosition.West;
                        break;
                }

                var contract = new Contract(duplicateDeal.Contract, position);

                int tricks;
                duplicateDeal.Result = Int32.TryParse(values["Result"], out tricks)
                    ? _scoreCalculator.CalculateScore(contract, tricks, dealVulnerability)
                    : _scoreCalculator.CalculateScore(contract, values["Result"], dealVulnerability);

                duplicateDeal.NSPercentage = Int32.Parse(values.ContainsKey("Percentage_NS") ? values["Percentage_NS"] : values["IMP_NS"]);
                duplicateDeal.EWPercentage = Int32.Parse(values.ContainsKey("Percentage_EW") ? values["Percentage_EW"] : values["IMP_EW"]);

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
    }
}