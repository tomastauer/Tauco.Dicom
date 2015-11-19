using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using NUnit.Framework;

using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomServerFactoryTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomServerFactory), null, new List<Func<object>>
            {
                mockProvider.GetSettingsProviderFake,
                mockProvider.GetLoggerFake
            });
        }


        [Test]
        public void CreateDicomServer_NullDownloadAction_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = new DicomServerFactory(mockProvider.GetSettingsProviderFake(), mockProvider.GetLoggerFake());

            // Act + Assert
            Assert.That(() => serverFactory.CreateDicomServer(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void CreateDicomServer_CreatesNewInstanceOfDicomServer()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = new DicomServerFactory(mockProvider.GetSettingsProviderFake(), mockProvider.GetLoggerFake());

            // Act
            var server1 = serverFactory.CreateDicomServer((a, b) => new MemoryStream());
            var server2 = serverFactory.CreateDicomServer((a, b) => new MemoryStream());

            // Assert
            Assert.That(server1, Is.Not.SameAs(server2));
        }
    }
}
