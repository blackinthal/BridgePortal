using System;
using System.ComponentModel;
using System.Linq;

namespace BridgeWebAPI.Dependencies
{
    public class ComponentRegistrarBuilder
    {

        private static IWindsorContainer _container;


        public ComponentRegistrarBuilder Init(bool mock = false)
        {
            _container = new WindsorContainer();
            return this;
        }

        public void Build()
        {
            var windsorServiceLocator = new WindsorServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }

        public ComponentRegistrarBuilder RegisterDomainComponents()
        {
            AddListners();
            AddHandlers();
            AddAggregates();

            return this;
        }

        private static void AddAggregates()
        {
            _container.Register(
               Types.FromAssemblyInThisApplication()
                  .BasedOn<IAggregate>()
                   .LifestyleTransient());
        }

        private static void AddListners()
        {
            _container.Register(
                Classes.FromAssemblyInThisApplication()
                   .BasedOn(typeof(IApplyEvent<>))
                    .WithService.AllInterfaces().LifestyleTransient().Unless(u => u.FullName.Contains("Ports") || u.FullName.Contains("Cache")));
        }

        private static void AddHandlers()
        {
            _container.Register(
                Types.FromAssemblyInThisApplication()
                    .BasedOn(typeof(ICommandHandler<>))
                    .WithService.AllInterfaces().LifestyleTransient());


            _container.Register(
                Component.For(typeof(ICommandProcessor))
                    .ImplementedBy(typeof(EventedCommandProcessor))
                    .Named("commandProcessor").LifestyleTransient());
        }

        public ComponentRegistrarBuilder RegisterDbContext<TDbContext>(string connectionString = null)
    where TDbContext : DbContext
        {
            _container.Register(
                Component.For<TDbContext>().Named(typeof(TDbContext).Name)
                         .DependsOn(Property.ForKey("connectionString").Eq(connectionString ?? typeof(TDbContext).Name)).IsDefault()
                         .LifeStyle.Transient);

            return this;
        }


        public ComponentRegistrarBuilder RegisterEventsStorage<TAppendOnlyStore>(string connectionName = "EventStore")
    where TAppendOnlyStore : class, IAppendOnlyStore
        {
            if (!string.IsNullOrWhiteSpace(connectionName))
            {
                var connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

                _container.Register(
                    Component.For<IAppendOnlyStore>()
                             .ImplementedBy(typeof(TAppendOnlyStore))
                             .Named(typeof(TAppendOnlyStore).Name)
                             .DependsOn(
                                 Property.ForKey("connectionString")
                                         .Eq(connectionString ?? typeof(TAppendOnlyStore).Name)).IsDefault()
                             .LifeStyle.Transient);
                _container.Register(
             Component.For(typeof(IEventStore))
                 .ImplementedBy(typeof(EventStore))
                 .Named("eventStore").LifestyleTransient());
            }
            else
            {
                _container.Register(
                    Component.For<IAppendOnlyStore>()
                             .ImplementedBy(typeof(TAppendOnlyStore))
                             .Named(typeof(TAppendOnlyStore).Name)
                             .LifeStyle.Singleton);
                _container.Register(
             Component.For(typeof(IEventStore))
                 .ImplementedBy(typeof(EventStore))
                 .Named("eventStore").LifestyleTransient());
            }


            EventStore = _container.Resolve<IEventStore>();

            DomainEventPublisher.Instance.Subscribe(domainEvent => EventStore.Append(domainEvent));

            return this;
        }

        public ComponentRegistrarBuilder RegisterQueries()
        {
            _container.Register(
                Classes.FromThisAssembly().Pick().If(a => a.Name.EndsWith("Queries")).LifestyleTransient());
            return this;
        }

        public ComponentRegistrarBuilder RegisterModules(string connectionName = null)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            _container.Register(
                Component.For(typeof(ExportModule)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(ImportModule)).LifeStyle.Transient);

            _container.Register(
               Component.For(typeof(ImportRequestValidator)).LifeStyle.Transient);

            _container.Register(
               Component.For(typeof(ImportedInvalidationReasonBuilder)).LifeStyle.Transient);

            _container.Register(
               Component.For(typeof(CorrectRequestDRPCIVValidator)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(RegisterRequestValidator)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(RequestInfoModelValidator)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(UserValidator)).LifeStyle.Transient);

            _container.Register(
                 Component.For(typeof(ExcelFactoryModule)).LifeStyle.Transient);

            _container.Register(
                 Component.For(typeof(InvoiceImportModule)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(ValidatedVehiclesImportModule)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(ValidatedVehiclesValidator)).LifeStyle.Transient);

            _container.Register(
                 Component.For(typeof(DownloadReportModule)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(ApproveInvoiceValidator)).LifeStyle.Transient);

            _container.Register(
                Component.For(typeof(InvoiceValidator)).LifeStyle.Transient);

            _container.Register(
               Component.For(typeof(ApplicationKeys)).DependsOn(Property.ForKey("connectionString")
                                        .Eq(connectionString)).IsDefault().LifeStyle.Singleton);

            return this;
        }

        public IEventStore EventStore { get; private set; }

        public ComponentRegistrarBuilder RegisterAutoMapper()
        {
            _container.Register(
                Types.FromAssemblyNamed("DRPCIV.SupportingDomain")
                .BasedOn(typeof(IModelMap))
                .WithService.AllInterfaces().LifestyleTransient()
            );

            _container.Register(
                Types.FromAssemblyNamed("AFM.Domain.AFM")
                .BasedOn(typeof(IModelMap))
                .WithService.AllInterfaces().LifestyleTransient()
            );

            _container.Register(
                Types.FromThisAssembly()
                .BasedOn(typeof(IModelMap))
                .WithService.AllInterfaces().LifestyleTransient()
            );

            var all = _container.ResolveAll<IModelMap>().ToList();
            all.ForEach(mm => mm.Init());
            return this;
        }

        public ComponentRegistrarBuilder RegisterControllers(Assembly fromAssembly = null)
        {
            var assembly = fromAssembly ?? Assembly.GetCallingAssembly();

            RegisterControllers(assembly.GetExportedTypes());
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            return this;
        }

        public ComponentRegistrarBuilder RegisterTestControllers(Action<RouteCollection> registerRoutes = null, Assembly fromAssembly = null)
        {
            var routes = new RouteCollection();
            if (registerRoutes != null)
            {
                registerRoutes(routes);
            }
            else
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                routes.MapRoute("Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
            }
            var assembly = fromAssembly ?? Assembly.GetCallingAssembly();

            RegisterControllers(assembly.GetExportedTypes());
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            return this;
        }

        public ComponentRegistrarBuilder RegisterUserManager()
        {
            _container.Register(
                Component.For<IUserManager>().ImplementedBy<CustomUserManager>().LifestyleTransient()
            );

            _container.Register(
                Component.For<IUserStore>().ImplementedBy<CustomUserStore>().LifestyleTransient()
            );
            return this;
        }

        public ComponentRegistrarBuilder RegisterCaches()
        {
            _container.Register(
                Component.For<AuthenticationCache>().Forward<IApplyEvent<UserLoggedIn>>()
                    .ImplementedBy<AuthenticationCache>().LifestyleSingleton()
                );

            return this;
        }

        private void RegisterControllers(params Type[] controllerTypes)
        {
            foreach (var type in controllerTypes.Where(ControllerExtensions.IsController).Where(type => true))
            {
                _container.Register(new IRegistration[]
                {
                    Component.For(type).Named(type.FullName.ToLower()).LifeStyle.Is(LifestyleType.Transient)
                });
            }
        }
    }
}