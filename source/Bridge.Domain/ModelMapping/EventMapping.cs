using AutoMapper;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.EventAggregate.DomainEvents;
using Domain.Contracts;

namespace Bridge.Domain.ModelMapping
{
    public class EventMapping : IModelMap
    {
        public void Init()
        {
            Mapper.CreateMap<ImportEvent, ImportEventAttempted>();
        }
    }
}
