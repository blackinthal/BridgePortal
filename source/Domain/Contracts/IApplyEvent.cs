namespace Domain.Contracts
{
    public interface IApplyEvent<in TEvent> where TEvent: IDomainEvent
    {
        void ApplyEvent(TEvent e);
    }
}
