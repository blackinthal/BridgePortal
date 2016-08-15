using Bridge.Domain;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Modules
{
    [TestClass]
    public class ComputeContractScore
    {
        [TestMethod]
        public void TestContract1()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("1H", PlayerPosition.East);
            //Act
            var score = module.CalculateScore(contract, 9, SysVulnerabilityEnum.None);
            var score2 = module.CalculateScore(contract, "+2", SysVulnerabilityEnum.None);
            //Assert
            Assert.AreEqual(-140, score);
            Assert.AreEqual(-140, score2);
        }

        [TestMethod]
        public void TestContract2()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4SX", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 8, SysVulnerabilityEnum.NS);
            var score2 = module.CalculateScore(contract, "-2", SysVulnerabilityEnum.NS);
            //Assert
            Assert.AreEqual(-500, score);
            Assert.AreEqual(-500, score2);
        }

        [TestMethod]
        public void TestContract3()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("2D", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 10, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, "+2", SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(130, score);
            Assert.AreEqual(130, score2);
        }

        [TestMethod]
        public void TestContract4()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("1NT", PlayerPosition.West);
            //Act
            var score = module.CalculateScore(contract, 9, SysVulnerabilityEnum.All);
            var score2 = module.CalculateScore(contract, "+2", SysVulnerabilityEnum.All);
            //Assert
            Assert.AreEqual(-150, score);
            Assert.AreEqual(-150, score2);
        }

        [TestMethod]
        public void TestContract5()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("3NT", PlayerPosition.South);
            //Act
            var score = module.CalculateScore(contract, 8, SysVulnerabilityEnum.NS);
            var score2 = module.CalculateScore(contract, "-1", SysVulnerabilityEnum.NS);
            //Assert
            Assert.AreEqual(-100, score);
            Assert.AreEqual(-100, score2);
        }

        [TestMethod]
        public void TestContract6()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("3H", PlayerPosition.South);
            //Act
            var score = module.CalculateScore(contract, 10, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, "+1", SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(170, score);
            Assert.AreEqual(170, score2);
        }

        [TestMethod]
        public void TestContract7()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("3C", PlayerPosition.West);
            //Act
            var score = module.CalculateScore(contract, 9, SysVulnerabilityEnum.All);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.All);
            //Assert
            Assert.AreEqual(-110, score);
            Assert.AreEqual(-110, score2);
        }

        [TestMethod]
        public void TestContract8()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("7H", PlayerPosition.East);
            //Act
            var score = module.CalculateScore(contract, 13, SysVulnerabilityEnum.None);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.None);
            //Assert
            Assert.AreEqual(-1510, score);
            Assert.AreEqual(-1510, score2);
        }

        [TestMethod]
        public void TestContract9()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4SX", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 10, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(590, score);
            Assert.AreEqual(590, score2);
        }

        [TestMethod]
        public void TestContract10()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("2NT", PlayerPosition.East);
            //Act
            var score = module.CalculateScore(contract, 8, SysVulnerabilityEnum.All);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.All);
            //Assert
            Assert.AreEqual(-120, score);
            Assert.AreEqual(-120, score2);
        }

        [TestMethod]
        public void TestContract11()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("6C", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 12, SysVulnerabilityEnum.None);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.None);
            //Assert
            Assert.AreEqual(920, score);
            Assert.AreEqual(920, score2);
        }

        [TestMethod]
        public void TestContract12()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("2D", PlayerPosition.East);
            //Act
            var score = module.CalculateScore(contract, 9, SysVulnerabilityEnum.NS);
            var score2 = module.CalculateScore(contract, "+1", SysVulnerabilityEnum.NS);
            //Assert
            Assert.AreEqual(-110, score);
            Assert.AreEqual(-110, score2);
        }

        [TestMethod]
        public void TestContract13()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4HXX", PlayerPosition.West);
            //Act
            var score = module.CalculateScore(contract, 10, SysVulnerabilityEnum.All);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.All);
            //Assert
            Assert.AreEqual(-1080, score);
            Assert.AreEqual(-1080, score2);
        }

        [TestMethod]
        public void TestContract14()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("5S", PlayerPosition.South);
            //Act
            var score = module.CalculateScore(contract, 10, SysVulnerabilityEnum.None);
            var score2 = module.CalculateScore(contract, "-1", SysVulnerabilityEnum.None);
            //Assert
            Assert.AreEqual(-50, score);
            Assert.AreEqual(-50, score2);
        }

        [TestMethod]
        public void TestContract15()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4H", PlayerPosition.East);
            //Act
            var score = module.CalculateScore(contract,11, SysVulnerabilityEnum.NS);
            var score2 = module.CalculateScore(contract, "+1", SysVulnerabilityEnum.NS);
            //Assert
            Assert.AreEqual(-450, score);
            Assert.AreEqual(-450, score2);
        }

        [TestMethod]
        public void TestContract16()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("3NT", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 9, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, "=", SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(400, score);
            Assert.AreEqual(400, score2);
        }

        [TestMethod]
        public void TestContract17()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("Pass", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 0, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, string.Empty, SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(0, score);
            Assert.AreEqual(0, score2);
        }

        [TestMethod]
        public void TestContract18()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4SX", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 7, SysVulnerabilityEnum.EW);
            var score2 = module.CalculateScore(contract, "-3", SysVulnerabilityEnum.EW);
            //Assert
            Assert.AreEqual(-500, score);
            Assert.AreEqual(-500, score2);
        }

        [TestMethod]
        public void TestContract19()
        {
            //Arrange
            var module = new ContractScoreCalculatorModule();
            var contract = new Contract("4SX", PlayerPosition.North);
            //Act
            var score = module.CalculateScore(contract, 7, SysVulnerabilityEnum.All);
            var score2 = module.CalculateScore(contract, "-3", SysVulnerabilityEnum.All);
            //Assert
            Assert.AreEqual(-800, score);
            Assert.AreEqual(-800, score2);
        }
    }
}
