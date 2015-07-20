using System;
using System.Collections.Generic;
using Domain.Contracts;

namespace Bridge.Domain.EventAggregate.Commands
{
    public class ImportEvent : CommandBase
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int SysEventTypeId { get; set; }
        public IList<DealMetadata> Deals { get; set; }
        public IList<PairMetadata> Pairs { get; set; }
        public int NoOfBoards { get; set; }
        public int NoOfPairs { get; set; }
        public int NoOfRounds { get; set; }
        public ImportEvent()
        {
            Deals = new List<DealMetadata>();
            Pairs = new List<PairMetadata>();
        }
    }

    public class PairMetadata
    {
        public string Name { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public decimal Result { get; set; }
        public int Rank { get; set; }
        public int PairId { get; set; }
    }

    public class DealMetadata
    {
        public string PBNRepresentation { get; set; }
        public int Index { get; set; }
        public IList<DuplicateDealMetadata> DealResults { get; set; }
        public int SysVulnerabilityId { get; set; }
        public DealMetadata()
        {
            DealResults = new List<DuplicateDealMetadata>();
        }
    }

    public class DuplicateDealMetadata
    {
        public string Contract { get; set; }
        public int Declarer { get; set; }
        public int Result { get; set; }
        public int NSPairIndex { get; set; }
        public int EWPairIndex { get; set; }
        public decimal NSPercentage { get; set; }
        public decimal EWPercentage { get; set; }
    }
}
