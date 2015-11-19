using System;
using System.ComponentModel;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents dicom patient name.
    /// </summary>
    [TypeConverter(typeof(PatientNameTypeConverter))]
    [JsonConverter(typeof(PatientNameJsonConverter))]
    public class PatientName
    {
        /// <summary>
        /// Instantiate new instance of <see cref="PatientName"/>
        /// </summary>
        /// <param name="dicomString">Name in dicom format (Last Name^First Name^Middle Name^Prefix^Suffix)</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomString"/> is <see langword="null"/></exception>
        public PatientName([NotNull] string dicomString)
        {
            if (dicomString == null)
            {
                throw new ArgumentNullException(nameof(dicomString));
            }

            var names = dicomString.Split('^');
            
            LastName = GetName(names, 0);
            FirstName = GetName(names, 1);
            MiddleName = GetName(names, 2);
            Prefix = GetName(names, 3);
            Suffix = GetName(names, 4);
        }


        /// <summary>
        /// Gets or sets first name of the patient.
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets middle name fo the patient.
        /// </summary>
        public string MiddleName
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets last name of the patient.
        /// </summary>
        public string LastName
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets patient's name prefix.
        /// </summary>
        public string Prefix
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets patient's name suffix.
        /// </summary>
        public string Suffix
        {
            get;
            set;
        }


        /// <summary>
        /// Gets dicom string representation of name in format Last Name^First Name^Middle Name^Prefix^Suffix
        /// </summary>
        public string DicomString
        {
            get
            {
                var names = new[]
                {
                    LastName, FirstName, MiddleName, Prefix, Suffix
                };

                return string.Join("^", names).TrimEnd('^');
            }
        }


        /// <summary>
        /// Returns string representation of the patient name.
        /// </summary>
        /// <returns>
        /// String representation of the patient name
        /// </returns>
        public override string ToString()
        {
            var names = new[]
            {
                Prefix, FirstName, MiddleName, LastName, Suffix
            };

            return string.Join(" ", names.Where(c => !string.IsNullOrWhiteSpace(c)));
        }


        /// <summary>
        /// Gets single name from the given <paramref name="nameArray"/> according to the given <paramref name="index"/>.
        /// </summary>
        /// <param name="nameArray">Array of all names</param>
        /// <param name="index">Index of name to be obtained</param>
        /// <returns>If element at <paramref name="index"/> is empty or <see langword="null"/>, returns <see langword="null"/>; otherwise, returns the value</returns>
        private string GetName(string[] nameArray, int index)
        {
            var name = nameArray.ElementAtOrDefault(index);
            if (name == string.Empty)
            {
                return null;
            }

            return name;
        }


        /// <summary>
        /// Compares with another <see cref="PatientName"/> and returns whether their string representations equals.
        /// </summary>
        /// <param name="other">Other object to be checked</param>
        /// <returns>True, if <see cref="DicomString"/> is equal; otherwise, false</returns>
        protected bool Equals(PatientName other)
        {
            return DicomString.Equals(other.DicomString);
        }


        /// <summary>
        /// Compares with another object and returns whether their string representations equals.
        /// </summary>
        /// <param name="obj">Other object to be checked</param>
        /// <returns>True, if object is equal; otherwise, false</returns>
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
                return DicomString == (string) obj;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((PatientName) obj);
        }


        /// <summary>
        /// Compares with another object and returns whether string representation of <paramref name="obj"/> is contained within <see cref="DicomString"/>.
        /// </summary>
        /// <param name="obj">Other object to be checked</param>
        /// <returns>True, if object is contained; otherwise, false</returns>
        public bool Contains(object obj)
        {
            if (obj is string)
            {
                string objString = (string) obj;
                return DicomString.Contains(objString);
            }

            return DicomString.Contains(obj.ToString());
        }


        /// <summary>
        /// Returns the hash code for the identifier.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return DicomString.GetHashCode();
        }
    }
}
