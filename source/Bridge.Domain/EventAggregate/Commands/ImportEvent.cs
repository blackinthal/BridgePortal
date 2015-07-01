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
        public IEnumerable<DealMetadata> Deals { get; set; }
        public IEnumerable<PairMetadata> Pairs { get; set; }
    }

    public class PairMetadata
    {
        public string Name { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public decimal Result { get; set; }
        public int Rank { get; set; }
    }

    public class DealMetadata
    {
        public string PBNRepresentation { get; set; }
        public int Index { get; set; }
        public IEnumerable<DuplicateDealMetadata> DealResults { get; set; }
    }

    public class DuplicateDealMetadata
    {
        public string Contract { get; set; }
        public int Declarer { get; set; }
        public int Result { get; set; }
        public int NSPairIndex { get; set; }
        public int EWPairIndex { get; set; }
    }
}
