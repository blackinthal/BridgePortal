using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using ElmahExtensions;

namespace Domain.EventStorage
{
    /// <summary>
    /// Actual implementation of <see cref="IEventStore"/>, which deals with
    /// serialization and naming in order to provide bridge between event-centric
    /// domain code and string-based append-only persistence
    /// </summary>
    public class EventStore : IEventStore
    {
        readonly IAppendOnlyStore _appendOnlyStore;

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }


        public long CountStoredEvents(int? eventId = null)
        {
            return _appendOnlyStore.Count(eventId);
        }

        public List<StoredEvent> GetAllStoredEventsSince(long storedEventId)
        {
            throw new NotImplementedException();
        }

        public List<StoredEvent> GetAllStoredForProcessId(Guid processId, int take = 1)
        {
            var records = _appendOnlyStore
               .ReadRecords(processId, take)
               .ToList();

            return records;
        }

        public StoredEvent Append(IDomainEvent domainEvent)
        {
            var storedEvent = new StoredEvent(
                domainEvent.GetType().FullName + ", " + domainEvent.GetType().Assembly.GetName().Name,
                domainEvent.OccurredOn,
                domainEvent.ToString(),
                domainEvent.EventId,
                StoredEvent.ToStorePayload(domainEvent),
                domainEvent.UserId,
                domainEvent.ProcessId,
				domainEvent.UserId);

            try
            {
                _appendOnlyStore.Append(storedEvent);

                return storedEvent;
            }
            catch (Exception e)
            {
                CustomErrorSignal.Handle(e);
            }

            return null;

        }

        public void Clear()
        {
			if (_appendOnlyStore != null)
				_appendOnlyStore.Close();
        }
    }
}