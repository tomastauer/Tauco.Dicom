using Tauco.Dicom.Shared;

namespace Tauco.Dicom.IO
{
    /// <summary>
    /// Provides method for extracting all the images from the parsed dicomdir file and their composition to the single dicom image file.
    /// </summary>
    public interface IDicomdirImageStorageProvider
    {
        /// <summary>
        /// Stores all the images obtained from the given parsed <paramref name="dicomdirInfos"/>.
        /// </summary>
        /// <remarks>
        /// All images from every series are stored as single multiframe dicom image file using series UIDs as their name.
        /// </remarks>
        /// <param name="dicomdirInfos">Result of the dicomdir file parsing</param>
        void StoreImagesFromDicomdir(DicomdirInfos dicomdirInfos);
    }
}