using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections.Generic;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class DicomQueryTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomQuery<>), new[]{ typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetGeneralizedInfoProviderFake,
                mockProvider.GetDicomDataLoaderFake,
                mockProvider.GetWhereCollectionFake
            });
        }


        [Test]
        public void LoadFromCache_GetEnumerator_LoadsFromCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomDataLoader = mockProvider.GetDicomDataLoaderFake();
            var dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), dicomDataLoader, mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery.LoadFromServer().LoadFromCache().ToList();
            
            // Assert
            Assert.That(() => dicomDataLoader.Received(1).LoadDataFromCache(Arg.Any<IWhereCollection<TestInfo>>()), Throws.Nothing);
        }


        [Test]
        public void LoadFromServer_GetEnumerator_LoadsFromServer()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomDataLoader = mockProvider.GetDicomDataLoaderFake();
            var dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), dicomDataLoader, mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery.LoadFromCache().LoadFromServer().ToList();

            // Assert
            Assert.That(() => dicomDataLoader.Received(1).LoadDataFromServer(Arg.Any<IWhereCollection<TestInfo>>()), Throws.Nothing);
        }
        

        [Test]
        public async void LoadFromCache_ToListAsync_LoadsFromCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomDataLoader = mockProvider.GetDicomDataLoaderFake();
            var dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), dicomDataLoader, mockProvider.GetWhereCollectionFake());

            // Act
            await dicomQuery.LoadFromServer().LoadFromCache().ToListAsync();

            // Assert
            Assert.That(() => dicomDataLoader.Received(1).LoadDataFromCacheAsync(Arg.Any<IWhereCollection<TestInfo>>()), Throws.Nothing);
        }


        [Test]
        public async void LoadFromServer_ToListAsync_LoadsFromServer()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomDataLoader = mockProvider.GetDicomDataLoaderFake();
            var dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), dicomDataLoader, mockProvider.GetWhereCollectionFake());

            // Act
            await dicomQuery.LoadFromCache().LoadFromServer().ToListAsync();

            // Assert
            Assert.That(() => dicomDataLoader.Received(1).LoadDataFromServerAsync(Arg.Any<IWhereCollection<TestInfo>>()), Throws.Nothing);
        }
    }
}