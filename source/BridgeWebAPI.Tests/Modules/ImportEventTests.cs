using System;
using System.IO;
using System.Linq;
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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Deals.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomDeal()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider());

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
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider());

            //Act
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));

            //Assert
            Assert.AreEqual(26, command.Pairs.Count);
        }

        [TestMethod]
        public void TestPbnParsing_RandomPair()
        {
            //Arrange
            var module = new ExtractEventMetadataModule(new LocomotivaEventProvider(), new LocomotivaUrlProvider());

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
    }
}
