using System.Web.Mvc;
using Bridge.WebAPI.Modules;
using Domain.Contracts;

namespace Bridge.WebAPI.Controllers
{
    public class ImportEventController : Controller
    {
        private readonly ICommandProcessor _processor;
        private readonly ExtractEventMetadataModule _module;

        public ImportEventController(ICommandProcessor processor, ExtractEventMetadataModule module)
        {
            _processor = processor;
            _module = module;
        }


    }
}