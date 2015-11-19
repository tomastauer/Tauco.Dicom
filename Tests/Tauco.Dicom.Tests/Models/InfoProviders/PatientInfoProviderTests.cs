using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Castle.Windsor;

using NSubstitute;
using NSubstitute.Core;

using NUnit.Framework;

using Tauco.Cache;
using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Models
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PatientInfoProviderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof (PatientInfoProvider), null, new List<Func<object>>
            {
                mockProvider.GetDicomQueryProviderForPatientsFake,
                mockProvider.GetStudyInfoProviderFake,
                mockProvider.GetBirthNumberParserFake
            });
        }


        [Test]
        public void GetPatients_ProperQueryIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<PatientInfo>>();
            var patientInfoProvider = new PatientInfoProvider(queryProvider,mockProvider.GetStudyInfoProviderFake(), mockProvider.GetBirthNumberParserFake());

            // Act
            patientInfoProvider.GetPatients();

            // Assert
            Assert.That(() => queryProvider.Received(1).GetDicomQuery(), Throws.Nothing);
        }


        [Test]
        public void GetPatientByBirthNumberAsync_NullBirthNumber_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = new PatientInfoProvider(mockProvider.GetDicomQueryProviderForPatientsFake(), mockProvider.GetStudyInfoProviderFake(), mockProvider.GetBirthNumberParserFake());

            // Act
            var exception = patientInfoProvider.GetPatientByBirthNumberAsync(null).Exception;

            // Act + Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void GetPatientByBirthNumberAsync_InvalidBirthNumber_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = new PatientInfoProvider(mockProvider.GetDicomQueryProviderForPatientsFake(), mockProvider.GetStudyInfoProviderFake(), new BirthNumberParser());

            // Act
            var exception = patientInfoProvider.GetPatientByBirthNumberAsync("910725/6443").Exception;
                
            // Act + Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentException>());
        }


        [Test]
        public async void GetPatientByBirthNumber_FromServer_ProperQueryIsCreated([Values("910725/6444", "9107256444")] string birthNumber)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<PatientInfo>>();
            var dataLoader = Substitute.For<IDicomDataLoader<PatientInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<PatientInfo>>();
            var query = Substitute.For<DicomQuery<PatientInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            var patientInfoProvider = new PatientInfoProvider(queryProvider, mockProvider.GetStudyInfoProviderFake(), mockProvider.GetBirthNumberParserFake());

            // Act
            await patientInfoProvider.GetPatientByBirthNumberAsync(birthNumber);

            // Assert
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.PatientID, "910725/6444"), Throws.Nothing);
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.PatientID, "9107256444"), Throws.Nothing);
        }


        [Test]
        public async void GetPatientByBirthNumber_FromCache_ProperMethodIsCalled([Values("910725/6444", "9107256444")] string birthNumber)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<PatientInfo>>();
            var dataLoader = Substitute.For<IDicomDataLoader<PatientInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<PatientInfo>>();
            var query = Substitute.For<DicomQuery<PatientInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            var patientInfoProvider = new PatientInfoProvider(queryProvider, mockProvider.GetStudyInfoProviderFake(), mockProvider.GetBirthNumberParserFake());

            // Act
            await patientInfoProvider.GetPatientByBirthNumberAsync(birthNumber, true);

            // Assert
            Assert.That(() => dataLoader.Received(1).LoadDataFromCacheAsync(whereCollection), Throws.Nothing);
                
        }

            
        [Test]
        public void DownloadImages_NullPatient_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = new PatientInfoProvider(mockProvider.GetDicomQueryProviderForPatientsFake(), mockProvider.GetStudyInfoProviderFake(), mockProvider.GetBirthNumberParserFake());

            // Act + Assert
            Assert.That(() => patientInfoProvider.DownloadImagesAsync(null).Exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public async void DownloadImages_ProperMethodsAreCalled()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyInfoProvider = Substitute.For<IStudyInfoProvider>();
            var dataLoader = Substitute.For<IDicomDataLoader<StudyInfo>>();
            var whereCollection = Substitute.For<WhereCollection<StudyInfo>>(mockProvider.GetDicomMappingFake());
            var query = Substitute.For<DicomQuery<StudyInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            
            dataLoader.LoadDataFromServerAsync(Arg.Any<IWhereCollection<StudyInfo>>()).Returns(c =>
                Task<IImmutableList<StudyInfo>>.Factory.StartNew(() => 
                    new List<StudyInfo>
                    {
                        new StudyInfo(),
                        new StudyInfo()
                    }.ToImmutableList()));
            
            studyInfoProvider.GetStudiesForPatient(Arg.Any<PatientInfo>()).Returns(query);
            var patientInfoProvider = new PatientInfoProvider(mockProvider.GetDicomQueryProviderForPatientsFake(), studyInfoProvider, mockProvider.GetBirthNumberParserFake());
            var patient = new PatientInfo();
                
            // Act
            await patientInfoProvider.DownloadImagesAsync(patient);

            // Assert
            Assert.That(() => studyInfoProvider.Received(1).GetStudiesForPatient(patient), Throws.Nothing);
            Assert.That(() => studyInfoProvider.Received(2).DownloadImagesAsync(Arg.Any<StudyInfo>(), Arg.Any<Func<InfoIdentifier, InfoIdentifier, Stream>>()), Throws.Nothing);
        }
    }
}