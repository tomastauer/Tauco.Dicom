using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Tauco.Dicom.Shared.Converters
{
    /// <summary>
    /// Specifies converter from <see cref="BirthNumber"/> into JSON.
    /// </summary>
    internal class BirthNumberJsonConverter : JsonConverter
    {
        private BirthNumberParser mBirthNumberParser;


        private BirthNumberParser BirthNumberParser => mBirthNumberParser ?? (mBirthNumberParser = new BirthNumberParser());


        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> is <see langword="null"/> -or- 
        /// <paramref name="value"/> is <see langword="null"/> -or- 
        /// <paramref name="serializer"/> is <see langword="null"/>
        /// </exception>
        public override void WriteJson([NotNull] JsonWriter writer, [NotNull] object value, [NotNull] JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            var birthNumber = (BirthNumber) value;
            serializer.Serialize(writer, birthNumber.StringRepresentationWithoutSlash);
        }


        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is <see langword="null"/>
        /// </exception>
        /// <returns>The object value. </returns>
        public override object ReadJson([NotNull] JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
  
            var value = reader.Value;
            return BirthNumberParser.GetBirthNumber(value?.ToString() ?? string.Empty);
        }


        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
