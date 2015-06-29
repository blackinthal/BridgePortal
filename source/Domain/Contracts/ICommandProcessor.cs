using System.Collections.Generic;

namespace Domain.Contracts
{
    public interface ICommandProcessor
    {
        List<IDomainEvent> DomainEvents { get; }

        void Process<TCommand>(TCommand command) where TCommand : CommandBase;
    }
}
