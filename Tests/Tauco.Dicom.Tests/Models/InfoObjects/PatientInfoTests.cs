using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Tests.Models
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public class PatientInfoTests
    {
        [Test]
        public void PatientID_Setter_AfterGettingHashCode_ThrowsException()
        {
            // Arrange
            var patient = new PatientInfo
            {
                PatientID = new BirthNumber("9107256444")
            };

            // Act
            patient.GetHashCode();

            // Assert
            Assert.That(() => patient.PatientID = new BirthNumber("9107256444"), Throws.InvalidOperationException);
        }


        [Test]
        public void PatientName_Setter_AfterGettingHashCode_ThrowsException()
        {
            // Arrange
            var patient = new PatientInfo
            {
                PatientName = new PatientName("Doe^John")
            };

            // Act
            patient.GetHashCode();

            // Assert
            Assert.That(() => patient.PatientName = new PatientName("Doe^John"), Throws.InvalidOperationException);
        }


        [Test]
        public void DicomType_IsPatient()
        {
            // Arrange 
            var patient = new PatientInfo();

            // Act + Assert
            Assert.That(patient.DicomType, Is.EqualTo(DicomInfoType.Patient));
        }


        [Test]
        public void GetIdentifierHashCode_ReturnsHashCodeOfPatientID()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var patient = new PatientInfo
            {
                PatientID = birthNumber
            };

            // Act
            int hashCode = patient.GetIdentifierHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(birthNumber.GetHashCode()));
        }


        [Test]
        public void Equals_WithItself_ReturnsTrue()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var patient = new PatientInfo
            {
                PatientID = birthNumber
            };

            // Act
            var result = patient.Equals(patient);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_PatientsWithSameIDAndNameAreEqual()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var name = new PatientName("Doe^John");
            var patient1 = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };
            var patient2 = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };

            // Act
            var result = patient1.Equals(patient2);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_PatientsWithDifferentIDAndNameAreNotEqual()
        {
            // Arrange
            var patient1 = new PatientInfo
            {
                PatientID = new BirthNumber("9107256444"),
                PatientName = new PatientName("Doe^John")
            };
            var patient2 = new PatientInfo
            {
                PatientID = new BirthNumber("1510180012"),
                PatientName = new PatientName("Smith^Jack")
            };

            // Act
            var result = patient1.Equals(patient2);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var name = new PatientName("Doe^John");
            var patient = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };
            
            // Act
            var result = patient.Equals(null);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var name = new PatientName("Doe^John");
            var patient = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };
            
            // Act
            var result = patient.Equals(new object());

            // Assert
            Assert.That(result, Is.False);
        }
        

        [Test]
        public void GetHashCode_PatientsWithSameIDAndNameAreEqual()
        {
            // Arrange
            var birthNumber = new BirthNumber("9107256444");
            var name = new PatientName("Doe^John");
            var patient1 = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };
            var patient2 = new PatientInfo
            {
                PatientID = birthNumber,
                PatientName = name
            };

            // Act
            var hash1 = patient1.GetHashCode();
            var hash2 = patient2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }


        [Test]
        public void GetHashCode_PatientsWithDifferencNameAndIDAreNotEqual()
        {
            // Arrange
            var patient1 = new PatientInfo
            {
                PatientID = new BirthNumber("9107256444"),
                PatientName = new PatientName("Doe^John")
            };
            var patient2 = new PatientInfo
            {
                PatientID = new BirthNumber("1510180012"),
                PatientName = new PatientName("Smith^Jack")
            };

            // Act
            var hash1 = patient1.GetHashCode();
            var hash2 = patient2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }


        [TestCase("Doe^John", "151018/0012", "{\"PatientID\":\"1510180012\",\"PatientName\":\"Doe^John\"}")]
        [TestCase("Doe^John", "1510180012", "{\"PatientID\":\"1510180012\",\"PatientName\":\"Doe^John\"}")]
        public void SerializeToJson_CorrectOutput(string patientName, string birthNumber, string expectedResult)
        {
            // Arrange
            var patient = new PatientInfo
            {
                PatientID = new BirthNumber(birthNumber),
                PatientName = new PatientName(patientName),
                AdditionalInstances = { new PatientInfo() }
            };

            // Act
            var result = JsonConvert.SerializeObject(patient);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCase("{\"PatientID\":\"1510180012\",\"PatientName\":\"Doe^John\"}", "Doe^John", "1510180012")]
        [TestCase("{\"PatientID\":\"151018/0012\",\"PatientName\":\"Doe^John\"}", "Doe^John", "1510180012")]
        public void DeserializeToJson_CorrectOutput(string json, string patientName, string birthNumber)
        {
            // Arrange + Act
            var patient = JsonConvert.DeserializeObject<PatientInfo>(json);
            
            // Assert
            Assert.That(patient.PatientName, Is.EqualTo(new PatientName(patientName)));
            Assert.That(patient.PatientID, Is.EqualTo(new BirthNumber(birthNumber)));
        }
    }
}
