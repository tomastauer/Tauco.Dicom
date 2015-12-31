using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public class StudyInfoTests
    {
        [Test]
        public void StudyInstanceUID_Setter_AfterGettingHashCode_ThrowsException()
        {
            // Arrange
            var study = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("123")
            };

            // Act
            study.GetHashCode();

            // Assert
            Assert.That(() => study.StudyInstanceUID = new InfoIdentifier("321"), Throws.InvalidOperationException);
        }

        [Test]
        public void DicomType_IsStudy()
        {
            // Arrange 
            var study = new StudyInfo();

            // Act + Assert
            Assert.That(study.DicomType, Is.EqualTo(DicomInfoType.Study));
        }


        [Test]
        public void Equals_StudiesWithSameUIDAreEqual()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var study1 = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };
            var study2 = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };

            // Act
            var result = study1.Equals(study2);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_StudiesWithSameUIDAreNotEqual()
        {
            // Arrange
            var study1 = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("123")
            };
            var study2 = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("321")
            };

            // Act
            var result = study1.Equals(study2);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var study = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };

            // Act
            var result = study.Equals(null);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithItself_ReturnsTrue()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var study = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };

            // Act
            var result = study.Equals(study);

            // Assert
            Assert.That(result, Is.True);
        }



        [Test]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            var infoIdentifier = new InfoIdentifier("123");
            var study = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };

            // Act
            var result = study.Equals(new object());

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_PatientsWithSameIDAndNameAreEqual()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var study1 = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };
            var study2 = new StudyInfo
            {
                StudyInstanceUID = infoIdentifier
            };

            // Act
            var hash1 = study1.GetHashCode();
            var hash2 = study2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }


        [Test]
        public void GetHashCode_PatientsWithDifferencNameAndIDAreNotEqual()
        {
            // Arrange
            var study1 = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("123")
            };
            var study2 = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("321")
            };

            // Act
            var hash1 = study1.GetHashCode();
            var hash2 = study2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }


        [TestCase("1510180012", "456.789", "{\"PatientID\":\"1510180012\",\"StudyInstanceUID\":\"456.789\"}")]
        [TestCase("151018/0012", "456.789", "{\"PatientID\":\"1510180012\",\"StudyInstanceUID\":\"456.789\"}")]
        public void SerializeToJson_CorrectOutput(string birthNumber, string studyUid, string expectedResult)
        {
            // Arrange
            var study = new StudyInfo
            {
                PatientID = new BirthNumber(birthNumber),
                StudyInstanceUID = new InfoIdentifier(studyUid)
            };

            // Act
            var result = JsonConvert.SerializeObject(study);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCase("{\"PatientID\":\"1510180012\",\"StudyInstanceUID\":\"456.789\"}", "1510180012", "456.789")]
        [TestCase("{\"PatientID\":\"151018/0012\",\"StudyInstanceUID\":\"456.789\"}", "1510180012", "456.789")]
        public void DeserializeToJson_CorrectOutput(string json, string birthNumber, string studyUid)
        {
            // Arrange + Act
            var study = JsonConvert.DeserializeObject<StudyInfo>(json);

            // Assert
            Assert.That(study.PatientID, Is.EqualTo(new BirthNumber(birthNumber)));
            Assert.That(study.StudyInstanceUID, Is.EqualTo(new InfoIdentifier(studyUid)));
        }
    }
}
