using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class DicomMappingTests
    {
        [Test]
        public void DicomMapping_TypeWithoutProperties_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new DicomMapping(typeof(ObjectWithoutProperties)), Throws.ArgumentException);
        }


        [Test]
        public void DicomMapping_TypeWithoutAttributes_CollectionIsEmpty()
        {
            // Arrange
            var objectWithoutAttributes = new ObjectWithoutAttributes();

            // Act
            var dicomMapping = new DicomMapping(objectWithoutAttributes.GetType());

            // Assert
            CollectionAssert.IsEmpty(dicomMapping);
        }


        [Test]
        public void DicomMapping_TypeWithAttributes_CollectionContainsAttributes()
        {
            // Arrange
            var objectWithAttributes = new ObjectWithAttributes();

            // Act
            var dicomMapping = new DicomMapping(objectWithAttributes.GetType());

            // Assert
            CollectionAssert.IsNotEmpty(dicomMapping);

            Assert.That(dicomMapping.Single(c => c.Key.PropertyType == typeof(string)).Value, Is.EqualTo(DicomTags.PatientName));
            Assert.That(dicomMapping.Single(c => c.Key.PropertyType == typeof(int)).Value, Is.EqualTo(DicomTags.PatientID));
        }


        private class ObjectWithoutProperties
        {
        }


        private class ObjectWithoutAttributes 
        {
            public string PatientName
            {
                get;
                set;
            }


            public int PatientID
            {
                get;
                set;
            }
        }


        private class ObjectWithAttributes
        {
            [Dicom(DicomTags.PatientName)]
            public string PatientName
            {
                get;
                set;
            }


            [Dicom(DicomTags.PatientID)]
            public int PatientID
            {
                get;
                set;
            }
        }
    }
}
