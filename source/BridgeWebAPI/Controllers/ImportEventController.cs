using System.Web.Mvc;
using BridgeWebAPI.Modules;
using Domain.Contracts;

namespace BridgeWebAPI.Controllers
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