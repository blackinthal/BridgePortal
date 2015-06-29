using System;
using System.Collections.Generic;
using Domain.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace Domain.Common
{
    public class CommandProcessor : ICommandProcessor
    {
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
