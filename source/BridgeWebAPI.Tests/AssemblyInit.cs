using Bridge.Domain.Models;
using Bridge.WebAPI.Dependencies;
using Domain.EventStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void Startup(TestContext context)
        {
            new ComponentRegistrarBuilder()
                .Init()
                .RegisterDbContext<BridgeContext>("BridgeContext")
                .RegisterEventsStorage<SqlAppendOnlyStore>()
                .RegisterDomainComponents()
                .RegisterAutoMapper()
                .RegisterModules()
                .RegisterQueries()
                .Build();
        }
    }
}
