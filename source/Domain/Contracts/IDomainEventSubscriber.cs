namespace Domain.Contracts
{
    public interface IDomainEventSubscriber <in T> where T : IDomainEvent 
    {
        void HandleEvent(T domainEvent);
    }
}
