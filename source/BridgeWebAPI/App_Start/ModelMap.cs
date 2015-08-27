using System.Linq;
using AutoMapper;
using Bridge.Domain.Models;
using Bridge.WebAPI.Models;
using Domain.Contracts;

namespace Bridge.WebAPI.App_Start
{
    public class ModelMap : IModelMap
    {
        public void Init()
        {
            Mapper.CreateMap<Event, ImportedEventModel>()
                  .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Date.Year))
                  .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Date.Month))
                  .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Date.Day));

            Mapper.CreateMap<Event, EventModel>();

            Mapper.CreateMap<Event, EventDetailModel>()
                .ForMember(dest => dest.Pairs, opt => opt.MapFrom(src => src.Pairs.OrderBy(o => o.Rank)))
                .ForMember(dest => dest.Deals, opt => opt.MapFrom(src => src.Deals.OrderBy(o => o.Index)));

            Mapper.CreateMap<Deal, DealModel>();
            Mapper.CreateMap<Pair, PairModel>();

            Mapper.CreateMap<Deal, DealDetailModel>()
                  .ForMember(dest => dest.DealResults, opt => opt.MapFrom(src => src.DuplicateDeals.OrderBy(o => o.EWPercentage)))
                  .ForMember(dest => dest.MakeableContracts, opt => opt.MapFrom(src => src.MakeableContracts
                      .OrderBy(o => o.Denomination).ThenBy(o => o.Declarer)));

            Mapper.CreateMap<DuplicateDeal,DealResultViewModel>();
            Mapper.CreateMap<MakeableContract, MakeableContractViewModel>();
        }
    }
}