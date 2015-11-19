using System;
using System.Globalization;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Service for parsing czech birth numbers.
    /// </summary>
    internal class BirthNumberParser : IBirthNumberParser
    {
        private const string BASIC_VALIDITY_PATTERN = @"^[\d]{9,10}$";
        private const string PARSING_PATTERN = @"^(?<year>\d{2})(?<month>\d{2})(?<day>\d{2})(?<last3>\d{3})(?<last>\d)?$";


        /// <summary>
        /// Parses given <paramref name="birthNumber"/> and returns new instance of <see cref="BirthNumber"/> with the values obtained from the birth number.
        /// </summary>
        /// <param name="birthNumber">Birth number to be parsed</param>
        /// <exception cref="ArgumentNullException"><paramref name="birthNumber"/> is <see langword="null"/></exception>
        /// <returns>
        /// Instance of <see cref="BirthNumber"/> containing data obtained from <paramref name="birthNumber"/>, if <paramref name="birthNumber"/> is in valid format; otherwise, retuns null
        /// </returns>
        public BirthNumber GetBirthNumber([NotNull] string birthNumber)
        {
            if (birthNumber == null)
            {
                throw new ArgumentNullException(nameof(birthNumber));
            }

            if (IsValid(birthNumber))
            {
                return ParseBirthNumber(birthNumber);
            }

            return null;
        }


        /// <summary>
        /// Checks whether the given <paramref name="birthNumber" /> has correct length, consists only from digits and matches
        /// divisibility rules specified in the ISVS catalog.
        /// </summary>
        /// <param name="birthNumber">Birth number to be verified</param>
        /// <returns>True, is <paramref name="birthNumber"/> consists only from digits and satisfies divisibility restrictions specified in ISVS catalog</returns>
        private bool IsValid(string birthNumber)
        {
            birthNumber = birthNumber.Replace("/", string.Empty);

            if (!Regex.IsMatch(birthNumber, BASIC_VALIDITY_PATTERN))
            {
                return false;
            }

            if (birthNumber.Length == 9)
            {
                return true;
            }

            var firstNineDigits = birthNumber.Substring(0, 9);
            var remainder = (byte)(int.Parse(firstNineDigits) % 11);
            byte lastDigit = remainder == 10 ? (byte)0 : remainder;

            if (!birthNumber.EndsWith(lastDigit.ToString(CultureInfo.InvariantCulture)))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Parses given <paramref name="birthNumber"/> and returns new instance of <see cref="BirthNumber"/> with the values obtained from the birth number.
        /// </summary>
        /// <param name="birthNumber">Birth number to be parsed</param>
        /// <returns>
        /// Instance of <see cref="BirthNumber"/> containing data obtained from <paramref name="birthNumber"/>, if <paramref name="birthNumber"/> is in valid format; otherwise, retuns null
        /// </returns>
        private BirthNumber ParseBirthNumber(string birthNumber)
        {
            string birthNumberWithoutSlash = birthNumber.Replace("/", string.Empty);
            var match = Regex.Match(birthNumberWithoutSlash, PARSING_PATTERN);

            short year = byte.Parse(match.Groups["year"].Value);
            byte month = byte.Parse(match.Groups["month"].Value);
            byte day = byte.Parse(match.Groups["day"].Value);
            short suffix = short.Parse(match.Groups["last3"].Value);
            byte checkDigit;
            bool hasCheckDigit;

            if (byte.TryParse(match.Groups["last"].Value, out checkDigit))
            {
                year += year < 54 ? (short)2000 : (short)1900;
                hasCheckDigit = true;
            }
            else
            {
                year += year < 54 ? (short)1900 : (short)1800;
                hasCheckDigit = false;
            }

            var gender = month > 50 ? Gender.Female : Gender.Male;

            month %= 50;
            // Since 2004 when there is not enough of unique birth numbers available for some day, it is permitted to add 20 to the month to create more combinations
            // However, before 2004 such changes would be invalid.
            if (month > 20 && year < 2004)
            {
                return null;
            }
            month %= 20;

            try
            {
                var dateTime = new DateTime(year, month, day);
                return new BirthNumber(birthNumber)
                {
                    Gender = gender,
                    BirthDate = dateTime,
                    CheckDigit = !hasCheckDigit ? (byte?)null : checkDigit,
                    Suffix = suffix
                };
            }
            // Invalid datetime parameters => birth number is invalid format
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}
