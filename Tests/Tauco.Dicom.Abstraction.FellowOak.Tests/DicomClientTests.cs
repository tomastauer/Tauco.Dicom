using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NSubstitute;

using NUnit.Framework;

using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class DicomClientTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomClient<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetSettingsProviderFake,
                mockProvider.GetRequestAdapterFake
            });
        }


        [Test]
        public void AddFindRequest_NullDicomRequest_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Act + Assert
            Assert.That(() => dicomClient.AddFindRequest(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void AddFindRequest_RequestAdapterIsCalled()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FelloOakMockProvider();
            var requestAdapter = mockProvider.GetRequestAdapterFake();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), requestAdapter);
            var dicomFindRequest = new FellowOakDicomFindRequest<TestInfo>(mockProvider.GetDicomMappingFake(),fellowOakMockProvider.GetDicomTagAdapterFake(),mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetMappingEngine(),fellowOakMockProvider.GetDicomSopClassUidProviderFake(),c=> {},mockProvider.GetDicomWhereCollectionFake());

            // Act
            dicomClient.AddFindRequest(dicomFindRequest);

            // Assert
            Assert.That(() => requestAdapter.Received(1).CreateFindDicomRequest(dicomFindRequest), Throws.Nothing);
        }


        [Test]
        public void AddMoveRequest_NullDicomRequest_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();

            // Act
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Assert
            Assert.That(() => dicomClient.AddMoveRequest(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void AddMoveRequest_RequestAdapterIsCalled()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var requestAdapter = mockProvider.GetRequestAdapterFake();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), requestAdapter);
            var dicomMoveRequest = Substitute.For<IDicomMoveRequest>();

            // Act
            dicomClient.AddMoveRequest(dicomMoveRequest);

            // Assert
            Assert.That(() => requestAdapter.Received(1).CreateMoveDicomRequest(dicomMoveRequest), Throws.Nothing);
        }


        [Test]
        public void SendAsync_NullServerIP_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Act + Assert
            Assert.That(() => dicomClient.SendAsync(null, 0, "callingAE", "calledAE").Exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void SendAsync_NullCallingAE_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Act + Assert
            Assert.That(() => dicomClient.SendAsync("serverIP", 0, null, "calledAE").Exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void SendAsync_NullCAlledAE_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomClient = new DicomClient<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Act + Assert
            Assert.That(() => dicomClient.SendAsync("serverIP", 0, "callingAE", null).Exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
        }
    }
}