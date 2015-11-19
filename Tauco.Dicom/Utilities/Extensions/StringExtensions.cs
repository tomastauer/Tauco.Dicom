using System;
using System.Globalization;

using JetBrains.Annotations;

namespace Tauco.Dicom.Utilities.Extensions
{
    /// <summary>
    /// Provides method extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the specified <see cref="T:System.String"/> object occurs within this string. Ignores case with respect to given culture.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <paramref name="value"/> parameter occurs within this string, or if <paramref name="value"/> is the empty string (""); otherwise, false.
        /// </returns>
        /// <param name="current">Current string to be checked. </param>
        /// <param name="value">The string to seek. </param>
        /// <param name="culture">Culture the check is made with respect to</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/> -or- <paramref name="culture"/> is <see langword="null"/></exception>
        public static bool Contains(this string current, [NotNull] string value, [NotNull] CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            return culture.CompareInfo.IndexOf(current, value, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
