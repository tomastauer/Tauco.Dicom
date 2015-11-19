using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

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
    public class StudyInfoProviderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            MockProvider mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof (StudyInfoProvider), null, new List<Func<object>>
            {
                mockProvider.GetDicomQueryProviderForStudiesFake,
                mockProvider.GetDicomDownloaderForStudiesFake
            });
        }


        [Test]
        public void DownloadImages_NullStudy_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(mockProvider.GetDicomQueryProviderForStudiesFake(), mockProvider.GetDicomDownloaderForStudiesFake());


            // Act + Assert
            Assert.That(() => studyInfoProvider.DownloadImagesAsync(null).Exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public async void DownloadImages_ProperMethodsAreCalled()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();

            var downloader = mockProvider.GetDicomDownloaderForStudiesFake();
            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(mockProvider.GetDicomQueryProviderForStudiesFake(), downloader);
            InfoIdentifier studyIdentifier = new InfoIdentifier("1.2.3");
            StudyInfo studyInfo = new StudyInfo
            {
                StudyInstanceUID = studyIdentifier
            };

            // Act
            await studyInfoProvider.DownloadImagesAsync(studyInfo);

            // Assert
            Assert.That(() => downloader.Received(1).DownloadAsync(studyIdentifier, Arg.Any<Func<InfoIdentifier, InfoIdentifier, Stream>>()), Throws.Nothing);
        }


        [Test]
        public void GetStudies_ProperQueryIsCreated()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<StudyInfo>>();
            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(queryProvider, mockProvider.GetDicomDownloaderForStudiesFake());

            // Act
            studyInfoProvider.GetStudies();

            // Assert
            Assert.That(() => queryProvider.Received(1).GetDicomQuery(), Throws.Nothing);
        }


        [Test]
        public void GetStudiesForPatient_NullPatient_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(mockProvider.GetDicomQueryProviderForStudiesFake(), mockProvider.GetDicomDownloaderForStudiesFake());

            // Act + Assert
            Assert.That(() => studyInfoProvider.GetStudiesForPatient(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void GetStudiesForPatient_ProperQueryIsCreated([Values("910725/6444", "9107256444")] string birthNumber)
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<StudyInfo>>();
            var dataLoader = Substitute.For<IDicomDataLoader<StudyInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<StudyInfo>>();
            var query = Substitute.For<DicomQuery<StudyInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            PatientInfo patient = new PatientInfo
            {
                PatientID = new BirthNumber(birthNumber)
            };

            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(queryProvider, mockProvider.GetDicomDownloaderForStudiesFake());

            // Act
            studyInfoProvider.GetStudiesForPatient(patient);

            // Assert
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.PatientID, birthNumber), Throws.Nothing);
        }


        [Test]
        public async void GetStudyByIDAsync_FromCache_ProperQueryIsCreated()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var queryProvider = mockProvider.GetDicomQueryProviderForStudiesFake();
            var dataLoader = Substitute.For<IDicomDataLoader<StudyInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<StudyInfo>>();
            var query = Substitute.For<DicomQuery<StudyInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(queryProvider, mockProvider.GetDicomDownloaderForStudiesFake());

            // Act
            await studyInfoProvider.GetStudyByIDAsync("1.2.3", true);

            // Assert
            Assert.That(() => dataLoader.Received(1).LoadDataFromCacheAsync(whereCollection), Throws.Nothing);
        }


        [Test]
        public async void GetStudyByIDAsync_FromServer_ProperQueryIsCreated()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var queryProvider = mockProvider.GetDicomQueryProviderForStudiesFake();
            var dataLoader = Substitute.For<IDicomDataLoader<StudyInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<StudyInfo>>();
            var query = Substitute.For<DicomQuery<StudyInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(queryProvider, mockProvider.GetDicomDownloaderForStudiesFake());

            // Act
            await studyInfoProvider.GetStudyByIDAsync("1.2.3");

            // Assert
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.StudyInstanceUID, "1.2.3"), Throws.Nothing);
        }


        [Test]
        public void GetStudyByIDAsync_NullIdentifier_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            StudyInfoProvider studyInfoProvider = new StudyInfoProvider(mockProvider.GetDicomQueryProviderForStudiesFake(), mockProvider.GetDicomDownloaderForStudiesFake());

            // Act
            AggregateException exception = studyInfoProvider.GetStudyByIDAsync(null).Exception;

            // Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentNullException>());
        }
    }
}