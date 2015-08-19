using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Bridge.Domain.Models;
using Bridge.WebAPI.Dependencies;
using Domain.EventStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bridge.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

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
