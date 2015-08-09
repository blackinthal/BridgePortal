using System.Collections.Generic;
using System.Web.Http;
using Bridge.WebAPI.Models;
using Bridge.WebAPI.Queries;

namespace Bridge.WebAPI.Controllers
{
    [RoutePrefix("api/events")]
    public class EventController : ApiController
    {
        private readonly EventQueries _query;
        
        public EventController(EventQueries query)
        {
            _query = query;
        }
        //GET api/Event/year/month
        [Route("{year:int}/{month:int}")]
        public IEnumerable<EventModel> GetEvents(int year, int month)
        {
            return _query.GetEventsInMonth(year, month);
        } 
    }
}
