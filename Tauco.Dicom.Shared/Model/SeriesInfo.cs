using System;

using Newtonsoft.Json;

using Tauco.Cache;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents single Dicom series.
    /// </summary>
    public class SeriesInfo : IDicomInfo
    {
        private InfoIdentifier mSeriesInstanceUid;
        private bool mGetHashCodeCalled;


        /// <summary>
        /// Gets or sets study unique identifier.
        /// </summary>
        [Dicom(DicomTags.StudyInstanceUID)]
        public InfoIdentifier StudyInstanceUID
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets series unique identifier.
        /// </summary>
        [Dicom(DicomTags.SeriesInstanceUID)]
        [CacheIndex]
        public InfoIdentifier SeriesInstanceUID
        {
            get
            {
                return mSeriesInstanceUid;
            }
            set
            {
                if (mGetHashCodeCalled)
                {
                    throw new InvalidOperationException("Hash code for the object has already been obtained, cannot change the dependent properties");
                }

                mSeriesInstanceUid = value;
            }
        }


        /// <summary>
        /// Gets or sets series modality. Most common values are MR, CT, US
        /// </summary>
        [Dicom(DicomTags.Modality)]
        public string Modality
        {
            get;
            set;
        }


        #region IDicomInfo implementation

        /// <summary>
        /// Represents type of the dicom object.
        /// </summary>
        [JsonIgnore]
        public DicomInfoType DicomType => DicomInfoType.Series;

        #endregion


        /// <summary>
        /// Compares with another <see cref="SeriesInfo"/> info and returns whether their UID equals.
        /// </summary>
        /// <param name="other">Other patient info to be checked</param>
        /// <returns>True, if patient IDs are equals; otherwise, false</returns>
        protected bool Equals(SeriesInfo other)
        {
            return Equals(SeriesInstanceUID, other.SeriesInstanceUID);
        }


        /// <summary>
        /// Compares with another object and returns whether their UID equals.
        /// </summary>
        /// <param name="obj">Other patient info to be checked</param>
        /// <returns>True, if patient IDs are equals; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((SeriesInfo)obj);
        }


        /// <summary>
        /// Returns the hash code for the <see cref="SeriesInfo"/>.
        /// </summary>
        /// <remarks>
        /// Since calling of this method sets up flag internally, it cannot be longer considered as pure.
        /// </remarks>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            mGetHashCodeCalled = true;

            return SeriesInstanceUID?.GetHashCode() ?? 0;
        }
    }
}
