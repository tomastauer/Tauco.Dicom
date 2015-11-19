using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomDownloaderTests
    {
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists("DICOM"))
            {
                Directory.Delete("DICOM", true);
            }
        }

        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomDownloader<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetDicomServerFactoryFake,
                mockProvider.GetDicomClientFactoryFake,
                mockProvider.GetDicomRequestFactoryFake
            });
        }


        [Test]
        public void DownloadAsync_NullIdentifier_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = mockProvider.GetDicomServerFactoryFake();
            var clientFactory = mockProvider.GetDicomClientFactoryFake();
            var requestFactory = mockProvider.GetDicomRequestFactoryFake();

            var dicomDownloader = new DicomDownloader<TestInfo>(serverFactory, clientFactory, requestFactory);
            
            // Act + Assert
            Assert.That(dicomDownloader.DownloadAsync(null).Exception?.InnerExceptions[0], Is.TypeOf<ArgumentNullException>());
        }


        [Test]
        public async void DownloadAsync_NullDownloadAction_DefaultActionIsUsed()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = mockProvider.GetDicomServerFactoryFake();
            var clientFactory = mockProvider.GetDicomClientFactoryFake();
            var requestFactory = mockProvider.GetDicomRequestFactoryFake();

            var dicomDownloader = new DicomDownloader<TestInfo>(serverFactory, clientFactory, requestFactory);
            var identifier = new InfoIdentifier("identifier");

            serverFactory.When(c=>c.CreateDicomServer(Arg.Any<Func<InfoIdentifier, InfoIdentifier, Stream>>())).Do(c =>
            {
                var input = c.Arg<Func<InfoIdentifier, InfoIdentifier, Stream>>();
                input(identifier, new InfoIdentifier("test")).Dispose();
            });

            // Act
            await dicomDownloader.DownloadAsync(identifier);

            // Assert
            Assert.That(File.Exists("./DICOM/identifier/test.dcm"), Is.True);
        }


        [Test]
        public async void DownloadAsync_MoveRequestIsAddedAndSent()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = mockProvider.GetDicomServerFactoryFake();
            var clientFactory = mockProvider.GetDicomClientFactoryFake();
            var requestFactory = mockProvider.GetDicomRequestFactoryFake();

            var client = Substitute.For<IDicomClient<TestInfo>>();
            clientFactory.CreateDicomClient().Returns(client);

            var moveRequest = Substitute.For<IDicomMoveRequest>();
            requestFactory.CreateDicomMoveRequest(Arg.Any<InfoIdentifier>()).Returns(moveRequest);

            var dicomDownloader = new DicomDownloader<TestInfo>(serverFactory, clientFactory, requestFactory);
            var identifier = new InfoIdentifier("identifier");
            
            // Act
            await dicomDownloader.DownloadAsync(identifier, GetDownloadActionFake());

            // Assert
            Assert.That(() => requestFactory.Received(1).CreateDicomMoveRequest(identifier), Throws.Nothing);
            Assert.That(() => client.Received(1).AddMoveRequest(moveRequest), Throws.Nothing);
            Assert.That(() => client.Received(1).SendAsync(), Throws.Nothing);
        }


        [Test]
        public void DownloadAsync_SeveralRequest_OnlyOneServerIsRunning()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var serverFactory = mockProvider.GetDicomServerFactoryFake();
            var clientFactory = mockProvider.GetDicomClientFactoryFake();
            var requestFactory = mockProvider.GetDicomRequestFactoryFake();

            var client = Substitute.For<IDicomClient<TestInfo>>();
            clientFactory.CreateDicomClient().Returns(client);

            client.When(c=>c.SendAsync()).Do(c=> Thread.Sleep(100));

            var dicomDownloader = new DicomDownloader<TestInfo>(serverFactory, clientFactory, requestFactory);
            var identifier = new InfoIdentifier("identifier");

            // Act
            Task.WaitAll(
                Task.Factory.StartNew(() => dicomDownloader.DownloadAsync(identifier, GetDownloadActionFake())),
                Task.Factory.StartNew(() => dicomDownloader.DownloadAsync(identifier, GetDownloadActionFake())),
                Task.Factory.StartNew(() => dicomDownloader.DownloadAsync(identifier, GetDownloadActionFake()))
            );
            
            // Assert
            Assert.That(() => serverFactory.Received(1).CreateDicomServer(Arg.Any<Func<InfoIdentifier, InfoIdentifier, Stream>>()), Throws.Nothing);
       }


        private Func<InfoIdentifier, InfoIdentifier, Stream> GetDownloadActionFake()
        {
            return (x, y) => new MemoryStream();
        } 

    }
}
