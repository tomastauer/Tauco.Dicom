using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides method for obtaining third party DICOM tag representation from the <see cref="DicomTags"/>.
    /// </summary>
    public interface IDicomTagAdapter
    {
        /// <summary>
        /// Gets DICOM tag representation instance from the given <paramref name="dicomTag"/>.
        /// </summary>
        /// <param name="dicomTag">Dicom tag to be converted to the dicom tag</param>
        /// <returns>Instance of dicom tag corresponding to the given <paramref name="dicomTag"/></returns>
        object GetDicomTag(DicomTags dicomTag);
    }
}