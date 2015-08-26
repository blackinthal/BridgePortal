using System.Linq;
using AutoMapper.QueryableExtensions;
using Bridge.Domain.Models;
using Bridge.WebAPI.Models;

namespace Bridge.WebAPI.Queries
{
    public class DealQueries
    {
        private readonly BridgeContext _context;

        public DealQueries(BridgeContext context)
        {
            _context = context;
        }

        public DealDetailModel Get(int id)
        {
            return _context.Deals
                .Where(w => w.Id == id)
                .Project().To<DealDetailModel>()
                .FirstOrDefault();
        }
    }
}