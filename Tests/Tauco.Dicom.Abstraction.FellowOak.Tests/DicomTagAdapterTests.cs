using System;

using Dicom;

using NUnit.Framework;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    public class DicomTagAdapterTests
    {
        [Test]
        public void GetDicomTag_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            var dicomTagAdapter = new DicomTagAdapter();

            // Act + Assert
            Assert.That(() => dicomTagAdapter.GetDicomTag(DicomTags.Undefined), Throws.ArgumentException);
        }


        [Test]
        public void GetDicomTag_ReturnsCorrectTag()
        {
            // Arrange
            var dicomTagAdapter = new DicomTagAdapter();

            // Act
            var patientId = dicomTagAdapter.GetDicomTag(DicomTags.PatientID);
            var patientName = dicomTagAdapter.GetDicomTag(DicomTags.PatientName);

            // Arrange
            Assert.That(patientId, Is.EqualTo(DicomTag.PatientID));
            Assert.That(patientName, Is.EqualTo(DicomTag.PatientName));
        }
    }
}
