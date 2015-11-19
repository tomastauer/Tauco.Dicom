using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Castle.Core.Logging;

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
    public class DicomStoreTests
    {
        [TearDown]
        public void TearDown()
        {
            if (File.Exists("test.dcm"))
            {
                File.Delete("test.dcm");
            }
        }


        [Test]
        public void Constructor_NullDownloadAction_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();

            // Act + Assert
            Assert.That(() => new DicomStore(new MemoryStream(), null, mockProvider.GetLoggerFake()), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Constructor_NullLogger_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();

            // Act + Assert
            Assert.That(() => new DicomStore(new MemoryStream(), (a, b) => new MemoryStream(), null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void OnReceiveAssociationRequest_NullAssociation_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomStore = GetDicomStore(mockProvider.GetLoggerFake());
           
            // Act + Assert
            Assert.That(() => dicomStore.OnReceiveAssociationRequest(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void OnReceiveAbort_LogsInfo()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var loggerFake = mockProvider.GetLoggerFake();
            var dicomStore = GetDicomStore(loggerFake);

            // Act
            dicomStore.OnReceiveAbort(DicomAbortSource.Unknown, DicomAbortReason.NotSpecified);

            // Assert
            Assert.That(() => loggerFake.Received().Info(Arg.Any<string>()), Throws.Nothing);
        }


        [Test]
        public void OnReceiveAssociationReleaseRequest_LogsInfo()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var loggerFake = mockProvider.GetLoggerFake();
            var dicomStore = GetDicomStore(loggerFake);

            // Act
            dicomStore.OnReceiveAssociationReleaseRequest();

            // Assert
            Assert.That(() => loggerFake.Received().Info(Arg.Any<string>()), Throws.Nothing);
        }


        [Test]
        public void OnConnectionClosed_ExceptionThrown_LogsError()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var loggerFake = mockProvider.GetLoggerFake();
            var dicomStore = GetDicomStore(loggerFake);
            var exception = new Exception("Test");

            // Act
            dicomStore.OnConnectionClosed(exception);

            // Assert
            Assert.That(() => loggerFake.Received().Error(Arg.Any<string>(), exception), Throws.Nothing);
        }


        [Test]
        public void OnConnectionClosed_WithoutException_LogsInfo()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var loggerFake = mockProvider.GetLoggerFake();
            var dicomStore = GetDicomStore(loggerFake);
            
            // Act
            dicomStore.OnConnectionClosed(null);

            // Assert
            Assert.That(() => loggerFake.Received().Info(Arg.Any<string>()), Throws.Nothing);
        }


        [Test]
        public void OnCStoreRequest_SavesFileToStream_ReturnsCStoreResponse()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var stream = File.Create("test.dcm");
            var action = new Func<InfoIdentifier, InfoIdentifier, Stream>((a, b) => stream);

            var dicomStore = new DicomStore(new MemoryStream(), action, mockProvider.GetLoggerFake());
            
            // Act
            var response = dicomStore.OnCStoreRequest(new DicomCStoreRequest("Assets/testImage.dcm"));
            
            // Assert
            Assert.That(response.Status, Is.EqualTo(DicomStatus.Success));
            Assert.That(File.Exists("test.dcm"));
            Assert.That(new FileInfo("test.dcm").Length, Is.GreaterThan(0));
        }


        [Test]
        public void OnCStoreRequestException_LogsError()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var loggerFake = mockProvider.GetLoggerFake();
            var dicomStore = GetDicomStore(loggerFake);
            var exception = new Exception("Test");

            // Act
            dicomStore.OnCStoreRequestException(null, exception);

            // Assert
            Assert.That(() => loggerFake.Received(1).Error(Arg.Any<string>(), exception), Throws.Nothing);
        }


        [Test]
        public void OnCEchoRequest_ReturnsCEchoResponse()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomStore = GetDicomStore(mockProvider.GetLoggerFake());

            // Act
            var response = dicomStore.OnCEchoRequest(new DicomCEchoRequest());

            // Assert
            Assert.That(response.Status, Is.EqualTo(DicomStatus.Success));
        }


        private DicomStore GetDicomStore(ILogger logger)
        {
            var mockProvider = new MockProvider();
            return new DicomStore(new MemoryStream(), (a, b) => new MemoryStream(), logger);
        }
    }
}
