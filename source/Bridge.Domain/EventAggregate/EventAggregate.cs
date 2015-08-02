using System;
using System.Collections.Generic;
using System.Linq;
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

                var bridgeEvent = Mapper.Map<Event>(command);

                var pairsDictionary = new Dictionary<int, Pair>();
                command.Pairs.Each(pairMetadata =>
                {
                    var pair = Mapper.Map<Pair>(pairMetadata);
                    pairsDictionary.Add(pairMetadata.PairId, pair);
                    bridgeEvent.Pairs.Add(pair);
                });

                command.Deals.Each(dealMetadata =>
                {
                    var deal = Mapper.Map<Deal>(dealMetadata);
                    dealMetadata.DealResults.Each(duplicateDealMetadata =>
                    {
                        var duplicateDeal = Mapper.Map<DuplicateDeal>(duplicateDealMetadata);
                        duplicateDeal.Pair = pairsDictionary[duplicateDealMetadata.EWPairIndex];
                        duplicateDeal.Pair1 = pairsDictionary[duplicateDealMetadata.NSPairIndex];
                        deal.DuplicateDeals.Add(duplicateDeal);
                    });

                    bridgeEvent.Deals.Add(deal);
                });

                _context.Events.Add(bridgeEvent);
                _context.SaveChanges();

                DomainEventExtensions.PrepareSuccessfulEvent(succesEvent,command);
                succesEvent.EventId = bridgeEvent.Id;
                succesEvent.DealIds = bridgeEvent.Deals.Select(c => c.Id).ToArray();
                succesEvent.PairIds = bridgeEvent.Pairs.Select(c => c.Id).ToArray();
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
