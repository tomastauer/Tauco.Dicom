using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.MicroKernel.Proxy;

using JetBrains.Annotations;

namespace Tauco.Dicom.Utilities
{
    /// <summary>
    /// Selects <see cref="LoggingAspect"/> for the interceptor.
    /// </summary>
    public class InterceptorSelector : IModelInterceptorsSelector
    {
        /// <summary>
        /// Determine whether the specified has interceptors. 
        /// </summary>
        /// <param name="model">The model</param>
        /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/></exception>
        /// <returns>
        /// False, if model is of type <see cref="LoggingAspect"/>; otherwise, true.
        /// </returns>
        public bool HasInterceptors([NotNull] ComponentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return typeof(LoggingAspect) != model.Implementation;
        }


        /// <summary>
        /// Select <see cref="LoggingAspect"/> as the interceptor.
        /// </summary>
        /// <param name="model">The model to select the interceptors for</param>
        /// <param name="interceptors">The interceptors selected by previous selectors in the pipeline or <see cref="P:Castle.Core.ComponentModel.Interceptors"/> if this is the first interceptor in the pipeline.</param>
        /// <returns>
        /// The interceptor for this model (in the current context) or a null reference
        /// </returns>
        public InterceptorReference[] SelectInterceptors(ComponentModel model, InterceptorReference[] interceptors)
        {
            var result = new List<InterceptorReference>(interceptors)
            {
                InterceptorReference.ForType<LoggingAspect>()
            };

            return result.ToArray();
        }
    }
}