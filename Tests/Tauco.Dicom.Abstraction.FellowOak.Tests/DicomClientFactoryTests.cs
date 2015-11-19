using System;
using System.Collections.Generic;

using NUnit.Framework;

using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    public class DicomClientFactoryTests
    {

        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomClientFactory<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetSettingsProviderFake,
                mockProvider.GetRequestAdapterFake
            });
        }


        [Test]
        public void CreateDicomClient_CreatesNewInstanceOfDicomClient()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var clientFactory = new DicomClientFactory<TestInfo>(mockProvider.GetSettingsProviderFake(), mockProvider.GetRequestAdapterFake());

            // Act
            var client1 = clientFactory.CreateDicomClient();
            var client2 = clientFactory.CreateDicomClient();

            // Assert
            Assert.That(client1, Is.Not.SameAs(client2));
        }
    }
}
