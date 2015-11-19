using System;

using AutoMapper;

using JetBrains.Annotations;

namespace Tauco.Dicom.Shared.Converters
{
    /// <summary>
    /// Converts string to BirthNumber type instead of normal member mapping
    /// </summary>
    public class BirthNumberTypeConverter : ITypeConverter<string, BirthNumber>
    {
        private readonly IBirthNumberParser mBirthNumberParser;
        
        /// <summary>
        /// Instantiates new instance of <see cref="BirthNumberTypeConverter"/>
        /// </summary>
        /// <param name="birthNumberParser"></param>
        /// <exception cref="ArgumentNullException"><paramref name="birthNumberParser"/> is <see langword="null"/></exception>
        public BirthNumberTypeConverter([NotNull] IBirthNumberParser birthNumberParser)
        {
            if (birthNumberParser == null)
            {
                throw new ArgumentNullException(nameof(birthNumberParser));
            }

            mBirthNumberParser = birthNumberParser;
        }


        /// <summary>
        /// Performs conversion from string to BirthNumber type
        /// </summary>
        /// <param name="context">Resolution context</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/></exception>
        /// <returns>
        /// Crated BirthNumber.
        /// </returns>
        public BirthNumber Convert([NotNull] ResolutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return mBirthNumberParser.GetBirthNumber((string)context.SourceValue);
        }
    }
}
