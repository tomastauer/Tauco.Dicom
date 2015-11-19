using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Castle.Windsor;

using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Tests.Integration.Models
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class StudyInfoProviderTests
    {
        private IStudyInfoProvider mStudyInfoProvider;
        private PatientInfo mPatient;

        [SetUp]
        public void SetUp()
        {
            var container = new WindsorContainer().Install(new CommonInstaller());
            mStudyInfoProvider = container.Resolve<IStudyInfoProvider>();
            var patientInfoProvider = container.Resolve<IPatientInfoProvider>();

            mPatient = patientInfoProvider.GetPatientByBirthNumberAsync("151018/0012").Result;
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            if (Directory.Exists("DICOM"))
            {
                Directory.Delete("DICOM", true);
            }
        }
        

        [Test]
        public void GetStudies_SyncFromServer_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies();

            // Act
            var studies = studyQuery.ToList();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(6));
        }


        [Test]
        public async void GetStudies_AsyncFromServer_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies();

            // Act
            var studies = await studyQuery.ToListAsync();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(6));
        }


        [Test]
        public void GetStudies_SyncFromCache_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies();
            // Fill cache
            studyQuery.ToList();

            // Act
            var studies = studyQuery.LoadFromCache().ToList();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(6));
        }


        [Test]
        public async void GetStudies_AsyncFromCache_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies();
            // Fill cache
            await studyQuery.ToListAsync();

            // Act
            var studies = await studyQuery.LoadFromCache().ToListAsync();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(6));
        }


        [Test]
        public void GetStudies_SpecificStudyFromServer_Equals_ReturnsCorrectStudy()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies().WhereEquals(DicomTags.StudyInstanceUID, "1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838");

            // Act + Assert
            Assert.That(() => studyQuery.Single(), Throws.Nothing);
        }


        [Test]
        public void GetStudies_SpecificStudyFromCache_Equals_ReturnsCorrectStudy()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies().WhereEquals(DicomTags.StudyInstanceUID, "1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838");

            // Fill cache
            mStudyInfoProvider.GetStudies().ToList();

            // Act + Assert
            Assert.That(() => studyQuery.LoadFromCache().Single(), Throws.Nothing);
        }


        [Test]
        public void GetStudies_SpecificStudyFromServer_Like_ReturnsCorrectStudy()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies().WhereLike(DicomTags.StudyInstanceUID, "1447498448.838");

            // Act + Assert
            Assert.That(() => studyQuery.Single(), Throws.Nothing);
        }


        [Test]
        public void GetStudies_SpecificStudyFromCache_Like_ReturnsCorrectStudy()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudies().WhereLike(DicomTags.StudyInstanceUID, "1447498448.838");

            // Fill cache
            mStudyInfoProvider.GetStudies().ToList();

            // Act + Assert
            Assert.That(() => studyQuery.LoadFromCache().Single(), Throws.Nothing);
        }


        [Test]
        public void GetStudiesForPatient_FromServer_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudiesForPatient(mPatient);

            // Act
            var studies = studyQuery.ToList();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(4));
        }


        [Test]
        public void GetStudiesForPatient_FromCache_ReturnsAllStudies()
        {
            // Arrange
            var studyQuery = mStudyInfoProvider.GetStudiesForPatient(mPatient);
            // Fill cache
            studyQuery.ToList();

            // Act
            var studies = studyQuery.LoadFromCache().ToList();

            // Assert
            Assert.That(studies.Count, Is.EqualTo(4));
        }


        [Test]
        public async void GetStudyByIDAsync_FromServer_ReturnsCorrectStudy()
        {
            // Arrange + Act
            var study = await mStudyInfoProvider.GetStudyByIDAsync("1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838");

            // Assert
            Assert.That(study, Is.Not.Null);
        }


        [Test]
        public async void GetStudyByIDAsync_NonExisting_FromServer_ReturnsNull()
        {
            // Arrange + Act
            var study = await mStudyInfoProvider.GetStudyByIDAsync("666");

            // Assert
            Assert.That(study, Is.Null);
        }


        [Test]
        public async void GetStudyByIDAsync_FromCache_ReturnsCorrectStudy()
        {
            // Arrange
            // Fill cache
            await mStudyInfoProvider.GetStudies().ToListAsync();

            // Act
            var study = await mStudyInfoProvider.GetStudyByIDAsync("1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838", true);

            // Assert
            Assert.That(study, Is.Not.Null);
        }


        [Test]
        public async void GetStudyByIDAsync_NonExisting_FromCache_ReturnsNull()
        {
            // Arrange
            // Fill cache
            await mStudyInfoProvider.GetStudies().ToListAsync();

            // Act
            var study = await mStudyInfoProvider.GetStudyByIDAsync("666", true);

            // Assert
            Assert.That(study, Is.Null);
        }


        [Test]
        public async void DownloadImagesAsync_DefaultAction()
        {
            // Arrange
            var study = await mStudyInfoProvider.GetStudyByIDAsync("1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838");

            // Act
            Assert.That(Directory.Exists("DICOM/1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838/"), Is.False);

            await mStudyInfoProvider.DownloadImagesAsync(study);

            var directory = new DirectoryInfo("DICOM/1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838/");

            // Assert
            Assert.That(directory.GetFiles().Length, Is.EqualTo(3));
        }
    }
}
