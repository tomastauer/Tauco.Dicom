using System;

using Newtonsoft.Json;

using Tauco.Cache;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Represents single Dicom study object.
    /// </summary>
    public class StudyInfo : IDicomInfo
    {
        private InfoIdentifier mStudyInstanceUid;
        private bool mGetHashCodeCalled;


        /// <summary>
        /// Gets or sets patient identifier (birth number).
        /// </summary>
        [Dicom(DicomTags.PatientID)]
        public BirthNumber PatientID
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets study unique identifier.
        /// </summary>
        [Dicom(DicomTags.StudyInstanceUID)]
        [CacheIndex]
        public InfoIdentifier StudyInstanceUID
        {
            get
            {
                return mStudyInstanceUid;
            }
            set
            {
                if (mGetHashCodeCalled)
                {
                    throw new InvalidOperationException("Hash code for the object has already been obtained, cannot change the dependent properties");
                }
                mStudyInstanceUid = value;
            }
        }


        #region IDicomInfo implementation

        /// <summary>
        /// Represents type of the dicom object.
        /// </summary>
        [JsonIgnore]
        public DicomInfoType DicomType => DicomInfoType.Study;

        #endregion


        /// <summary>
        /// Compares with another <see cref="StudyInfo"/> info and returns whether their UID equals.
        /// </summary>
        /// <param name="other">Other patient info to be checked</param>
        /// <returns>True, if patient IDs are equals; otherwise, false</returns>
        protected bool Equals(StudyInfo other)
        {
            return Equals(StudyInstanceUID, other.StudyInstanceUID);
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
            return Equals((StudyInfo) obj);
        }


        /// <summary>
        /// Returns the hash code for the <see cref="StudyInfo"/>.
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

            return StudyInstanceUID?.GetHashCode() ?? 0;
        }
    }
}
