using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BridgeWebAPI.Dependencies
{
    public static class WindsorExtensions
    {
        public static BasedOnDescriptor FirstNonGenericCoreInterface(this ServiceDescriptor descriptor,
            string interfaceNamespace)
        {
            return descriptor.Select((type, baseType) =>
            {
                IEnumerable<Type> types = type.GetInterfaces().Where(t => !t.IsGenericType && t.Namespace.StartsWith(interfaceNamespace));
                if (!types.Any())
                    return (IEnumerable<Type>)null;
                return (IEnumerable<Type>)new[]
                {
                    types.ElementAt(0)
                };
            });
        }
    }

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _container = container;
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            _container.Release(controller);
        }

        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            if (controllerType == null)
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", context.HttpContext.Request.Path));
            return (IController)_container.Resolve(controllerType);
        }
    }

    public class ControllerExtensions
    {
        public static bool IsController(Type type)
        {
            if (type != null && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) && !type.IsAbstract)
                return typeof(IController).IsAssignableFrom(type);
            return false;
        }
    }
}