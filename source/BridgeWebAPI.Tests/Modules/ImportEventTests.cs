using System;
using System.IO;
using System.Linq;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using BridgeWebAPI.Modules;
using BridgeWebAPI.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BridgeWebAPI.Tests.Modules
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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Deals.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDeal()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Pairs.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomPair()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var pair = command.Pairs.ElementAt(9);
            Assert.AreEqual(10, pair.Rank);
            Assert.AreEqual((decimal)54.14, pair.Result);
            Assert.AreEqual("POSEA VLAD - RUSU VLAD", pair.Name);
            Assert.AreEqual("POSEA VLAD", pair.Player1Name);
            Assert.AreEqual("RUSU VLAD", pair.Player2Name);
            Assert.AreEqual(21, pair.PairId);
        }

        [TestMethod]
        public void TestPbnParsing_DuplicateDealResultsList()
        {    //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            var deal = command.Deals.ElementAt(1);
            Assert.AreEqual(12,deal.DealResults.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDuplicateDeal1()
        {    //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider(), new ContractScoreCalculatorModule());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(17,command.Deals.Count(d => d.DealResults.Count() == 12));
            Assert.AreEqual(8, command.Deals.Count(d => d.DealResults.Count() == 11));
            Assert.AreEqual(1, command.Deals.Count(d => d.DealResults.Count() == 10));
            Assert.AreEqual(26, command.Deals.Count());
        }
    }
}
