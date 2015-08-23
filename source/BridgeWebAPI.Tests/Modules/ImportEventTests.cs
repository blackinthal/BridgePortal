using System;
using System.IO;
using System.Linq;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Factories;
using Bridge.WebAPI.Modules;
using Bridge.WebAPI.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Modules
{
    [TestClass]
    public class ImportEventTests
    {
        const string Url = "http://www.locomotiva.ro/cls/15/04/28-04-15.pbn";
        [TestMethod]
        public void TestTemporaryFileIsCreated()
        {
            //Arrange
            var provider = new LocomotivaEventProvider();
            //Act
            var tempFilePath = provider.ReadEventPBNData(Url);
            //Assert
            Assert.IsTrue(File.Exists(tempFilePath));

            provider.CleanUp(tempFilePath);
        }

        [TestMethod]
        public void TestTemporaryFileIsDeleted()
        {
            //Arrange
            var provider = new LocomotivaEventProvider();
            //Act
            var tempFilePath = provider.ReadEventPBNData(Url);
            provider.CleanUp(tempFilePath);
            //Assert
            Assert.IsTrue(!File.Exists(tempFilePath));
        }

        [TestMethod]
        public void TestUrlIsWellFormed()
        {
            //Arrange
            var provider = new LocomotivaUrlProvider();
            //Act
            var url = provider.GetUrl(new DateTime(2015, 4, 28));
            //Assert
            Assert.AreEqual(Url,url);
        }

        [TestMethod]
        public void IntegrationTest1()
        {
            //Arrange
            var eventProvider = new LocomotivaEventProvider();
            var urlProvider = new LocomotivaUrlProvider();
            //Act
            var tempFilePath = eventProvider.ReadEventPBNData(urlProvider.GetUrl(new DateTime(2015,4,28)));
            
            //Assert
            Assert.IsTrue(File.Exists(tempFilePath));
            var fi = new FileInfo(tempFilePath);
            Assert.IsTrue(fi.Length > 0);

            eventProvider.CleanUp(tempFilePath);
            Assert.IsTrue(!File.Exists(tempFilePath));
        }

        [TestMethod]
        public void TestPbnParsing()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(24,command.NoOfBoards);
            Assert.AreEqual(26,command.NoOfPairs);
            Assert.AreEqual(12,command.NoOfRounds);
        }

        [TestMethod]
        public void TestPbnParsing_DealList()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Deals.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDeal()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var deal = command.Deals.ElementAt(7);
            Assert.AreEqual("W:42.Q.73.KQJT7632 AQJ8.KJ863.JT.54 K97653.A942.86.A T.T75.AKQ9542.98",deal.PBNRepresentation);
            Assert.AreEqual(8, deal.Index);
            Assert.AreEqual((int)SysVulnerabilityEnum.None, deal.SysVulnerabilityId);
        }

        [TestMethod]
        public void TestPbnParsing_PairsList()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Pairs.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomPair()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var pair = command.Pairs.ElementAt(9);
            Assert.AreEqual(10, pair.Rank);
            Assert.AreEqual((decimal)54.14, pair.Score);
            Assert.AreEqual("POSEA VLAD - RUSU VLAD", pair.Name);
            Assert.AreEqual("POSEA VLAD", pair.Player1Name);
            Assert.AreEqual("RUSU VLAD", pair.Player2Name);
            Assert.AreEqual(21, pair.PairId);
        }

        [TestMethod]
        public void TestPbnParsing_DuplicateDealResultsList()
        {    //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var deal = command.Deals.ElementAt(1);
            Assert.AreEqual(12,deal.DealResults.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDuplicateDeal1()
        {    //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var deal = command.Deals.ElementAt(3);
            var duplicateDeal = deal.DealResults.ElementAt(5);
            //"13  3 13 31 4S   W 11 H6      -  "650"  6.6 15.4  30  70"
            Assert.AreEqual(13, duplicateDeal.NSPairIndex);
            Assert.AreEqual(31, duplicateDeal.EWPairIndex);
            Assert.AreEqual("4S", duplicateDeal.Contract);
            Assert.AreEqual((int)SysPlayer.W, duplicateDeal.Declarer);
            Assert.AreEqual(-650, duplicateDeal.Result);
            Assert.AreEqual(30, duplicateDeal.NSPercentage);
            Assert.AreEqual(70, duplicateDeal.EWPercentage);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDuplicateDeal2()
        {    
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var deal = command.Deals.ElementAt(5);
            var duplicateDeal = deal.DealResults.ElementAt(10);
            // 5 12 27  5 1N   E  7 S8      -   "90"  3.0 19.0  14  86
            Assert.AreEqual(27, duplicateDeal.NSPairIndex);
            Assert.AreEqual(5, duplicateDeal.EWPairIndex);
            Assert.AreEqual("1N", duplicateDeal.Contract);
            Assert.AreEqual((int)SysPlayer.E, duplicateDeal.Declarer);
            Assert.AreEqual(-90, duplicateDeal.Result);
            Assert.AreEqual(14, duplicateDeal.NSPercentage);
            Assert.AreEqual(86, duplicateDeal.EWPercentage);
        }

        [TestMethod]
        public void TestDuplicateDealAreWellExtracted()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new UrlProviderFactory(), new ContractScoreCalculatorModule(), new ComputeOptimalScoreModule(new ContractScoreCalculatorModule()));

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(17,command.Deals.Count(d => d.DealResults.Count() == 12));
            Assert.AreEqual(8, command.Deals.Count(d => d.DealResults.Count() == 11));
            Assert.AreEqual(1, command.Deals.Count(d => d.DealResults.Count() == 10));
            Assert.AreEqual(26, command.Deals.Count());
        }

        [TestMethod]
        public void TestScoreTableHeaderIsParsed()
        {
            //Arrange
            const string input = @"Table\2R;Round\2R;PairId_NS\2R;PairId_EW\2R;Contract\4L;Declarer\1R;Result\2R;Lead\3L;Score_NS\6R;Score_EW\6R;MP_NS\2R;MP_EW\2R;Percentage_NS\3R;Percentage_EW\3R";
            //Act
            var output = ExtractEventMetadataModule.ParseTableHeader(input);
            //Assert
            Assert.AreEqual(2,output["Table"]);
            Assert.AreEqual(2, output["Round"]);
            Assert.AreEqual(2, output["PairId_NS"]);
            Assert.AreEqual(2, output["PairId_EW"]);
            Assert.AreEqual(4, output["Contract"]);
            Assert.AreEqual(1, output["Declarer"]);
            Assert.AreEqual(2, output["Result"]);
            Assert.AreEqual(3, output["Lead"]);
            Assert.AreEqual(6, output["Score_NS"]);
            Assert.AreEqual(6, output["Score_EW"]);
            Assert.AreEqual(2, output["MP_NS"]);
            Assert.AreEqual(2, output["MP_EW"]);
            Assert.AreEqual(3, output["Percentage_NS"]);
            Assert.AreEqual(3, output["Percentage_EW"]);
        }

        [TestMethod]
        public void TestScoreTableLineIsParsed()
        {
            //Arrange
            const string header = @"Table\2R;Round\2R;PairId_NS\2R;PairId_EW\2R;Contract\4L;Declarer\1R;Result\2R;Lead\3L;Score_NS\6R;Score_EW\6R;MP_NS\2R;MP_EW\2R;Percentage_NS\3R;Percentage_EW\3R";
            const string line = " 8  5  8 24 2H   N  7 H3       -   \"50\" 20  0 100   0";
            //Act
            var headerColumns = ExtractEventMetadataModule.ParseTableHeader(header);
            var output = ExtractEventMetadataModule.ParseTableLine(line, headerColumns);

            //Assert
            Assert.AreEqual("8", output["Table"]);
            Assert.AreEqual("5", output["Round"]);
            Assert.AreEqual("8", output["PairId_NS"]);
            Assert.AreEqual("24", output["PairId_EW"]);
            Assert.AreEqual("2H", output["Contract"]);
            Assert.AreEqual("N", output["Declarer"]);
            Assert.AreEqual("7", output["Result"]);
            Assert.AreEqual("H3", output["Lead"]);
            Assert.AreEqual("-", output["Score_NS"]);
            Assert.AreEqual("50", output["Score_EW"]);
            Assert.AreEqual("20", output["MP_NS"]);
            Assert.AreEqual("0", output["MP_EW"]);
            Assert.AreEqual("100", output["Percentage_NS"]);
            Assert.AreEqual("0", output["Percentage_EW"]);
        }

        [TestMethod]
        public void TestTotalScoreTableHeaderIsParsed()
        {
            //Arrange
            const string input = @"Rank\2R;PairId\2R;Table\2R;Direction\5R;TotalScoreMP\3R;TotalPercentage\5R;Names\40L;NrBoards\2R";
            //Act
            var output = ExtractEventMetadataModule.ParseTableHeader(input);
            //Assert
            Assert.AreEqual(2, output["Rank"]);
            Assert.AreEqual(2, output["PairId"]);
            Assert.AreEqual(2, output["Table"]);
            Assert.AreEqual(5, output["Direction"]);
            Assert.AreEqual(3, output["TotalScoreMP"]);
            Assert.AreEqual(5, output["TotalPercentage"]);
            Assert.AreEqual(40, output["Names"]);
            Assert.AreEqual(2, output["NrBoards"]);
        }

        [TestMethod]
        public void TestTotalScoreTableLineIsParsed()
        {
            //Arrange
            const string header = @"Rank\2R;PairId\2R;Table\2R;Direction\5R;TotalScoreMP\3R;TotalPercentage\5R;Names\40L;NrBoards\2R";
            const string line = " 1  5  5 \"N-S\" 305 69.32 \"GAVRILIU MADALINA - BONTAS BUJOR\"       22";
            //Act
            var headerColumns = ExtractEventMetadataModule.ParseTableHeader(header);
            var output = ExtractEventMetadataModule.ParseTableLine(line, headerColumns);

            //Assert
            Assert.AreEqual("1", output["Rank"]);
            Assert.AreEqual("5", output["PairId"]);
            Assert.AreEqual("5", output["Table"]);
            Assert.AreEqual("N-S", output["Direction"]);
            Assert.AreEqual("305", output["TotalScoreMP"]);
            Assert.AreEqual("69.32", output["TotalPercentage"]);
            Assert.AreEqual("GAVRILIU MADALINA - BONTAS BUJOR", output["Names"]);
            Assert.AreEqual("22", output["NrBoards"]);
        }
    }
}
