using System;
using System.Globalization;
using System.Linq;
using Bridge.Domain;
using Bridge.Domain.Extensions;
using Bridge.Domain.StaticModels;
using Dds.Net;

namespace Bridge.WebAPI.Modules
{
    public class ContractScoreCalculatorModule
    {
        private const int MinorSuitTrickScore = 20;
        private const int MajorSuitTrickScore = 30;
        private const int FirstNoTrumpTrickScore = 40;
        private const int SubsequentNoTrumpTricksScore = 30;
        private const int DoubledNotVulnerableOvertrickScore = 100;
        private const int RedoubledNotVulnerableOvertrickScore = 200;
        private const int DoubledVulnerableOvertrickScore = 200;
        private const int RedoubledVulnerableOvertrickScore = 400;
        private const int DoubledContractBonus = 50;
        private const int RedoubledContractBonus = 100;
        private const int PartialContractBonus = 50;
        private const int NonVulnerableGameBonus = 300;
        private const int VulnerableGameBonus = 500;
        private const int NonVulnerableSmallSlamBonus = 500;
        private const int VulnerableSmallSlamBonus = 750;
        private const int NonVulnerableBigSlamBonus = 1000;
        private const int VulnerableBigSlamBonus = 1500;
        private const int GameBonusLimit = 100;

        private const int NonVulnerableUndertrickPenalty = 50;
        private const int VulnerableUndertrickPenalty = 100;
        private readonly int[] _doubledVulnerableUndertrickPenalties = {200, 300};
        private readonly int[] _redoubledVulnerableUndertrickPenalties = {400, 600};
        private readonly int[] _doubledNotVulnerableUndertrickPenalties = {100, 200, 200, 300};
        private readonly int[] _redoubledNotVulnerableUndertrickPenalties = {200, 400, 400, 600};

        private Contract _contract;
        private bool _vulnerable;
        private ContractDenomination _contractType;

        public int CalculateScore(Contract c, int tricksMade, SysVulnerabilityEnum vulnerability)
        {
            _contract = c;

            if (c.Level == 0)
                return 0;

            SetVulnerability(vulnerability);
            SetContractType();

            var trickDifference = tricksMade - _contract.TricksToBeMade;

            var score = trickDifference >= 0 ? CalculateContractMadePoints(trickDifference) : -1 * CalculateContractPenaltyPoints(Math.Abs(trickDifference));

            return _contract.PlayerPosition == PlayerPosition.North || _contract.PlayerPosition == PlayerPosition.South ? score : -1 * score;
        }
        public int CalculateScore(Contract c, string tricksMade, SysVulnerabilityEnum vulnerability)
        {
            if (tricksMade.Length == 0)
                return 0;

            var firstLetter = tricksMade.ElementAt(0);
            if (firstLetter == '=')
                return CalculateScore(c, c.TricksToBeMade, vulnerability);
            var tricks = int.Parse(tricksMade.ElementAt(1).ToString(CultureInfo.InvariantCulture));

            return firstLetter == '+' ? CalculateScore(c, c.TricksToBeMade + tricks, vulnerability) : CalculateScore(c, c.TricksToBeMade - tricks, vulnerability);
        }
       
        public Tuple<int, Contract> ComputeOptimalContract(string pbnDeal, SysVulnerabilityEnum vulnerabilty)
        {
            var makeableContracts = DoubleDummyModule.CalculateMakeableContracts(pbnDeal).AsQueryable();

            var bestContract = GetBestUndoubledContract(makeableContracts, vulnerabilty);

            var bestScore = CalculateScore(bestContract, bestContract.TricksToBeMade, vulnerabilty);

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
                    var tricksDown = nextContract.Level - contract.Level;
                    var score = CalculateScore(nextContract, nextContract.TricksToBeMade - tricksDown, vulnerabilty);
                    if (Math.Abs(score) >= Math.Abs(bestScore)) return;

                    bestScore = score;
                    bestContract = nextContract.Clone();
                });
            }

            return new Tuple<int, Contract>(bestScore, bestContract);
        }

        public Contract GetBestUndoubledContract(IQueryable<Contract> contractList, SysVulnerabilityEnum vulnerability)
        {
            var bestNSContract = contractList
                .PlayedByNS()
                .OrderByDescending(c => CalculateScore(c, c.TricksToBeMade, vulnerability))
                .FirstOrDefault();

            var bestEWContract = contractList
                .PlayedByEW()
                .OrderBy(c => CalculateScore(c, c.TricksToBeMade, vulnerability))
                .FirstOrDefault();

            if (bestNSContract == null && bestEWContract == null)
                return null;
            if (bestNSContract == null)
                return bestEWContract;
            if (bestEWContract == null)
                return bestNSContract;

            return bestNSContract.CanBidOver(bestEWContract) ? bestNSContract : bestEWContract;
        }

        private void SetContractType()
        {
            if (_contract.Trump.Equals(Trump.Clubs) || _contract.Trump.Equals(Trump.Diamonds))
                _contractType = ContractDenomination.MinorSuit;
            else
            {
                _contractType = _contract.Trump.Equals(Trump.NoTrump)
                    ? ContractDenomination.NoTrump
                    : ContractDenomination.MajorSuit;
            }
        }
        private void SetVulnerability(SysVulnerabilityEnum vulnerability)
        {
            switch (vulnerability)
            {
                case SysVulnerabilityEnum.All:
                    _vulnerable = true;
                    break;
                case SysVulnerabilityEnum.NS:
                    _vulnerable = _contract.PlayerPosition == PlayerPosition.North ||
                                  _contract.PlayerPosition == PlayerPosition.South;
                    break;
                case SysVulnerabilityEnum.EW:
                    _vulnerable = _contract.PlayerPosition == PlayerPosition.East ||
                                  _contract.PlayerPosition == PlayerPosition.West;
                    break;
                default:
                    _vulnerable = false;
                    break;
            }
        }
        private int CalculateContractMadePoints(int overTricks)
        {
            var contractPoints = ComputeContractPoints();
            var overtrickPoints = ComputeOvertrickPoints(overTricks);
            var gamePoints = ComputeGamePoints(contractPoints);
            var slamBonus = ComputeSlamBonus();
            var doubledContractBonus = ComputeDoubledBonus();

            return contractPoints + overtrickPoints + gamePoints + slamBonus + doubledContractBonus;
        }
        private int ComputeContractPoints()
        {
            var points = 0;
            switch (_contractType)
            {
                case ContractDenomination.MinorSuit:
                    points = MinorSuitTrickScore * _contract.Level;
                    break;
                case ContractDenomination.MajorSuit:
                    points = MajorSuitTrickScore * _contract.Level;
                    break;
                case ContractDenomination.NoTrump:
                    points = FirstNoTrumpTrickScore + SubsequentNoTrumpTricksScore * (_contract.Level - 1);
                    break;
            }

            if (_contract.Doubled)
                return points * 2;
            if (_contract.Redoubled)
                return points * 4;

            return points;
        }
        private int ComputeOvertrickPoints(int overTricks)
        {
            var overtrickPoints = 0;

            if (overTricks == 0)
                return overtrickPoints;

            if (_contract.Redoubled)
            {
                overtrickPoints = overTricks*(_vulnerable ? RedoubledVulnerableOvertrickScore : RedoubledNotVulnerableOvertrickScore);
            }
            else
            {
                if (_contract.Doubled)
                {
                    overtrickPoints = overTricks * (_vulnerable ? DoubledVulnerableOvertrickScore : DoubledNotVulnerableOvertrickScore);
                }
                else
                {
                    overtrickPoints = overTricks * GetUndoubledOvertrickPoints();
                }
            }

            return overtrickPoints;
        }
        private int GetUndoubledOvertrickPoints()
        {
            switch (_contractType)
            {
                case ContractDenomination.MinorSuit:
                    return MinorSuitTrickScore;
                case ContractDenomination.MajorSuit:
                    return MajorSuitTrickScore;
                default:
                    return SubsequentNoTrumpTricksScore;
            }
        }
        private int ComputeGamePoints(int contractPoints)
        {
            if (contractPoints >= GameBonusLimit)
            {
                return _vulnerable ? VulnerableGameBonus : NonVulnerableGameBonus;
            }

            return PartialContractBonus;
        }
        private int ComputeSlamBonus()
        {
            if (_contract.SmallSlam)
            {
                return _vulnerable ? VulnerableSmallSlamBonus : NonVulnerableSmallSlamBonus;
            }
            if (_contract.BigSlam)
            {
                return _vulnerable ? VulnerableBigSlamBonus : NonVulnerableBigSlamBonus;
            }
            return 0;
        }
        private int ComputeDoubledBonus()
        {
            if (_contract.Redoubled)
                return RedoubledContractBonus;
            return _contract.Doubled ? DoubledContractBonus : 0;
        }
        private int CalculateContractPenaltyPoints(int underTricks)
        {
            if (_contract.Redoubled)
                return ComputePenaltyPoints(underTricks, _vulnerable ? _redoubledVulnerableUndertrickPenalties : _redoubledNotVulnerableUndertrickPenalties);
            if (_contract.Doubled)
                return ComputePenaltyPoints(underTricks, _vulnerable ? _doubledVulnerableUndertrickPenalties : _doubledNotVulnerableUndertrickPenalties);
            
            return underTricks*(_vulnerable ? VulnerableUndertrickPenalty : NonVulnerableUndertrickPenalty);
        }
        private static int ComputePenaltyPoints(int underTricks, int[] underTrickPenalties)
        {
            var penaltyPoints = 0;
            var lastPenalty = underTrickPenalties.Last();

            for (var i = 0; i < underTrickPenalties.Length && i < underTricks ; i++)
            {
                penaltyPoints += underTrickPenalties[i];
            }

            for (var i = underTrickPenalties.Length; i < underTricks; i++)
            {
                penaltyPoints += lastPenalty;
            }

            return penaltyPoints;
        }
    }
}
