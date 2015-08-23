using System;
using System.Linq;
using Bridge.Domain;
using Bridge.Domain.Extensions;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Dds.Net;

namespace Bridge.WebAPI.Modules
{
    public class ComputeOptimalScoreModule
    {
        private readonly ContractScoreCalculatorModule _calculateScore;

        public ComputeOptimalScoreModule(ContractScoreCalculatorModule calculateScore)
        {
            _calculateScore = calculateScore;
        }

        public Tuple<int,Contract> ComputeOptimalContract(string pbnDeal, SysVulnerabilityEnum vulnerabilty)
        {
            var makeableContracts = DoubleDummyModule.CalculateMakeableContracts(pbnDeal).AsQueryable();
            var bestContract = GetBestUndoubledContract(makeableContracts, vulnerabilty);

            var bestScore = _calculateScore.CalculateScore(bestContract, bestContract.TricksToBeMade, vulnerabilty);

            var contractPlayedByNs = bestContract.PlayerPosition == PlayerPosition.North ||
                                     bestContract.PlayerPosition == PlayerPosition.South;

            var nextContract = bestContract;

            for (var i = 0; i < 5; i++)
            {
                nextContract = nextContract.GetNextContract();
                if (nextContract == null)
                    break;

                var contracts = makeableContracts.GetContractsByTrumpAndPlayingSide(contractPlayedByNs, nextContract.Trump)
                    .ToList();
                nextContract.Doubled = true;
                nextContract.PlayerPosition = contractPlayedByNs ? PlayerPosition.East : PlayerPosition.North;

                contracts.ForEach(contract =>
                {
                    var tricksDown = nextContract.Value - contract.Value;
                    var score = _calculateScore.CalculateScore(nextContract, nextContract.TricksToBeMade - tricksDown, vulnerabilty);
                    if (Math.Abs(score) >= Math.Abs(bestScore)) return;

                    bestScore = score;
                    bestContract = nextContract.Clone();
                });
            }

            return new Tuple<int, Contract>(bestScore, bestContract);
        }

        public Contract GetBestUndoubledContract(IQueryable<Contract> contractList, SysVulnerabilityEnum vulnerability)
        {
            var bestNSContract = contractList.PlayedByNS()
                .OrderByDescending(c => _calculateScore.CalculateScore(c,c.TricksToBeMade,vulnerability))
                .FirstOrDefault();
            var bestEWContract = contractList.PlayedByEW()
                .OrderBy(c => _calculateScore.CalculateScore(c, c.TricksToBeMade, vulnerability))
                .FirstOrDefault();

            if (bestNSContract == null && bestEWContract == null)
                return null;
            if (bestNSContract == null)
                return bestEWContract;
            if (bestEWContract == null)
                return bestNSContract;

            return bestNSContract.GreaterThan(bestEWContract) ? bestNSContract : bestEWContract;
        }
    }
}
