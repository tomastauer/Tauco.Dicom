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
    public class BirthNumberJsonConverterTests
    {
        [Test]
        public void WriteJson_NullWriter_ThrowsException()
        {
            // Arrange
            var birthNumberJsonConverter = new BirthNumberJsonConverter();

            // Act + Assert
            Assert.That(() => birthNumberJsonConverter.WriteJson(null, 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullValue_ThrowsException()
        {
            // Arrange
            var birthNumberJsonConverter = new BirthNumberJsonConverter();

            // Act + Assert
            Assert.That(() => birthNumberJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), null, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullSerializer_ThrowsException()
        {
            // Arrange
            var birthNumberJsonConverter = new BirthNumberJsonConverter();

            // Act + Assert
            Assert.That(() => birthNumberJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), 0, null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void ReadJson_NullReader_ThrowsException()
        {
            // Arrange
            var birthNumberJsonConverter = new BirthNumberJsonConverter();

            // Act + Assert
            Assert.That(() => birthNumberJsonConverter.ReadJson(null, typeof (object), 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void CanConvert_ReturnsTrue()
        {
            // Arrange
            var birthNumberJsonConverter = new BirthNumberJsonConverter();

            // Act
            var result = birthNumberJsonConverter.CanConvert(null);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }


        [TestCase("151018/0012", "\"1510180012\"")]
        [TestCase("1510180012", "\"1510180012\"")]
        public void WriteJson_SerializesCorrectly(string birthNumber, string expectedResult)
        {
            // Arrange  
            var testBirthNumber = new BirthNumber(birthNumber);
            
            // Act
            var result = JsonConvert.SerializeObject(testBirthNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCase("\"1510180012\"")]
        [TestCase("\"151018/0012\"")]
        public void ReadJson_DeserializesCorrectly(string birthNumber)
        {
            // Arrange + Act
            var result = JsonConvert.DeserializeObject<BirthNumber>(birthNumber);

            // Assert
            Assert.That(result.BirthDate, Is.EqualTo(new DateTime(2015,10,18)));
            Assert.That(result.Gender, Is.EqualTo(Gender.Male));
            Assert.That(result.Suffix, Is.EqualTo(001));
            Assert.That(result.CheckDigit, Is.EqualTo(2));
        }
    }
}
