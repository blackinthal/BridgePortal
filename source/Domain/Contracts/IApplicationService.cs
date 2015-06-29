namespace Domain.Contracts
{
    public interface IApplicationService<T>
        where T: CommandBase
    {
        void When(T cmd);
        bool Handling { get; }
    }
}
