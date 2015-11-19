using System;
using System.ComponentModel;

using Newtonsoft.Json;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents Czech birth number.
    /// </summary>
    [TypeConverter(typeof(BirthNumberTypeConverter))]
    [JsonConverter(typeof(BirthNumberJsonConverter))]
    public class BirthNumber : InfoIdentifier
    {
        /// <summary>
        /// Instantiate new instance of <see cref="BirthNumber"/>.
        /// </summary>
        /// <param name="identifier">String representation of birth number</param>
        /// <remarks>
        /// This class should be instantiated only by using <see cref="BirthNumberParser"/> to ensure <paramref name="identifier"/>
        /// is in valid format.
        /// </remarks>
        internal BirthNumber(string identifier) : base(identifier)
        {
        }


        /// <summary>
        /// Patient's birth date. Is computed from the birth number.
        /// </summary>
        public DateTime BirthDate
        {
            get;
            set;
        }


        /// <summary>
        /// Patient's Gender. Is computed from the month component of the birth number.
        /// </summary>
        public Gender Gender
        {
            get;
            set;
        }


        /// <summary>
        /// Specifies first three digits from the birth number suffix (part following the slash).
        /// </summary>
        /// <remarks>
        /// Since type of this property is short, leading zeros has to be considered when converting to string.
        /// </remarks>
        public short Suffix
        {
            get;
            set;
        }


        /// <summary>
        /// Last (tenth) digit of the birth number, computed to satisfy divisibility rules for the number. In birth numbers issued before 1954 can be missing.
        /// </summary>
        public byte? CheckDigit
        {
            get;
            set;
        }


        /// <summary>
        /// Gets string representation of <see cref="BirthNumber"/> without slash, eg. 9107256444.
        /// </summary>
        public string StringRepresentationWithoutSlash => StringRepresentation.Replace("/", "");


        /// <summary>
        /// Gets string representation of <see cref="BirthNumber"/> with slash, eg. 910725/6444.
        /// </summary>
        public string StringRepresentationWithSlash => StringRepresentationWithoutSlash.Insert(6, "/");


        /// <summary>
        /// Returns the hash code for the birth number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return StringRepresentationWithoutSlash.GetHashCode();
        }


        /// <summary>
        /// Compares with another birth number and returns whether their string representations without slash equals.
        /// </summary>
        /// <param name="other">Other birth number to be checked</param>
        /// <returns>True, if birth numbers' string representations without slash are equal; otherwise, false</returns>
        protected bool Equals(BirthNumber other)
        {
            return other.StringRepresentationWithoutSlash == StringRepresentationWithoutSlash;
        }


        /// <summary>
        /// Compares with another birth number and returns whether their string representations without slash equals.
        /// </summary>
        /// <param name="obj">Other birth number to be checked</param>
        /// <returns>True, if birth numbers' string representations without slash are equal; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is string)
            {
                string objString = (string) obj;
                return StringRepresentationWithoutSlash == objString || StringRepresentationWithSlash == objString;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((BirthNumber) obj);
        }


        /// <summary>
        /// Compares with another object and returns whether string representation of <paramref name="obj"/> is contained within <see cref="StringRepresentationWithoutSlash"/> or <see cref="StringRepresentationWithSlash"/>.
        /// </summary>
        /// <param name="obj">Other object to be checked</param>
        /// <returns>True, if object is contained; otherwise, false</returns>
        public bool Contains(object obj)
        {
            if (obj is string)
            {
                string objString = (string)obj;
                return StringRepresentationWithoutSlash.Contains(objString) || StringRepresentationWithSlash.Contains(objString);
            }

            return false;
        }
    }
}
