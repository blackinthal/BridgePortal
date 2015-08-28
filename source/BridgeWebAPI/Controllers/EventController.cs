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
        /// <summary>
        /// Returns a list of all imported events
        /// </summary>
        /// <returns>list of all imported events</returns>
        [Route("")]
        public IEnumerable<EventModel> Get()
        {
            return _queries.GetEvents();
        }
        /// <summary>
        /// Returns metadata about an imported event
        /// </summary>
        /// <param name="id"></param>
        /// <returns>metadata about an imported event</returns>
        [Route("{id:int}")]
        public EventDetailModel Get(int id)
        {
            return _queries.GetEvent(id);
        }
    }
}
