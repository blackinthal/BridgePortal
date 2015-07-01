using Bridge.Domain.EventAggregate.Commands;
using Domain.Contracts;

namespace Bridge.Domain.EventAggregate
{
    public class EventService : ApplicationService<EventAggregate>,
        ICommandHandler<ImportEvent>
    {
        public void When(ImportEvent command)
        {
            Apply(a => a.ImportEvent(command));
        }
    }
}
