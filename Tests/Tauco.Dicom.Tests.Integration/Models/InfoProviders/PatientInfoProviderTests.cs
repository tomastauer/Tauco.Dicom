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
    public class PatientInfoProviderTests
    {
        private IPatientInfoProvider mPatientInfoProvider;

        [SetUp]
        public void SetUp()
        {
            var container = new WindsorContainer().Install(new CommonInstaller());
            mPatientInfoProvider = container.Resolve<IPatientInfoProvider>();
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
        public void GetPatients_SyncFromServer_ReturnsAllPatients()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients();

            // Act
            var patients = patientsQuery.ToList();

            // Assert
            Assert.That(patients.Count, Is.EqualTo(3));
        }


        [Test]
        public async void GetPatients_AsyncFromServer_ReturnsAllPatients()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients();

            // Act
            var patients = await patientsQuery.ToListAsync();

            // Assert
            Assert.That(patients.Count, Is.EqualTo(3));
        }


        [Test]
        public void GetPatients_SyncFromCache_ReturnsAllPatients()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients();
            // Fill cache
            patientsQuery.ToList();

            // Act
            var patients = patientsQuery.LoadFromCache().ToList();

            // Assert
            Assert.That(patients.Count, Is.EqualTo(3));
        }


        [Test]
        public async void GetPatients_AsyncFromCache_ReturnsAllPatients()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients();
            // Fill cache
            await patientsQuery.ToListAsync();

            // Act
            var patients = await patientsQuery.LoadFromCache().ToListAsync();

            // Assert
            Assert.That(patients.Count, Is.EqualTo(3));
        }


        [Test]
        public void GetPatients_SpecificPatientFromServer_Equals_ReturnsCorrectPatient()
        {
            // Arrange
            var patientsQueryName = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientName, "Doe^John");
            var patientsQueryBirthNumber = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientID, "151018/0012");
            var patientsQueryBoth = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientName, "Doe^John").WhereEquals(DicomTags.PatientID, "151018/0012");

            // Act
            var patient1 = patientsQueryName.Single();
            var patient2 = patientsQueryBirthNumber.Single();
            var patient3 = patientsQueryBoth.Single();

            // Assert
            Assert.That(patient1, Is.EqualTo(patient2));
            Assert.That(patient2, Is.EqualTo(patient3));
        }


        [Test]
        public void GetPatients_SpecificPatientFromCache_Equals_ReturnsCorrectPatient()
        {
            // Arrange
            var patientsQueryName = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientName, "Doe^John");
            var patientsQueryBirthNumber = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientID, "151018/0012");
            var patientsQueryBoth = mPatientInfoProvider.GetPatients().WhereEquals(DicomTags.PatientName, "Doe^John").WhereEquals(DicomTags.PatientID, "151018/0012");

            // Fill cache
            mPatientInfoProvider.GetPatients().ToList();

            // Act
            var patient1 = patientsQueryName.LoadFromCache().Single();
            var patient2 = patientsQueryBirthNumber.LoadFromCache().Single();
            var patient3 = patientsQueryBoth.LoadFromCache().Single();

            // Assert
            Assert.That(patient1, Is.EqualTo(patient2));
            Assert.That(patient2, Is.EqualTo(patient3));
        }


        [Test]
        public void GetPatients_SpecificPatientFromServer_Like_ReturnsCorrectPatient()
        {
            // Arrange
            var patientsQueryName = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe^John");
            var patientsQueryBirthNumber = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientID, "151018/0012");
            var patientsQueryBoth = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe^John").WhereLike(DicomTags.PatientID, "151018/0012");

            // Act
            var patient1 = patientsQueryName.Single();
            var patient2 = patientsQueryBirthNumber.Single();
            var patient3 = patientsQueryBoth.Single();

            // Assert
            Assert.That(patient1, Is.EqualTo(patient2));
            Assert.That(patient2, Is.EqualTo(patient3));
        }


        [Test]
        public void GetPatients_SpecificPatientFromCache_Like_ReturnsCorrectPatient()
        {
            // Arrange
            var patientsQueryName = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe^John");
            var patientsQueryBirthNumber = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientID, "151018/0012");
            var patientsQueryBoth = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe^John").WhereLike(DicomTags.PatientID, "151018/0012");

            // Fill cache
            mPatientInfoProvider.GetPatients().ToList();

            // Act
            var patient1 = patientsQueryName.LoadFromCache().Single();
            var patient2 = patientsQueryBirthNumber.LoadFromCache().Single();
            var patient3 = patientsQueryBoth.LoadFromCache().Single();

            // Assert
            Assert.That(patient1, Is.EqualTo(patient2));
            Assert.That(patient2, Is.EqualTo(patient3));
        }


        [Test]
        public void MultipleInstance_SyncFromServer()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe").WhereLike(DicomTags.PatientID, "0012");

            // Act
            var patients = patientsQuery.Single();

            // Assert
            Assert.That(patients.AdditionalInstances.Count, Is.EqualTo(1));
        }


        [Test]
        public void MultipleInstance_SyncFromCache()
        {
            // Arrange
            var patientsQuery = mPatientInfoProvider.GetPatients().WhereLike(DicomTags.PatientName, "Doe").WhereLike(DicomTags.PatientID, "0012");
            // Fill cache
            patientsQuery.ToList();

            // Act
            var patients = patientsQuery.LoadFromCache().Single();

            // Assert
            Assert.That(patients.AdditionalInstances.Count, Is.EqualTo(1));
        }


        [Test]
        public async void GetPatientByBirthNumberAsync_FromServer_ReturnsCorrectPatient()
        {
            // Arrange + Act
            var patientWithoutSlash = await mPatientInfoProvider.GetPatientByBirthNumberAsync("8002291110");
            var patientWithSlash = await mPatientInfoProvider.GetPatientByBirthNumberAsync("800229/1110");

            // Assert
            Assert.That(patientWithoutSlash, Is.Not.Null);
            Assert.That(patientWithSlash, Is.Not.Null);
            Assert.That(patientWithSlash, Is.EqualTo(patientWithSlash));
        }


        [Test]
        public async void GetPatientByBirthNumberAsync_NonExisting_FromServer_ReturnsNull()
        {
            // Arrange + Act
            var patient = await mPatientInfoProvider.GetPatientByBirthNumberAsync("9254252568");

            // Assert
            Assert.That(patient, Is.Null);
        }


        [Test]
        public async void GetPatientByBirthNumberAsync_FromCache_ReturnsCorrectPatient()
        {
            // Arrange
            // Fill cache
            await mPatientInfoProvider.GetPatients().ToListAsync();

            // Act
            var patientWithoutSlash = await mPatientInfoProvider.GetPatientByBirthNumberAsync("8002291110", true);
            var patientWithSlash = await mPatientInfoProvider.GetPatientByBirthNumberAsync("800229/1110", true);

            // Assert
            Assert.That(patientWithoutSlash, Is.Not.Null);
            Assert.That(patientWithSlash, Is.Not.Null);
            Assert.That(patientWithSlash, Is.EqualTo(patientWithSlash));
        }


        [Test]
        public async void GetPatientByBirthNumberAsync_NonExisting_FromCache_ReturnsNull()
        {
            // Arrange
            // Fill cache
            await mPatientInfoProvider.GetPatients().ToListAsync();

            // Act
            var patient = await mPatientInfoProvider.GetPatientByBirthNumberAsync("9254252568", true);

            // Assert
            Assert.That(patient, Is.Null);
        }


        [Test]
        public async void DownloadImagesAsync_DefaultAction()
        {
            // Arrange
            var patient = await mPatientInfoProvider.GetPatientByBirthNumberAsync("8002291110");

            // Act
            Assert.That(Directory.Exists("DICOM/1.2.276.0.7230010.3.1.2.655267989.4560.1447013493.699/"), Is.False);

            await mPatientInfoProvider.DownloadImagesAsync(patient);

            var directory = new DirectoryInfo("DICOM/1.2.276.0.7230010.3.1.2.655267989.4560.1447013493.699/");

            // Assert
            Assert.That(directory.GetFiles().Length, Is.EqualTo(3));
        }
    }
}
