using System.Diagnostics.CodeAnalysis;
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
    public class SeriesInfoProviderTests
    {
        private ISeriesInfoProvider mSeriesInfoProvider;
        private StudyInfo mStudy;

        [SetUp]
        public void SetUp()
        {
            var container = new WindsorContainer().Install(new CommonInstaller());
            mSeriesInfoProvider = container.Resolve<ISeriesInfoProvider>();
            var studyInfoProvider = container.Resolve<IStudyInfoProvider>();

            mStudy = studyInfoProvider.GetStudyByIDAsync("1.2.276.0.7230010.3.1.2.655267989.4252.1447498448.838").Result;
        }

        [Test]
        public void GetSeries_SyncFromServer_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries();

            // Act
            var series = seriesQuery.ToList();

            // Assert
            Assert.That(series.Count, Is.EqualTo(8));
        }



        [Test]
        public async void GetSeries_AsyncFromServer_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries();

            // Act
            var series = await seriesQuery.ToListAsync();

            // Assert
            Assert.That(series.Count, Is.EqualTo(8));
        }


        [Test]
        public void GetSeries_SyncFromCache_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries();
            // Fill cache
            seriesQuery.ToList();

            // Act
            var series = seriesQuery.LoadFromCache().ToList();

            // Assert
            Assert.That(series.Count, Is.EqualTo(8));
        }
        

        [Test]
        public async void GetSeries_AsyncFromCache_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries();
            // Fill cache
            await seriesQuery.ToListAsync();
            
            // Act
            var series = await seriesQuery.LoadFromCache().ToListAsync();

            // Assert
            Assert.That(series.Count, Is.EqualTo(8));
        }


        [Test]
        public void GetSeries_SpecificSeriesFromServer_Equals_ReturnsCorrectSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries().WhereEquals(DicomTags.SeriesInstanceUID, "1.2.276.0.7230010.3.1.3.655267989.160.1445185438.280");

            // Act + Assert
            Assert.That(() => seriesQuery.Single(), Throws.Nothing);
        }


        [Test]
        public void GetSeries_SpecificSeriesFromCache_Equals_ReturnsCorrectSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries().WhereEquals(DicomTags.SeriesInstanceUID, "1.2.276.0.7230010.3.1.3.655267989.160.1445185438.280");

            // Fill cache
            mSeriesInfoProvider.GetSeries().ToList();

            // Act + Assert
            Assert.That(() => seriesQuery.LoadFromCache().Single(), Throws.Nothing);
        }

        [Test]
        public void GetSeries_SpecificSeriesFromServer_Like_ReturnsCorrectSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries().WhereLike(DicomTags.SeriesInstanceUID, "1445185438.280");

            // Act + Assert
            Assert.That(() => seriesQuery.Single(), Throws.Nothing);
        }


        [Test]
        public void GetSeries_SpecificSeriesFromCache_Like_ReturnsCorrectSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeries().WhereLike(DicomTags.SeriesInstanceUID, "1445185438.280");

            // Fill cache
            mSeriesInfoProvider.GetSeries().ToList();

            // Act + Assert
            Assert.That(() => seriesQuery.LoadFromCache().Single(), Throws.Nothing);
        }


        [Test]
        public void GetSeriesForStudy_FromServer_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeriesForStudy(mStudy);

            // Act
            var series = seriesQuery.ToList();

            // Assert
            Assert.That(series.Count, Is.EqualTo(1));
        }


        [Test]
        public void GetSeriesForStudy_FromCache_ReturnsAllSeries()
        {
            // Arrange
            var seriesQuery = mSeriesInfoProvider.GetSeriesForStudy(mStudy);
            // Fill cache
            seriesQuery.ToList();

            // Act
            var series = seriesQuery.LoadFromCache().ToList();

            // Assert
            Assert.That(series.Count, Is.EqualTo(1));
        }


        [Test]
        public async void GetSeriesByIDAsync_FromServer_ReturnsCorrectSeries()
        {
            // Arrange + Act
            var series = await mSeriesInfoProvider.GetSeriesByIDAsync("1.2.276.0.7230010.3.1.3.655267989.160.1445185438.280");

            // Assert
            Assert.That(series, Is.Not.Null);
        }


        [Test]
        public async void GetSeriesByIDAsync_NonExisting_FromServer_ReturnsCorrectSeries()
        {
            // Arrange + Act
            var series = await mSeriesInfoProvider.GetSeriesByIDAsync("666");

            // Assert
            Assert.That(series, Is.Null);
        }


        [Test]
        public async void GetSeriesByIDAsync_FromCache_ReturnsCorrectSeries()
        {
            // Arrange
            // Fill cache
            await mSeriesInfoProvider.GetSeries().ToListAsync();

            // Act
            var series = await mSeriesInfoProvider.GetSeriesByIDAsync("1.2.276.0.7230010.3.1.3.655267989.160.1445185438.280", true);

            // Assert
            Assert.That(series, Is.Not.Null);
        }


        [Test]
        public async void GetSeriesByIDAsync_NonExisting_FromCache_ReturnsCorrectSeries()
        {
            // Arrange
            // Fill cache
            await mSeriesInfoProvider.GetSeries().ToListAsync();

            // Act
            var series = await mSeriesInfoProvider.GetSeriesByIDAsync("666", true);

            // Assert
            Assert.That(series, Is.Null);
        }
    }
}
