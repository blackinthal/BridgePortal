using System;
using System.Collections.Generic;
using System.IO;
using Bridge.Domain;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Providers;
using ElmahExtensions;

namespace Bridge.WebAPI.Modules
{
    public class ExtractEventMetadataModule
    {
        private readonly IEventProvider _provider;
        private readonly IUrlProvider _urlProvider;
        private readonly ContractScoreCalculatorModule _scoreCalculator;

        public List<string> Errors; 
        public ExtractEventMetadataModule(IEventProvider provider, IUrlProvider urlProvider, ContractScoreCalculatorModule scoreCalculator)
        {
            _provider = provider;
            _urlProvider = urlProvider;
            _scoreCalculator = scoreCalculator;

            Errors = new List<string>();
        }

        public ImportEvent ExtractEventMetadata(DateTime selectedDate)
        {
            var tempFilePath = _provider.ReadEventPBNData(_urlProvider.GetUrl(selectedDate));

            var command = ProcessPbnFile(tempFilePath);

            command.Date = selectedDate;
            command.Name = string.Format("Locomotiva {0}", selectedDate);
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
                        result.Pairs.Add(ExtractPairMetadata(line));
                        identifiedPairs++;
                        continue;
                    }
                    if (currentState == ParseState.ReadingDealScoreTable && identifiedDealResults < result.NoOfRounds)
                    {
                        var duplicateDeal = ExtractDuplicateDealMetadata(line, (SysVulnerabilityEnum) currentDeal.SysVulnerabilityId);
                        if(duplicateDeal != null)
                            currentDeal.DealResults.Add(duplicateDeal);
                        identifiedDealResults++;
                        continue;
                    }
                    if (line.StartsWith("[TotalScoreTable")){ currentState = ParseState.ReadingTotalScoreTable; continue;}
                    if (line.StartsWith("[ScoreTable")) { currentState = ParseState.ReadingDealScoreTable;}
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

        public static PairMetadata ExtractPairMetadata(string line)
        {
            var pair = new PairMetadata();
            var values = line.Split(new[]{' '},7,StringSplitOptions.RemoveEmptyEntries);

            pair.Name = ExtractValue(values[6]);
            pair.Rank = Int32.Parse(values[0]);
            pair.Result = decimal.Parse(values[5]);
            pair.PairId = Int32.Parse(values[1]);

            var playerNames = pair.Name.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);

            pair.Player1Name = playerNames[0];
            pair.Player2Name = playerNames[1];
 
            return pair;
        }

        public DuplicateDealMetadata ExtractDuplicateDealMetadata(string line, SysVulnerabilityEnum dealVulnerability)
        {
            try
            {
                var duplicateDeal = new DuplicateDealMetadata();
                var values = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                duplicateDeal.NSPairIndex = Int32.Parse(values[2]);
                duplicateDeal.EWPairIndex = Int32.Parse(values[3]);
                duplicateDeal.Contract = values[4];

                SysPlayer declarer;
                duplicateDeal.Declarer = Enum.TryParse(values[5], true, out declarer) ? (int)declarer : (int)SysPlayer.N;

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
                duplicateDeal.Result = Int32.TryParse(values[6], out tricks)
                    ? _scoreCalculator.CalculateScore(contract, tricks, dealVulnerability)
                    : _scoreCalculator.CalculateScore(contract, values[6], dealVulnerability);

                duplicateDeal.NSPercentage = Int32.Parse(values[12]);
                duplicateDeal.EWPercentage = Int32.Parse(values[13]);

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
    }
}