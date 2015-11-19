using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Newtonsoft.Json;

using NUnit.Framework;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared.Tests.Converters
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class InfoIdentifierJsonConverterTests
    {
        [Test]
        public void WriteJson_NullWriter_ThrowsException()
        {
            // Arrange
            var infoIdentifierJsonConverter = new InfoIdentifierJsonConverter();

            // Act + Assert
            Assert.That(() => infoIdentifierJsonConverter.WriteJson(null, 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullValue_ThrowsException()
        {
            // Arrange
            var infoIdentifierJsonConverter = new InfoIdentifierJsonConverter();

            // Act + Assert
            Assert.That(() => infoIdentifierJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), null, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullSerializer_ThrowsException()
        {
            // Arrange
            var infoIdentifierJsonConverter = new InfoIdentifierJsonConverter();

            // Act + Assert
            Assert.That(() => infoIdentifierJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), 0, null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void ReadJson_NullReader_ThrowsException()
        {
            // Arrange
            var infoIdentifierJsonConverter = new InfoIdentifierJsonConverter();

            // Act + Assert
            Assert.That(() => infoIdentifierJsonConverter.ReadJson(null, typeof (object), 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void CanConvert_ReturnsTrue()
        {
            // Arrange
            var infoIdentifierJsonConverter = new InfoIdentifierJsonConverter();

            // Act
            var result = infoIdentifierJsonConverter.CanConvert(null);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }


        [Test]
        public void WriteJson_SerializesCorrectly()
        {
            // Arrange  
            var infoIdentifier = new InfoIdentifier("1.2.3");

            // Act
            var result = JsonConvert.SerializeObject(infoIdentifier);

            // Assert
            Assert.That(result, Is.EqualTo("\"1.2.3\""));
        }


        [Test]
        public void ReadJson_DeserializesCorrectly()
        {
            // Arrange + Act
            var result = JsonConvert.DeserializeObject<InfoIdentifier>("\"1.2.3\"");

            // Assert
            Assert.That(result.StringRepresentation, Is.EqualTo("1.2.3"));
        }
    }
}
