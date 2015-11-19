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
    public class PatientNameJsonConverterTests
    {
        [Test]
        public void WriteJson_NullWriter_ThrowsException()
        {
            // Arrange
            var patientNameJsonConverter = new PatientNameJsonConverter();

            // Act + Assert
            Assert.That(() => patientNameJsonConverter.WriteJson(null, 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullValue_ThrowsException()
        {
            // Arrange
            var patientNameJsonConverter = new PatientNameJsonConverter();

            // Act + Assert
            Assert.That(() => patientNameJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), null, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void WriteJson_NullSerializer_ThrowsException()
        {
            // Arrange
            var patientNameJsonConverter = new PatientNameJsonConverter();

            // Act + Assert
            Assert.That(() => patientNameJsonConverter.WriteJson(new JsonTextWriter(new StreamWriter(new MemoryStream())), 0, null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void ReadJson_NullReader_ThrowsException()
        {
            // Arrange
            var patientNameJsonConverter = new PatientNameJsonConverter();

            // Act + Assert
            Assert.That(() => patientNameJsonConverter.ReadJson(null, typeof (object), 0, new JsonSerializer()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void CanConvert_ReturnsTrue()
        {
            // Arrange
            var patientNameJsonConverter = new PatientNameJsonConverter();

            // Act
            var result = patientNameJsonConverter.CanConvert(null);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }


        [Test]
        public void WriteJson_SerializesCorrectly()
        {
            // Arrange  
            var testPatientName = new PatientName("Doe^John");
            
            // Act
            var result = JsonConvert.SerializeObject(testPatientName);

            // Assert
            Assert.That(result, Is.EqualTo("\"Doe^John\""));
        }

        
        [Test]
        public void ReadJson_DeserializesCorrectly()
        {
            // Arrange + Act
            var result = JsonConvert.DeserializeObject<PatientName>("\"Doe^John\"");

            // Assert
            Assert.That(result.DicomString, Is.EqualTo("Doe^John"));
        }
    }
}
