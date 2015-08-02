using System;
using System.Collections.Generic;
using Bridge.Domain;
using Bridge.Domain.Extensions;
using Bridge.Domain.Modules;
using Bridge.Domain.StaticModels;
using Dds.Net;
using WebGrease.Css.Extensions;

namespace Bridge.WebAPI.Modules
{
    public class ComputeOptimalScoreModule
    {
        private readonly ContractScoreCalculatorModule _calculateScore;
        private readonly DoubleDummyModule _solveBoard;

        public ComputeOptimalScoreModule(ContractScoreCalculatorModule calculateScore, DoubleDummyModule solveBoard)
        {
            _calculateScore = calculateScore;
            _solveBoard = solveBoard;
        }

        public Tuple<int,Contract> ComputeOptimalContract(string pbnDeal, SysVulnerabilityEnum vulnerabilty)
        {
            var makeableContracts = _solveBoard.CalculateMakeableContracts(pbnDeal);
            var bestContract = GetBestContract(makeableContracts);

            var bestScore = Math.Abs(_calculateScore.CalculateScore(bestContract, 0, vulnerabilty));

            var contractPlayedByNs = bestContract.PlayerPosition == PlayerPosition.North ||
                                     bestContract.PlayerPosition == PlayerPosition.South;

            var nextContract = bestContract.GetNextContract();

            for (var i = 0; i < 5; i++)
            {
                nextContract = nextContract.GetNextContract();
                if (nextContract == null)
                    break;

                var contracts = makeableContracts.GetContractsByTrumpAndPlayingSide(contractPlayedByNs, nextContract.Trump);
                nextContract.Doubled = true;

                contracts.ForEach(contract =>
                {
                    var score = Math.Abs(_calculateScore.CalculateScore(nextContract, contract.Value - nextContract.Value, vulnerabilty));
                    if (score >= bestScore) return;

                    bestScore = score;
                    bestContract = nextContract.Copy();
                });
            }

            return new Tuple<int, Contract>(bestScore, bestContract);
        }

        private Contract GetBestContract(IEnumerable<Contract> contractList)
        {
            var bestContract = new Contract();
            var bestScore = 0;
            contractList.ForEach(contract =>
            {
                //
            });
            return bestContract;
        }
    }
}
