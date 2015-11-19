using System;

using AutoMapper;

using JetBrains.Annotations;

namespace Tauco.Dicom.Shared.Converters
{
    /// <summary>
    /// Converts string to InfoIdentifier type instead of normal member mapping
    /// </summary>
    public class InfoIdentifierTypeConverter : ITypeConverter<string, InfoIdentifier>
    {
        /// <summary>
        /// Performs conversion from string to InfoIdentifier type.
        /// </summary>
        /// <param name="context">Resolution context</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/></exception>
        /// <returns>
        /// Crated InfoIdentifier.
        /// </returns>
        public InfoIdentifier Convert([NotNull] ResolutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new InfoIdentifier((string)context.SourceValue);
        }
    }
}