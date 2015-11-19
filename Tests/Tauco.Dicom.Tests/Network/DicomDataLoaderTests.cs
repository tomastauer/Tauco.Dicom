using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class DicomDataLoaderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomDataLoader<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetCacheProviderFake,
                mockProvider.GetCacheIndexProviderFake,
                mockProvider.GetDicomClientFactoryFake,
                mockProvider.GetDicomRequestFactoryFake
            });
        }


        [Test]
        public void LoadDataFromServer_NullWhereCondition_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dataLoader = mockProvider.GetDicomDataLoader();

            // Act + Assert
            Assert.That(() => dataLoader.LoadDataFromServer(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void LoadDataFromServer_WhereCollection_CreatesRequest()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var requestFactory = new DicomRequestFactoryFake();
            var dataLoader = new DicomDataLoader<TestInfo>(mockProvider.GetCacheProviderFake(), mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), requestFactory);
            var whereCollection = new WhereCollection<TestInfo>(mockProvider.GetDicomMappingFake());
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            dataLoader.LoadDataFromServer(whereCollection);

            var whereItem = ((IDicomWhereCollection)requestFactory.WhereCollection).Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo("1"));
        }


        [Test]
        public void LoadDataFromServer_GetResponse_StoresInCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var cacheProvider = mockProvider.GetCacheProviderFake();
            var dataLoader = new DicomDataLoader<TestInfo>(cacheProvider, mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), mockProvider.GetDicomRequestFactoryFake());

            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            var result = dataLoader.LoadDataFromServer(whereCollection);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.All(c => c.PatientID == 1), Is.True);
            Assert.That(() => cacheProvider.Received(1).Store(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<bool>()), Throws.Nothing);
        }


        [Test]
        public void LoadDataFromServer_AllCombinationsAreAddedAsRequest()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomClientFactory = mockProvider.GetDicomClientFactoryFake();
            var dicomClient = mockProvider.GetDicomClientFake();
            var dataLoader = new DicomDataLoader<TestInfo>(mockProvider.GetCacheProviderFake(), mockProvider.GetCacheIndexProviderFake(), dicomClientFactory, mockProvider.GetDicomRequestFactoryFake());

            dicomClientFactory.CreateDicomClient().Returns(dicomClient);

            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientID, 1);
            whereCollection.WhereEquals(DicomTags.PatientID, 2);

            whereCollection.WhereEquals(DicomTags.PatientName, "test");
            whereCollection.WhereLike(DicomTags.PatientName, "test1");

            // Act
            dataLoader.LoadDataFromServer(whereCollection);

            // Assert
            Assert.That(() => dicomClient.Received(4).AddFindRequest(Arg.Any<IDicomFindRequest<TestInfo>>()), Throws.Nothing);
        }


        [Test]
        public void LoadDataFromServerAsync_NullWhereCondition_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dataLoader = mockProvider.GetDicomDataLoader();

            // Act
            var exception = dataLoader.LoadDataFromServerAsync(null).Exception;

            // Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentNullException>());
        }


        [Test]
        public async void LoadDataFromServerAsync_WhereCollection_CreatesRequest()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var requestFactory = new DicomRequestFactoryFake();
            var dataLoader = new DicomDataLoader<TestInfo>(mockProvider.GetCacheProviderFake(), mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), requestFactory);
            var whereCollection = new WhereCollection<TestInfo>(mockProvider.GetDicomMappingFake());
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            await dataLoader.LoadDataFromServerAsync(whereCollection);
            var whereItem = ((IDicomWhereCollection)requestFactory.WhereCollection).Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo("1"));
        }


        [Test]
        public async void LoadDataFromServerAsync_GetResponse_StoresInCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var cacheProvider = mockProvider.GetCacheProviderFake();
            var dataLoader = new DicomDataLoader<TestInfo>(cacheProvider, mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), mockProvider.GetDicomRequestFactoryFake());

            var whereCollection = new WhereCollection<TestInfo>(mockProvider.GetDicomMappingFake());
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            var result = await dataLoader.LoadDataFromServerAsync(whereCollection);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.All(c=>c.PatientID == 1), Is.True);
            Assert.That(() => cacheProvider.Received(1).Store(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<bool>()), Throws.Nothing);
        }


        [Test]
        public void LoadDataFromCache_NullWhereCondition_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dataLoader = mockProvider.GetDicomDataLoader();

            // Act + Assert
            Assert.That(() => dataLoader.LoadDataFromCache(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void LoadDataFromCache_ReturnsFromCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var cacheProvider = mockProvider.GetCacheProviderFake();
            var dataLoader = new DicomDataLoader<TestInfo>(cacheProvider, mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), mockProvider.GetDicomRequestFactoryFake());
            var whereCollection = new WhereCollection<TestInfo>(mockProvider.GetDicomMappingFake());
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            var result = dataLoader.LoadDataFromCache(whereCollection).Single();

            // Assert
            Assert.That(result.PatientID, Is.EqualTo(1));
            Assert.That(() => cacheProvider.Received(1).Retrieve<TestInfo>(), Throws.Nothing);
        }


        [Test]
        public void LoadDataFromCacheAsync_NullWhereCondition_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dataLoader = mockProvider.GetDicomDataLoader();

            // Act + Assert
            Assert.That(() => dataLoader.LoadDataFromCacheAsync(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public async void LoadDataFromCacheAsync_ReturnsFromCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var cacheProvider = mockProvider.GetCacheProviderFake();
            var dataLoader = new DicomDataLoader<TestInfo>(cacheProvider, mockProvider.GetCacheIndexProviderFake(), mockProvider.GetDicomClientFactoryFake(), mockProvider.GetDicomRequestFactoryFake());
            var whereCollection = new WhereCollection<TestInfo>(mockProvider.GetDicomMappingFake());
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            var result = (await dataLoader.LoadDataFromCacheAsync(whereCollection)).Single();

            // Assert
            Assert.That(result.PatientID, Is.EqualTo(1));
            Assert.That(() => cacheProvider.Received(1).Retrieve<TestInfo>(), Throws.Nothing);
        }
    }
}
