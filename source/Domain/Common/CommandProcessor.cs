using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace Domain.Common
{
    public class CommandProcessor : ICommandProcessor
    {
        public bool HasErrors
        {
            get { return DomainEvents.Any() && DomainEvents.Any(a => a is IDomainEventError); }
        }

        public void Process<TCommand>(TCommand command) where TCommand : CommandBase
        {

            var aggregateService = ServiceLocator.Current.GetInstance<ICommandHandler<TCommand>>();

            if (aggregateService == null)
                throw new ArgumentException(typeof(TCommand).FullName);

            aggregateService.When(command);
        }

        public List<IDomainEvent> DomainEvents { get; private set; }
    }
}
