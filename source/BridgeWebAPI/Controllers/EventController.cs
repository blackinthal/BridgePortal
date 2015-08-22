using System.Collections.Generic;
using System.Web.Http;
using Bridge.WebAPI.Models;
using Bridge.WebAPI.Queries;

namespace Bridge.WebAPI.Controllers
{
    [RoutePrefix("api/events")]
    public class EventController : ApiController
    {
        private readonly EventQueries _queries;
        
        public EventController(EventQueries queries)
        {
            _queries = queries;
        }
        [Route("")]
        public IEnumerable<EventModel> Get()
        {
            return _queries.GetEvents();
        }
        [Route("{id:int}")]
        public EventDetailModel Get(int id)
        {
            return _queries.GetEvent(id);
        }
    }
}
