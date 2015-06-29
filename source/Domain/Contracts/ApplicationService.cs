using System;
using System.Linq;
using Domain.Common;
using ElmahExtensions;
using Microsoft.Practices.ServiceLocation;

namespace Domain.Contracts
{
	public abstract class ApplicationService<TAggregate> where TAggregate : IAggregate
	{

	    protected void Apply(Action<TAggregate> action)
		{
			try
			{
			    var aggregate = ServiceLocator.Current.GetInstance<TAggregate>();

                action(aggregate);

                if (aggregate.DomainEvents != null && aggregate.DomainEvents.Any())
                        DomainEventPublisher.Instance.PublishAll(aggregate.DomainEvents);
			}
			catch (Exception ex)
			{
                CustomErrorSignal.Handle(ex);
			}

		}
	}
}