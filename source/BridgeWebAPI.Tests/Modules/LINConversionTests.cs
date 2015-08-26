using Bridge.Domain;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Modules.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Modules
{
    [TestClass]
    public class LinConversionTests
    {
        [TestMethod]
        public void DeclarerNDealerN()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.North);
            const int dealerId = (int) SysPlayer.N;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerNDealerS()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.North);
            const int dealerId = (int)SysPlayer.S;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerNDealerE()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.North);
            const int dealerId = (int)SysPlayer.E;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerNDealerW()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.North);
            const int dealerId = (int)SysPlayer.W;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerSDealerN()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.South);
            const int dealerId = (int)SysPlayer.N;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerSDealerS()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.South);
            const int dealerId = (int)SysPlayer.S;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerSDealerE()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.South);
            const int dealerId = (int)SysPlayer.E;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerSDealerW()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.South);
            const int dealerId = (int)SysPlayer.W;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerEDealerN()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.East);
            const int dealerId = (int)SysPlayer.N;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerEDealerS()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.East);
            const int dealerId = (int)SysPlayer.S;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerEDealerE()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.East);
            const int dealerId = (int)SysPlayer.E;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerEDealerW()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.East);
            const int dealerId = (int)SysPlayer.W;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerWDealerN()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.West);
            const int dealerId = (int)SysPlayer.N;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerWDealerS()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.West);
            const int dealerId = (int)SysPlayer.S;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerWDealerE()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.West);
            const int dealerId = (int)SysPlayer.E;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|p|mb|p|mb|1S|mb|p|mb|p|mb|p|", output);
        }

        [TestMethod]
        public void DeclarerWDealerW()
        {
            //Arrange
            var contract = new Contract("1S", PlayerPosition.West);
            const int dealerId = (int)SysPlayer.W;
            //Act
            var output = ParsePBNHelpers.ConvertToPbnBiddingSequence(contract, dealerId);
            //Assert
            Assert.AreEqual("|b|mb|1S|mb|p|mb|p|mb|p|", output);
        }
    }
}
