using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomRequestFactoryTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomRequestFactory), null, new List<Func<object>>
            {
                mockProvider.GetSettingsProviderFake
            });
        }


        [Test]
        public void CreateDicomFindRequest_NullWhereCollection_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomRequestFactory = new DicomRequestFactory(mockProvider.GetSettingsProviderFake());

            // Act + Assert
            Assert.That(() => dicomRequestFactory.CreateDicomFindRequest<TestInfo>(null, item =>{}), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void CreateDicomFindRequest_NullResponseCallback_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomRequestFactory = new DicomRequestFactory(mockProvider.GetSettingsProviderFake());

            // Act + Assert
            Assert.That(() => dicomRequestFactory.CreateDicomFindRequest<TestInfo>(mockProvider.GetDicomWhereCollectionFake(), null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void CreateDicomFindRequest_CorrectRequestIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomRequestFactory = new DicomRequestFactory(mockProvider.GetSettingsProviderFake());

            var dicomWhereCollection = mockProvider.GetDicomWhereCollectionFake();
            Action<TestInfo> responseCallback = item => {};

            // Act
            var request = dicomRequestFactory.CreateDicomFindRequest(dicomWhereCollection, responseCallback);

            // Assert
            Assert.That(request.ResponseCallback, Is.SameAs(responseCallback));
            Assert.That(request.DicomWhereCollection, Is.SameAs(dicomWhereCollection));
        }


        [Test]
        public void CreateDicomMoveRequest_NullInfoIdentifier_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomRequestFactory = new DicomRequestFactory(mockProvider.GetSettingsProviderFake());

            // Act + Assert
            Assert.That(() => dicomRequestFactory.CreateDicomMoveRequest(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void CreateDicomMoveRequest_CorrectRequestIsCreated()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var settingsProvider = mockProvider.GetSettingsProviderFake();
            settingsProvider.DestinationApplicationEntity.Returns("DestinationAE");
            var identifier = new InfoIdentifier("identifier");

            var dicomRequestFactory = new DicomRequestFactory(settingsProvider);

            // Act
            var request = dicomRequestFactory.CreateDicomMoveRequest(identifier);

            // Assert
            Assert.That(request.DestinationAE, Is.EqualTo("DestinationAE"));
            Assert.That(request.Identifier, Is.SameAs(identifier));
        }
    }
}