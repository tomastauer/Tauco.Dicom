using System;
using System.IO.Abstractions;
using System.Linq;

using dcmdir2dcm.Lib;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.IO
{
    /// <summary>
    /// Provides method for extracting all the images from the parsed dicomdir file and their composition to the single dicom image file.
    /// </summary>
    public class DicomdirImageStorageProvider : IDicomdirImageStorageProvider
    {   
        /// <summary>
        /// Specifies directory where all the dicom images will be stored.
        /// </summary>
        internal const string IMAGE_STORE_LOCATION = "ImageStore";

        private readonly IFileSystem mFileSystem;
        private readonly IDicomImageComposer mDicomImageComposer;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomdirImageStorageProvider"/>.
        /// </summary>
        /// <param name="fileSystem">IO file system</param>
        /// <param name="dicomImageComposer"></param>
        /// <exception cref="ArgumentNullException"><paramref name="fileSystem"/> is null -or- <paramref name="dicomImageComposer"/> is null</exception>
        public DicomdirImageStorageProvider([NotNull] IFileSystem fileSystem, [NotNull] IDicomImageComposer dicomImageComposer)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }
            if (dicomImageComposer == null)
            {
                throw new ArgumentNullException(nameof(dicomImageComposer));
            }

            mFileSystem = fileSystem;
            mDicomImageComposer = dicomImageComposer;
        }


        /// <summary>
        /// Stores all the images obtained from the given parsed <paramref name="dicomdirInfos"/>.
        /// </summary>
        /// <remarks>
        /// All images from every series are stored as single multiframe dicom image file using series UIDs as their name.
        /// </remarks>
        /// <param name="dicomdirInfos">Result of the dicomdir file parsing</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomdirInfos"/> is null</exception>
        public void StoreImagesFromDicomdir([NotNull] DicomdirInfos dicomdirInfos)
        {
            if (dicomdirInfos == null)
            {
                throw new ArgumentNullException(nameof(dicomdirInfos));
            }

            string directoryName = mFileSystem.Path.Combine(mFileSystem.Directory.GetCurrentDirectory(), IMAGE_STORE_LOCATION);

            var imageStoreDirectory = mFileSystem.DirectoryInfo.FromDirectoryName(directoryName);
            if (!imageStoreDirectory.Exists)
            {
                imageStoreDirectory.Create();
            }

            foreach (var series in dicomdirInfos.Images.GroupBy(image => image.SeriesInstanceUID))
            {
                var fileInfos = series.Select(image => mFileSystem.Path.Combine(mFileSystem.Path.GetDirectoryName(dicomdirInfos.OriginalDicomdirFileLocation), image.ReferencedFileID));
                mDicomImageComposer.Compose(fileInfos, mFileSystem.Path.Combine(imageStoreDirectory.FullName, $"{series.Key}.dcm"));
            }
        }
    }
}
