using System;
using System.Collections.Generic;
using Domain.EventStorage;

namespace Domain.Contracts
{
    public interface IAppendOnlyStore
    {

        IEnumerable<StoredEvent> ReadRecords(Guid fromSessionId, int maxCount = 1);

        IEnumerable<StoredEvent> ReadRecords(int forEventId, int maxCount = 1);

        void Initialize();

        void Append(StoredEvent storedEvent);

        long Count(int? eventId = null);

        bool Close();
    }
}