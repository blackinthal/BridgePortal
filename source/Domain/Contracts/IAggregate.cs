using System.Collections.Generic;

namespace Domain.Contracts
{
	public interface IAggregate //: IDisposable
	{
		List<IDomainEvent> DomainEvents { get; }
	}
}