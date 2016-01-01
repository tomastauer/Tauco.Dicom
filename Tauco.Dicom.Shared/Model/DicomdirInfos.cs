using System.Collections.Generic;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents result object of dicomdir file parsing. Contains collections of all <see cref="PatientInfo"/>,
    /// <see cref="StudyInfo"/>, <see cref="SeriesInfo"/> and <see cref="ImageInfo"/> nested in the dicomdir file.
    /// </summary>
    public class DicomdirInfos
    {
        /// <summary>
        /// Gets or sets collection of all <see cref="PatientInfo"/> parsed from the dicomdir file.
        /// </summary>
        public IEnumerable<PatientInfo> Patients
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets collection of all <see cref="StudyInfo"/> parsed from the dicomdir file.
        /// </summary>
        public IEnumerable<StudyInfo> Studies
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets collection of all <see cref="SeriesInfo"/> parsed from the dicomdir file.
        /// </summary>
        public IEnumerable<SeriesInfo> Series
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets collection of all <see cref="ImageInfo"/> parsed from the dicomdir file.
        /// </summary>
        public IEnumerable<ImageInfo> Images
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets location of the original dicomdir file.
        /// </summary>
        public string OriginalDicomdirFileLocation
        {
            get;
            set;
        }
    }
}