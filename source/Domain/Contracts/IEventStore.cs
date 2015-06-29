using System;
using System.Collections.Generic;
using Domain.EventStorage;

namespace Domain.Contracts
{
    public interface IEventStore
    {
        long CountStoredEvents(int? eventId = null);

        List<StoredEvent> GetAllStoredEventsSince(long storedEventId);

        List<StoredEvent> GetAllStoredForProcessId(Guid processId, int take = 1);

        StoredEvent Append(IDomainEvent domainEvent);

        void Clear();
    }
}
