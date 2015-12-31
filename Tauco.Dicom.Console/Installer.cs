using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// The contract to install components in the container.
    /// </summary>
    internal class Installer
    {
        /// <summary>
        /// Performs the installation in the <see cref="IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">Containser interface exposing all the functionality the Windsor implements</param>
        public void Install(IWindsorContainer container)
        {
            container.Register(Classes
             .FromAssemblyInThisApplication()
             .IncludeNonPublicTypes()
             .Pick()
             .WithService
             .DefaultInterfaces());
        }
    }
}