using System;
using System.Collections.Generic;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.Models;
using Domain.Contracts;

namespace Bridge.Domain.EventAggregate
{
    public class EventAggregate : IAggregate
    {
        private readonly BridgeContext _context;
        public List<IDomainEvent> DomainEvents { get; private set; }

        public EventAggregate(BridgeContext context)
        {
            _context = context;
        }

        public void ImportEvent(ImportEvent command)
        {
            
        }
    }
}
