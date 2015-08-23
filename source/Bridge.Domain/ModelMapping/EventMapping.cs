using AutoMapper;
using Bridge.Domain.EventAggregate.Commands;
using Bridge.Domain.EventAggregate.DomainEvents;
using Bridge.Domain.Models;
using Domain.Contracts;

namespace Bridge.Domain.ModelMapping
{
    public class EventMapping : IModelMap
    {
        public void Init()
        {
            Mapper.CreateMap<ImportEvent, ImportEventAttempted>();
            Mapper.CreateMap<ImportEvent, Event>()
                  .ForMember(dest => dest.Pairs, opt => opt.Ignore())
                  .ForMember(dest => dest.Deals, opt => opt.Ignore()); ;
            Mapper.CreateMap<PairMetadata, Pair>();
            Mapper.CreateMap<DealMetadata, Deal>();
            Mapper.CreateMap<DuplicateDealMetadata, DuplicateDeal>();
            Mapper.CreateMap<MakeableContractMetadata, MakeableContract>();
        }
    }
}
