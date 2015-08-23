using System.Linq;
using Bridge.Domain;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Bridge.WebAPI.Modules;
using Dds.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Modules
{
    [TestClass]
    public class ComputeOptimalScoreModuleTests
    {
        [TestMethod]
        public void TestBestUndoubledContractIsComputed()
        {
            //Arrange
            const string pbnDeal = "W:752.854.Q3.A8732 AT83.T.AK9752.K9 KJ6.AKQJ732..J54 Q94.96.JT864.QT6";
            const SysVulnerabilityEnum vulnerability = SysVulnerabilityEnum.None;

            var module = new ComputeOptimalScoreModule(new ContractScoreCalculatorModule());
            var makeableContracts = DoubleDummyModule.CalculateMakeableContracts(pbnDeal);
            //Act
            var result = module.GetBestUndoubledContract(makeableContracts.AsQueryable(), vulnerability);

            //Assert
            Assert.AreEqual(3, result.Value);
            Assert.AreEqual(Trump.Spades, result.Trump);
        }
        [TestMethod]
        public void TestOptimalContractIsComputed()
        {
            //Arrange
            const string pbnDeal = "W:752.854.Q3.A8732 AT83.T.AK9752.K9 KJ6.AKQJ732..J54 Q94.96.JT864.QT6";
            const SysVulnerabilityEnum vulnerability = SysVulnerabilityEnum.None;

            var module = new ComputeOptimalScoreModule(new ContractScoreCalculatorModule());

            //Act
            var result = module.ComputeOptimalContract(pbnDeal, vulnerability);

            //Assert
            Assert.AreEqual(100,result.Item1);
            Assert.AreEqual(4,result.Item2.Value);
            Assert.AreEqual(Trump.Hearts, result.Item2.Trump);
            Assert.IsTrue(result.Item2.PlayerPosition == PlayerPosition.East || result.Item2.PlayerPosition == PlayerPosition.West);
        }
    }
}
