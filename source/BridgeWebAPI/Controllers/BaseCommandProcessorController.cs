using System.Linq;
using System.Text;
using System.Web.Http;
using Domain.Contracts;
using Elmah;

namespace Bridge.WebAPI.Controllers
{
    public class BaseCommandProcessorController : ApiController
    {
        private readonly ICommandProcessor _processor;

        public BaseCommandProcessorController(ICommandProcessor processor)
        {
            _processor = processor;
        }

        protected IHttpActionResult ProcessCommand<T>(T command) where T : CommandBase
        {
            _processor.Process(command);

            if (_processor.HasErrors)
            {
                return InternalServerError(new ApplicationException(BuildErrorMessage()));
            }

            return Ok();   
        }

        private string BuildErrorMessage()
        {
            var sb = new StringBuilder();
            var errors = _processor.DomainEvents.Where(a => a is IDomainEventError).ToList();

            errors.ForEach(error =>
            {
                var er = error as IDomainEventError;
                if (er != null)
                {
                    sb.Append(er.Errors).Append("</br>");
                }
            });

            return sb.ToString();
        }
    }
}