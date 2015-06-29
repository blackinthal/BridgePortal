namespace Domain.Contracts
{
    public interface ICommandHandler<in TCommand> where TCommand : CommandBase
    {
        void When(TCommand command);
    }
}
