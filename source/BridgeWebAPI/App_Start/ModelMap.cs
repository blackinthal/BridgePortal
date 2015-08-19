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
            Mapper.CreateMap<Event, EventModel>()
                  .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Date.Year))
                  .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Date.Month))
                  .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Date.Day))
                  ;
        }
    }
}