using System;
using System.Collections.Generic;
using AutoMapper;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.EventAggregate.DomainEvents;
using Bridge.Domain.Models;
using Domain.Contracts;
using Domain.Extensions;

namespace Bridge.Domain.EventAggregate
{
    public class EventAggregate : IAggregate
    {
        private readonly BridgeContext _context;
        public List<IDomainEvent> DomainEvents { get; private set; }

        public EventAggregate(BridgeContext context)
        {
            _context = context;
            DomainEvents = new List<IDomainEvent>();
        }

        public void ImportEvent(ImportEvent command)
        {
            try
            {
                var succesEvent = new EventImported();
                DomainEventExtensions.PrepareSuccessfulEvent(succesEvent,command);
                DomainEvents.Add(succesEvent);
            }
            catch (Exception ex)
            {
                var errorEvent = Mapper.Map<ImportEventAttempted>(command);
                DomainEventExtensions.PrepareAttemptedEvent(errorEvent,ex,command);
                DomainEvents.Add(errorEvent);
            }
        }
    }
}
