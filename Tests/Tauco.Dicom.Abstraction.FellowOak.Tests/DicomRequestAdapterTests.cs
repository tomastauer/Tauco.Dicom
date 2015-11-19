using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Dicom.Network;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomRequestAdapterTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FelloOakMockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomRequestAdapter<>), new [] { typeof(TestInfo) }, new List<Func<object>>
            {
                fellowOakMockProvider.GetDicomTagAdapterFake,
                mockProvider.GetGeneralizedInfoProviderFake,
                mockProvider.GetMappingEngine,
                fellowOakMockProvider.GetDicomSopClassUidProviderFake
            });
        }


        [Test]
        public void CreateFindDicomRequest_NullDicomFindRequest_ThrowsException()
        {
            // Arrange
            var dicomRequestAdapter = GetDicomRequestAdapter();

            // Act + Assert
            Assert.That(() => dicomRequestAdapter.CreateFindDicomRequest(null), Throws.Exception);
        }


        [Test]
        public void CreateFindDicomRequest_ReturnsDicomCFindRequest()
        {
            // Arrange
            var dicomRequestAdapter = GetDicomRequestAdapter();
            var findDicomRequest = Substitute.For<IDicomFindRequest<TestInfo>>();
            var dicomWhereCollection = Substitute.For<IDicomWhereCollection>();

            findDicomRequest.ResponseCallback.Returns(testInfo => {});
            findDicomRequest.DicomWhereCollection.Returns(dicomWhereCollection);

            // Act
            var result = dicomRequestAdapter.CreateFindDicomRequest(findDicomRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<DicomCFindRequest>());
        }


        [Test]
        public void CreateMoveDicomRequest_NullDicomMoveRequest_ThrowsException()
        {
            // Arrange
            var dicomRequestAdapter = GetDicomRequestAdapter();

            // Act + Assert
            Assert.That(() => dicomRequestAdapter.CreateMoveDicomRequest(null), Throws.Exception);
        }


        [Test]
        public void CreateMoveDicomRequest_ReturnsDicomCMoveRequest()
        {
            // Arrange
            var dicomRequestAdapter = GetDicomRequestAdapter();
            var moveDicomRequest = Substitute.For<IDicomMoveRequest>();
            moveDicomRequest.DestinationAE.Returns("destinationAE");
            moveDicomRequest.Identifier.Returns(new InfoIdentifier("identifier"));
            
            // Act
            var result = dicomRequestAdapter.CreateMoveDicomRequest(moveDicomRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<DicomCMoveRequest>());
        }


        private IDicomRequestAdapter<TestInfo> GetDicomRequestAdapter()
        {
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FelloOakMockProvider();

            return new DicomRequestAdapter<TestInfo>(fellowOakMockProvider.GetDicomTagAdapterFake(), mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetMappingEngine(),
                fellowOakMockProvider.GetDicomSopClassUidProviderFake());
        }
    }
}