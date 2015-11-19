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
    public class SeriesInfoTests
    {
        [Test]
        public void SeriesInstanceUID_Setter_AfterGettingHashCode_ThrowsException()
        {
            // Arrange
            var series = new SeriesInfo
            {
                SeriesInstanceUID = new InfoIdentifier("123")
            };

            // Act
            series.GetHashCode();

            // Assert
            Assert.That(() => series.SeriesInstanceUID = new InfoIdentifier("321"), Throws.InvalidOperationException);
        }

        [Test]
        public void DicomType_IsSeries()
        {
            // Arrange 
            var series = new SeriesInfo();

            // Act + Assert
            Assert.That(series.DicomType, Is.EqualTo(DicomInfoType.Series));
        }


        [Test]
        public void Equals_SeriesWithSameUIDAreEqual()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var series1 = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };
            var series2 = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };

            // Act
            var result = series1.Equals(series2);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_SeriesWithSameUIDAreNotEqual()
        {
            // Arrange
            var series1 = new SeriesInfo
            {
                SeriesInstanceUID = new InfoIdentifier("123")
            };
            var series2 = new SeriesInfo
            {
                SeriesInstanceUID = new InfoIdentifier("321")
            };

            // Act
            var result = series1.Equals(series2);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithItself_ReturnsTrue()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var series = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };

            // Act
            var result = series.Equals(series);

            // Assert
            Assert.That(result, Is.True);
        }
        

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var series = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };

            // Act
            var result = series.Equals(null);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var series = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };

            // Act
            var result = series.Equals(new object());

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_SeriesWithSameUIDAreEqual()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("123");
            var series1 = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };
            var series2 = new SeriesInfo
            {
                SeriesInstanceUID = infoIdentifier
            };

            // Act
            var hash1 = series1.GetHashCode();
            var hash2 = series2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }


        [Test]
        public void GetHashCode_SeriesWithSameUIDAreNotEqual()
        {
            // Arrange
            var series1 = new SeriesInfo
            {
                SeriesInstanceUID = new InfoIdentifier("123")
            };
            var series2 = new SeriesInfo
            {
                SeriesInstanceUID = new InfoIdentifier("456")
            };
            
            // Act
            var hash1 = series1.GetHashCode();
            var hash2 = series2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }


        [Test]
        public void SerializeToJson_CorrectOutput()
        {
            // Arrange
            var series = new SeriesInfo
            {
                StudyInstanceUID = new InfoIdentifier("123.456"),
                SeriesInstanceUID = new InfoIdentifier("456.789"),
                Modality = "MR"
            };

            // Act
            var result = JsonConvert.SerializeObject(series);

            // Assert
            Assert.That(result, Is.EqualTo("{\"StudyInstanceUID\":\"123.456\",\"SeriesInstanceUID\":\"456.789\",\"Modality\":\"MR\"}"));
        }


        [Test]
        public void DeserializeToJson_CorrectOutput()
        {
            // Arrange + Act
            var series = JsonConvert.DeserializeObject<SeriesInfo>("{\"StudyInstanceUID\":\"123.456\",\"SeriesInstanceUID\":\"456.789\",\"Modality\":\"MR\"}");

            // Assert
            Assert.That(series.StudyInstanceUID, Is.EqualTo(new InfoIdentifier("123.456")));
            Assert.That(series.SeriesInstanceUID, Is.EqualTo(new InfoIdentifier("456.789")));
            Assert.That(series.Modality, Is.EqualTo(new InfoIdentifier("MR")));
        }
    }
}
