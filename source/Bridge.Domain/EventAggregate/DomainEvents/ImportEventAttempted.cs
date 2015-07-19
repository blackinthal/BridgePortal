using System;
using System.Collections.Generic;
using Bridge.Domain.EventAggregate.Commands;
using Domain.Contracts;

namespace Bridge.Domain.EventAggregate.DomainEvents
{
    public class ImportEventAttempted : IDomainEventError
    {
        public int EventId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int UserId { get; set; }
        public Guid ProcessId { get; set; }
        public string Errors { get; set; }
        public Guid ErrorId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int SysEventTypeId { get; set; }
        public IList<DealMetadata> Deals { get; set; }
        public IList<PairMetadata> Pairs { get; set; }
        public int NoOfBoards { get; set; }
        public int NoOfPairs { get; set; }
        public int NoOfRounds { get; set; }
    }
}
