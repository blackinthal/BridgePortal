using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using ElmahExtensions;
using Microsoft.Practices.ServiceLocation;

namespace Domain.Common
{
    public class EventedCommandProcessor : ICommandProcessor
    {
        public EventedCommandProcessor()
        {
            DomainEvents = new List<IDomainEvent>();
        }

        public List<IDomainEvent> DomainEvents { get; private set; }

        public bool HasErrors
        {
            get { return DomainEvents.Any() && DomainEvents.Any(a => a is IDomainEventError); }
        }

        public void Process<TCommand>(TCommand command) where TCommand : CommandBase
        {
            var subscriber = new DomainEventSubscriberForProcess(command.ProcessId);
            DomainEventPublisher.Instance.Subscribe(subscriber);

            try
            {
                var aggregateService = ServiceLocator.Current.GetInstance<ICommandHandler<TCommand>>();

                if (aggregateService == null)
                    throw new ArgumentException(typeof (TCommand).FullName);

                aggregateService.When(command);
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
            }
            finally
            {
                DomainEventPublisher.Instance.UnSubscribe(subscriber);
            }

            DomainEvents.AddRange(subscriber.DomainEvents);

        }
    }
}