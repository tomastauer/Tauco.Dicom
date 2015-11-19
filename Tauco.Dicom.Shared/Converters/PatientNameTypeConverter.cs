using System;

using AutoMapper;

using JetBrains.Annotations;

namespace Tauco.Dicom.Shared.Converters
{
    /// <summary>
    /// Converts string to Patient name type instead of normal member mapping
    /// </summary>
    public class PatientNameTypeConverter : ITypeConverter<string, PatientName>
    {
        /// <summary>
        /// Performs conversion from string to Patient name.
        /// </summary>
        /// <param name="context">Resolution context</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/></exception>
        /// <returns>
        /// Crated Patient name.
        /// </returns>
        public PatientName Convert([NotNull] ResolutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new PatientName((string)context.SourceValue);
        }
    }
}
