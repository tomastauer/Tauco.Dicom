namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents single constraint item within the request constraint collection suitable for DICOM requests.
    /// </summary>
    public class DicomWhereItem
    {
        /// <summary>
        /// Specifies which DICOM tag is used in the constraint.
        /// </summary>
        public DicomTags DicomTag
        {
            get;
            set;
        }


        /// <summary>
        /// Specifies relevant value of the constraint in DICOM format.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
