using System;

namespace Domain.Contracts
{
    public interface IDomainEvent
    {
        int EventId { get; set;  }
        DateTime OccurredOn { get; set; }
        int UserId { get; set; }
        Guid ProcessId { get; set; }
    }

    public interface IDomainEventError : IDomainEvent
    {
        string Errors { get; set;  }
		Guid ErrorId { get; set; }
    }
}
