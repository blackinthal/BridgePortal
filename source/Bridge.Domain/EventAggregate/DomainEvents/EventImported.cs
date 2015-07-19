using System;
using Domain.Contracts;

namespace Bridge.Domain.EventAggregate.DomainEvents
{
    public class EventImported : IDomainEvent
    {
        public int EventId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int UserId { get; set; }
        public Guid ProcessId { get; set; }
        public int[] DealIds { get; set; }
        public int[] PairIds { get; set; }
    }
}
