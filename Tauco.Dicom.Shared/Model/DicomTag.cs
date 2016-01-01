namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents all available Dicom tags.
    /// </summary>
    public enum DicomTags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Undefined,

        /// <summary>
        /// Identifier of the patient (birth number).
        /// </summary>
        PatientID,

        /// <summary>
        /// Unique identifier of study.
        /// </summary>
        StudyInstanceUID,

        /// <summary>
        /// Unique identifier of series.
        /// </summary>
        SeriesInstanceUID,

        /// <summary>
        /// Name of the patient.
        /// </summary>
        PatientName,

        /// <summary>
        /// Specifies modality of the series (MR, CT, US).
        /// </summary>
        Modality,
        
        /// <summary>
        /// Unique identifier of image.
        /// </summary>
        ReferencedSOPInstanceUIDInFile,

        /// <summary>
        /// Specifies physical location of the image file.
        /// </summary>
        ReferencedFileID
    }
}
