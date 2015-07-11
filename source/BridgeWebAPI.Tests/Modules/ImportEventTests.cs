using System;
using System.IO;
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
    }
}
