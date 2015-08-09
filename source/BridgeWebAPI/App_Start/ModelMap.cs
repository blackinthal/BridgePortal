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
            Mapper.CreateMap<Event, EventModel>();
        }
    }
}