using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Models
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class SeriesInfoProviderTests
    {
        [Test]
        public void Constructor_NullDicomQueryProvider_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new SeriesInfoProvider(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void GetSeries_ProperQueryIsCreated()
        {
            // Arrange
            var queryProvider = Substitute.For<IDicomQueryProvider<SeriesInfo>>();
            var seriesInfoProvider = new SeriesInfoProvider(queryProvider);

            // Act
            seriesInfoProvider.GetSeries();

            // Assert
            Assert.That(() => queryProvider.Received(1).GetDicomQuery(), Throws.Nothing);
        }


        [Test]
        public void GetSeriesByID_NullIdentifier_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesInfoProvider = new SeriesInfoProvider(mockProvider.GetDicomQueryProviderForSeriesFake());

            // Act
            var exception = seriesInfoProvider.GetSeriesByIDAsync(null).Exception;

            // Act + Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentNullException>());
        }


        [Test]
        public async void GetSeriesByIDAsync_FromCache_ProperQueryIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = mockProvider.GetDicomQueryProviderForSeriesFake();
            var dataLoader = Substitute.For<IDicomDataLoader<SeriesInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<SeriesInfo>>();
            var query = Substitute.For<DicomQuery<SeriesInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            var seriesInfoProvider = new SeriesInfoProvider(queryProvider);

            // Act
            await seriesInfoProvider.GetSeriesByIDAsync("1.2.3", true);

            // Assert
            Assert.That(() => dataLoader.Received(1).LoadDataFromCacheAsync(whereCollection), Throws.Nothing);
        }


        [Test]
        public async void GetSeriesByIDAsync_FromServer_ProperQueryIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = mockProvider.GetDicomQueryProviderForSeriesFake();
            var dataLoader = Substitute.For<IDicomDataLoader<SeriesInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<SeriesInfo>>();
            var query = Substitute.For<DicomQuery<SeriesInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            var seriesInfoProvider = new SeriesInfoProvider(queryProvider);

            // Act
            await seriesInfoProvider.GetSeriesByIDAsync("1.2.3");

            // Assert
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.SeriesInstanceUID, "1.2.3"), Throws.Nothing);
        }


        [Test]
        public void GetSeriesForStudy_NullStudyInfo_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesInfoProvider = new SeriesInfoProvider(mockProvider.GetDicomQueryProviderForSeriesFake());

            // Act + Assert
            Assert.That(() => seriesInfoProvider.GetSeriesForStudy(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void GetSeriesForStudy_PropertQueryIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var queryProvider = Substitute.For<IDicomQueryProvider<SeriesInfo>>();
            var dataLoader = Substitute.For<IDicomDataLoader<SeriesInfo>>();
            var whereCollection = Substitute.For<IWhereCollection<SeriesInfo>>();
            var query = Substitute.For<DicomQuery<SeriesInfo>>(mockProvider.GetGeneralizedInfoProviderFake(), dataLoader, whereCollection);
            queryProvider.GetDicomQuery().Returns(query);

            var study = new StudyInfo
            {
                StudyInstanceUID = new InfoIdentifier("1.2.3")
            };

            var seriesInfoProvider = new SeriesInfoProvider(queryProvider);

            // Act
            seriesInfoProvider.GetSeriesForStudy(study);

            // Assert
            Assert.That(() => whereCollection.Received(1).WhereEquals(DicomTags.StudyInstanceUID, "1.2.3"), Throws.Nothing);
        }
    }
}