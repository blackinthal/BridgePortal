using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Bridge.WebAPI.Models;
using Bridge.WebAPI.Modules;
using Bridge.WebAPI.Queries;
using Domain.Contracts;
using ElmahExtensions;

namespace Bridge.WebAPI.Controllers
{
    [RoutePrefix("api/importevents")]
    public class ImportEventController : BaseCommandProcessorController
    {
        private readonly EventQueries _query;
        private readonly ExtractEventMetadataModule _eventModule;

        public ImportEventController(EventQueries query, ICommandProcessor processor, ExtractEventMetadataModule eventModule)
            : base(processor)
        {
            _query = query;
            _eventModule = eventModule;
        }

        //GET api/Event/year/month
        [Route("{year:int}/{month:int}")]
        public IEnumerable<ImportedEventModel> GetEvents(int year, int month)
        {
            return _query.GetEventsInMonth(year, month);
        }

        [Route("{year:int}/{month:int}/{day:int}")]
        public IHttpActionResult Post(int year, int month, int day)
        {
            try
            {
                var command = _eventModule.ExtractEventMetadata(new DateTime(year, month, day));

                return ProcessCommand(command);
            }
            catch (WebException webEx)
            {
                return InternalServerError(new ApplicationException("The event was not found"));
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
                return InternalServerError();
            }
        }
    }
}