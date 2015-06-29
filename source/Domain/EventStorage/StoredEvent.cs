using System;
using Domain.AssertionConcerns;
using Domain.Contracts;

namespace Domain.EventStorage
{
    public class StoredEvent : IEquatable<StoredEvent>
    {
        public StoredEvent(string typeName, DateTime occurredOn, string eventDescription, int eventId, string eventBody, int userId, Guid processId, long inStoreId = 0)
        {
            AssertionConcern.AssertArgumentNotEmpty(typeName, "The event type name is required.");
            AssertionConcern.AssertArgumentLength(typeName, 100, "The event type name must be 100 characters or less.");

            AssertionConcern.AssertArgumentNotEmpty(eventDescription, "The event description is required.");
            AssertionConcern.AssertArgumentLength(eventDescription, 65000, "The event description must be 300 characters or less.");

            TypeName = typeName;
            OccurredOn = occurredOn;
            EventDescription = eventDescription;
            EventId = eventId;
            InStoreId = inStoreId;
            EventBody = eventBody;
            UserId = userId;
            ProcessId = processId;
        }

        public long InStoreId { get; set; }

        public string TypeName { get; private set; }

        public DateTime OccurredOn { get; private set; }

        public string EventDescription { get; private set; }

        public string EventBody { get; private set; }

        public int EventId { get; private set; }

        public int UserId { get; private set; }

        public Guid ProcessId { get; set; }

        public IDomainEvent ToDomainEvent()
        {
            return ToDomainEvent<IDomainEvent>();
        }

        public TEvent ToDomainEvent<TEvent>()
            where TEvent : IDomainEvent
        {
            Type eventType;

            try
            {
                eventType = Type.GetType(TypeName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format("Class load error, because: {0}", ex));
            }
            return (TEvent)EventSerializer.Instance.Deserialize(EventBody, eventType);
        }

        public static string ToStorePayload<TEvent>(TEvent domainEvent)
            where TEvent : IDomainEvent
        {
           return EventSerializer.Instance.Serialize(domainEvent);
        }

        public bool Equals(StoredEvent other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return EventId.Equals(other.EventId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StoredEvent);
        }

        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }
    }
}
