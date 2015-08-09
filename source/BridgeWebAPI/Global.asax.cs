using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Bridge.Domain.Models;
using Bridge.WebAPI.Dependencies;
using Domain.EventStorage;

namespace Bridge.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureWindsor(GlobalConfiguration.Configuration);
            
        }

        private void ConfigureWindsor(HttpConfiguration configuration)
        {
            new ComponentRegistrarBuilder()
                .Init()
                .RegisterDbContext<BridgeContext>("BridgeContext")
                .RegisterEventsStorage<SqlAppendOnlyStore>()
                .RegisterDomainComponents()
                .RegisterAutoMapper()
                .RegisterModules()
                .RegisterQueries()
                .RegisterControllers()
                .RegisterWindsorAsControllerFactory(configuration)
                .Build();
        }
    }
}
