namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Specifies single object that can be stored in the Dicom server.
    /// </summary>
    public interface IDicomInfo
    {
        /// <summary>
        /// Represents type of the dicom object.
        /// </summary>
        DicomInfoType DicomType
        {
            get;
        }
    }
}
