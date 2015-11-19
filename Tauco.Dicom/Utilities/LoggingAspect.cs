using System;

using Castle.Core.Logging;
using Castle.DynamicProxy;

using JetBrains.Annotations;

namespace Tauco.Dicom.Utilities
{
    /// <summary>
    /// Specifies logging interceptor. 
    /// </summary>
    public class LoggingAspect : IInterceptor
    {
        private readonly ILogger mLogger;


        /// <summary>
        /// Instantiates new instance of <see cref="LoggingAspect"/>.
        /// </summary>
        /// <param name="logger">Logging provider</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/></exception>
        public LoggingAspect([NotNull] ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            mLogger = logger;
        }


        /// <summary>
        /// Add logging routine to all invocations within the interceptor.
        /// </summary>
        /// <param name="invocation">Encapsulates an invocation of a proxied method</param>
        public void Intercept(IInvocation invocation)
        {
            try
            {
                mLogger.Debug($"Executing method {invocation.Method.Name}.");
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                mLogger.Error("Exception thrown", ex);
            }
            finally
            {
                mLogger.Debug($"Executing method {invocation.Method.Name} finished.");
            }
        }
    }
}
