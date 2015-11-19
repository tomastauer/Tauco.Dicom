using System;
using System.ComponentModel;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Universal info identifier.
    /// </summary>
    [TypeConverter(typeof(InfoIdentifierTypeConverter))]
    [JsonConverter(typeof(InfoIdentifierJsonConverter))]
    public class InfoIdentifier
    {
        /// <summary>
        /// Constructor. Creates identifier from given string.
        /// </summary>
        /// <param name="identifier">String identifier</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/></exception>
        public InfoIdentifier([NotNull] string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            StringRepresentation = identifier;
        }


        /// <summary>
        /// Implicitly convert <see cref="string"/> into <see cref="InfoIdentifier"/>.
        /// </summary>
        /// <param name="id">String to be converted</param>
        public static implicit operator InfoIdentifier(string id)
        {
            return new InfoIdentifier(id);
        }


        /// <summary>
        /// String representation of the identifier.
        /// </summary>
        public string StringRepresentation
        {
            get;
        }

    
        /// <summary>
        /// Implicitly convert <see cref="InfoIdentifier"/> into <see cref="string"/>.
        /// </summary>
        /// <param name="infoIdentifier">Info identifier to be converted</param>
        public static implicit operator string(InfoIdentifier infoIdentifier)
        {
            return infoIdentifier.StringRepresentation;
        }


        /// <summary>
        /// Returns the hash code for the identifier.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return StringRepresentation?.GetHashCode() ?? 0;
        }


        //protected bool Equals(InfoIdentifier other)
        //{
        //    return string.Equals(StringRepresentation, other.StringRepresentation);
        //}
        protected bool Equals(InfoIdentifier other)
        {
            return string.Equals(StringRepresentation, other.StringRepresentation);
        }


        /// <summary>
        /// Compares with another identifier and returns whether their string representations equals.
        /// </summary>
        /// <param name="obj">Other identifier to be checked</param>
        /// <returns>True, if identifiers' string representations are equal; otherwise, false</returns>
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
                return StringRepresentation == (string) obj;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((InfoIdentifier) obj);
        }


        /// <summary>
        /// Returns string representation of the identifier.
        /// </summary>
        /// <returns>
        /// String representation of the identifier
        /// </returns>
        public override string ToString()
        {
            return StringRepresentation;
        }
    }
}
