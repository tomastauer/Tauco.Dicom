using System;
using System.Diagnostics.CodeAnalysis;

using Dicom;

using NUnit.Framework;

using Tauco.Dicom.Models;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomSOPClassUIDProviderTests
    {
        [Test]
        public void GetDicomSOPClassUid_NullDicomInfo_ThrowsException()
        {
            // Arrange
            var dicomUIDProvider = new DicomSOPClassUIDProvider();

            // Act + Assert
            Assert.That(() => dicomUIDProvider.GetDicomSOPClassUid(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void GetDicomSOPClassUid_PatientInfo_ReturnsPatientRoot()
        {
            // Arrange
            var dicomUIDProvider = new DicomSOPClassUIDProvider();

            // Act
            var result = dicomUIDProvider.GetDicomSOPClassUid(new PatientInfo());

            // Assert
            Assert.That(result, Is.EqualTo(DicomUID.PatientRootQueryRetrieveInformationModelFIND));
        }

        
        [Test]
        public void GetDicomSOPClassUid_StudyInfo_ReturnsStudyRoot()
        {
            // Arrange
            var dicomUIDProvider = new DicomSOPClassUIDProvider();

            // Act
            var result = dicomUIDProvider.GetDicomSOPClassUid(new StudyInfo());

            // Assert
            Assert.That(result, Is.EqualTo(DicomUID.StudyRootQueryRetrieveInformationModelFIND));
        }


        [Test]
        public void GetDicomSOPClassUid_SeriesInfo_ReturnsStudyRoot()
        {
            // Arrange
            var dicomUIDProvider = new DicomSOPClassUIDProvider();

            // Act
            var result = dicomUIDProvider.GetDicomSOPClassUid(new SeriesInfo());

            // Assert
            Assert.That(result, Is.EqualTo(DicomUID.StudyRootQueryRetrieveInformationModelFIND));
        }
    }
}