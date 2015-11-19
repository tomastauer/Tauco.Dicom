using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Castle.Core.Logging;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomServerTests
    {
        [Test]
        public void Constructor_NullDownloadAction_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();

            // Act + Assert
            Assert.That(() => new DicomServer<DicomStore>(104, null, mockProvider.GetLoggerFake()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Constructor_NullLogger_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new DicomServer<DicomStore>(105, (a, b) => new MemoryStream(), null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void CreateScp_CreatesNewInstanceOfDicomStore()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomServer = new DicomServerFake(106, (a, b) => new MemoryStream(), mockProvider.GetLoggerFake());
            
            // Act
            var result = dicomServer.CreateStore(new MemoryStream());

            // Assert
            Assert.That(result, Is.InstanceOf<DicomStore>());
        }


        private class DicomServerFake : DicomServer<DicomStore>
        {
            public DicomServerFake(int port, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction, ILogger logger) 
                : base(port, downloadAction, logger)
            {
            }


            public DicomStore CreateStore(Stream stream)
            {
                return CreateScp(stream);
            }
        }
    }
}
