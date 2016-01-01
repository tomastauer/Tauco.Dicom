using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;

using dcmdir2dcm.Lib;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.IO;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomdirImageStorageProviderTests
    {
        [Test]
        public void DicomdirImageStorageProvider_NullFileSystem_ThrowsException()
        {
            // Arrange
            var dicomImageComposer = Substitute.For<IDicomImageComposer>();

            // Act + Assert
            Assert.That(() => new DicomdirImageStorageProvider(null, dicomImageComposer), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void DicomdirImageStorageProvider_NullDicomImageComposer_ThrowsException()
        {
            // Arrange
            var fileSystem = Substitute.For<IFileSystem>();

            // Act + Assert
            Assert.That(() => new DicomdirImageStorageProvider(fileSystem, null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void StoreImagesFromDicomdir_CreatesImageStoreDirectory()
        {
            // Arrange
            var fileSystem = Substitute.For<IFileSystem>();
            var defaultFileSystem = new FileSystem();
            fileSystem.Path.Returns(defaultFileSystem.Path);

            var directoryInfoBase = Substitute.For<DirectoryInfoBase>();
            directoryInfoBase.Exists.Returns(false);

            var directoryInfoFactory = Substitute.For<IDirectoryInfoFactory>();
            directoryInfoFactory.FromDirectoryName(Arg.Any<string>()).Returns(directoryInfoBase);
            fileSystem.DirectoryInfo.Returns(directoryInfoFactory);

            var directoryBase = Substitute.For<DirectoryBase>();
            directoryBase.GetCurrentDirectory().Returns(string.Empty);
            fileSystem.Directory.Returns(directoryBase);

            var dicomImageComposer = Substitute.For<IDicomImageComposer>();
            
            var dicomdirImageStorageProvider = new DicomdirImageStorageProvider(fileSystem, dicomImageComposer);
            var dicomdirInfos = new DicomdirInfos
            {
                Images = Enumerable.Empty<ImageInfo>()
            };

            // Act
            dicomdirImageStorageProvider.StoreImagesFromDicomdir(dicomdirInfos);

            // Assert
            Assert.That(() => directoryInfoFactory.Received(1).FromDirectoryName(DicomdirImageStorageProvider.IMAGE_STORE_LOCATION), Throws.Nothing);
            Assert.That(() => directoryInfoBase.Received(1).Exists, Throws.Nothing);
            Assert.That(() => directoryInfoBase.Received(1).Create(), Throws.Nothing);
        }


        [Test]
        public void StoreImagesFromDicomdir_ComposerIsCalled()
        {
            // Arrange
            var fileSystem = Substitute.For<IFileSystem>();
            var dicomImageComposer = Substitute.For<IDicomImageComposer>();
            var defaultFileSystem = new FileSystem();
            fileSystem.Path.Returns(defaultFileSystem.Path);

            var dicomdirImageStorageProvider = new DicomdirImageStorageProvider(fileSystem, dicomImageComposer);
            var dicomdirInfos = new DicomdirInfos
            {
                Images = new List<ImageInfo>
                {
                    new ImageInfo
                    {
                        SeriesInstanceUID = new InfoIdentifier("1.2"),
                        ReferencedFileID = "1"
                    },
                    new ImageInfo
                    {
                        SeriesInstanceUID = new InfoIdentifier("1.2"),
                        ReferencedFileID = "2"
                    },
                    new ImageInfo
                    {
                        SeriesInstanceUID = new InfoIdentifier("1.3"),
                        ReferencedFileID = "1"
                    },
                    new ImageInfo
                    {
                        SeriesInstanceUID = new InfoIdentifier("1.3"),
                        ReferencedFileID = "2"
                    }
                },
                OriginalDicomdirFileLocation = string.Empty
            };

            // Act
            dicomdirImageStorageProvider.StoreImagesFromDicomdir(dicomdirInfos);

            // Assert
            Assert.That(() => dicomImageComposer.Received(1).Compose(Arg.Any<IEnumerable<string>>(), "1.2.dcm"), Throws.Nothing);
            Assert.That(() => dicomImageComposer.Received(1).Compose(Arg.Any<IEnumerable<string>>(), "1.3.dcm"), Throws.Nothing);
        }
    }
}
