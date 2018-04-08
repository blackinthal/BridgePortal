using System.Linq;
using System.Threading.Tasks;
using Bridge.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bridge.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Event/[action]")]
    public class EventController : Controller
    {
        private readonly BridgeContext _context;

        public EventController(BridgeContext context)
        {
            _context = context;
        }

        public async Task<dynamic> GetEvents()
        {
            return await _context.Event.ToListAsync();
        }
    }
}