using System;
using System.Collections.Generic;
using System.Web.Http;
using Bridge.WebAPI.Models;
using Bridge.WebAPI.Modules;
using Bridge.WebAPI.Queries;
using Domain.Contracts;

namespace Bridge.WebAPI.Controllers
{
    [RoutePrefix("api/events")]
    public class EventController : ApiController
    {
        private readonly EventQueries _query;
        private readonly ICommandProcessor _processor;
        private readonly ExtractEventMetadataModule _eventModule;

        public EventController(EventQueries query, ICommandProcessor processor, ExtractEventMetadataModule eventModule)
        {
            _query = query;
            _processor = processor;
            _eventModule = eventModule;
        }

        //GET api/Event/year/month
        [Route("{year:int}/{month:int}")]
        public IEnumerable<EventModel> GetEvents(int year, int month)
        {
            return _query.GetEventsInMonth(year, month);
        }

        [Route("{year:int}/{month:int}/{day:int}")]
        public IHttpActionResult Post(int year, int month, int day)
        {
            var command = _eventModule.ExtractEventMetadata(new DateTime(year, month, day));

            _processor.Process(command);

            return Ok();
        }
    }
}
