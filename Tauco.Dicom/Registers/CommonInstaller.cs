using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

using AutoMapper;
using AutoMapper.Mappers;

using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Converters;
using Tauco.Dicom.Utilities;

namespace Tauco.Dicom
{
    /// <summary>
    /// The contract to install components in the container.
    /// </summary>
    public class CommonInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements</param>
        /// <param name="store">The configuration store</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterCurrent(container);
            RegisterCache(container);
            RegisterShared(container);
            RegisterFellowOak(container);
            RegisterAutoMapper(container);
            RegisterLogger(container);
            RegisterFileSystem(container);
            RegisterDcmdir2dcm(container);
        }


        /// <summary>
        /// Registers service for the dcmdir2dcm library.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private static void RegisterDcmdir2dcm(IWindsorContainer container)
        {
            container.Register(Classes
              .FromAssemblyNamed("dcmdir2dcm.Lib")
              .Pick()
              .WithService
              .DefaultInterfaces());
        }


        /// <summary>
        /// Registers service for the abstract file system.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private static void RegisterFileSystem(IWindsorContainer container)
        {
            container.Register(Component.For<IFileSystem>().UsingFactoryMethod(() => new FileSystem()));
        }


        /// <summary>
        /// Registers services for the logging interceptor.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private static void RegisterLogger(IWindsorContainer container)
        {
            container.Kernel.ProxyFactory.AddInterceptorSelector(new InterceptorSelector());
            container
                .AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog).WithConfig("NLog.config"));
        }


        /// <summary>
        /// Registers services for the Automapper.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private static void RegisterAutoMapper(IWindsorContainer container)
        {
            container.Register(
                Component.For<IEnumerable<IObjectMapper>>().UsingFactoryMethod(() => MapperRegistry.Mappers),
                Component.For<ConfigurationStore>()
                    .LifestyleSingleton()
                    .UsingFactoryMethod(x =>
                    {
                        var typeMapFactory = x.Resolve<ITypeMapFactory>();
                        var mappers = x.Resolve<IEnumerable<IObjectMapper>>();
                        ConfigurationStore configurationStore = new ConfigurationStore(typeMapFactory, mappers);
                        configurationStore.ConstructServicesUsing(x.Resolve);
                        configurationStore.AssertConfigurationIsValid();
                        configurationStore.CreateMap<string, InfoIdentifier>().ConvertUsing<InfoIdentifierTypeConverter>();
                        configurationStore.CreateMap<string, BirthNumber>().ConvertUsing<BirthNumberTypeConverter>();
                        configurationStore.CreateMap<string, PatientName>().ConvertUsing<PatientNameTypeConverter>();
                        configurationStore.CreateMap<string, string>().ConvertUsing(c => c);
                        return configurationStore;
                    }),
                Component.For<IConfigurationProvider>().UsingFactoryMethod(x => x.Resolve<ConfigurationStore>()),
                Component.For<IConfiguration>().UsingFactoryMethod(x => x.Resolve<ConfigurationStore>()),
                Component.For<IMappingEngine>().ImplementedBy<MappingEngine>().LifestyleSingleton(),
                Component.For<ITypeMapFactory>().ImplementedBy<TypeMapFactory>()
            );

            // Add all Profiles
            var configuration = container.Resolve<IConfiguration>();
            container.ResolveAll<Profile>().ToList().ForEach(configuration.AddProfile);
        }


        /// <summary>
        /// Registers services from Tauco.Dicom assembly.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private void RegisterCurrent(IWindsorContainer container)
        {
            container.Register(Classes
               .FromAssemblyInThisApplication()
               .IncludeNonPublicTypes()
               .Pick()
               .WithService
               .DefaultInterfaces());
        }


        /// <summary>
        /// Registers services from Tauco.Cache assembly.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private void RegisterCache(IWindsorContainer container)
        {
            container.Register(Classes
                .FromAssemblyNamed("Tauco.Cache")
                .Pick()
                .WithService
                .DefaultInterfaces());
        }


        /// <summary>
        /// Registers services from Tauco.Dicom.Shared assembly.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private void RegisterShared(IWindsorContainer container)
        {
            container.Register(Classes
                 .FromAssemblyNamed("Tauco.Dicom.Shared")
                 .IncludeNonPublicTypes()
                 .Pick()
                 .WithService
                 .DefaultInterfaces());
        }


        /// <summary>
        /// Registers services from Tauco.Dicom.Abstraction.FellowOak assembly.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements.</param>
        private void RegisterFellowOak(IWindsorContainer container)
        {
            container.Register(Classes
                 .FromAssemblyNamed("Tauco.Dicom.Abstraction.FellowOak")
                 .IncludeNonPublicTypes()
                 .Pick()
                 .WithService
                 .DefaultInterfaces());
        }

    }
}
