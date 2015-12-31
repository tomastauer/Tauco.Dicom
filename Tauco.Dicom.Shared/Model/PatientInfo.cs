using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Tauco.Cache;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents single Dicom patient object.
    /// </summary>
    public class PatientInfo : IDicomInfo, IMultipleInstance<PatientInfo>
    {
        private IList<PatientInfo> mAdditionalInstances;
        private bool mGetHashCodeCalled;
        private BirthNumber mPatientID;
        private PatientName mPatientName;

        /// <summary>
        /// Gets or sets patient identifier (birth number).
        /// </summary>
        [Dicom(DicomTags.PatientID)]
        [CacheIndex]
        public BirthNumber PatientID
        {
            get
            {
                return mPatientID;
            }
            set
            {
                if (mGetHashCodeCalled)
                {
                    throw new InvalidOperationException("Hash code for the object has already been obtained, cannot change the dependent properties");
                }
                mPatientID = value;
            }
        }


        /// <summary>
        /// Gets or sets patient name.
        /// </summary>
        [Dicom(DicomTags.PatientName)]
        public PatientName PatientName
        {
            get
            {
                return mPatientName;
            }
            set
            {
                if (mGetHashCodeCalled)
                {
                    throw new InvalidOperationException("Hash code for the object has already been obtained, cannot change the dependent properties");
                }
                mPatientName = value;
            }
        }


        #region IDicomInfo implementation

        /// <summary>
        /// Represents type of the dicom object.
        /// </summary>
        [JsonIgnore]
        public DicomInfoType DicomType => DicomInfoType.Patient;

        #endregion


        #region IMultipleInstance implementation

        /// <summary>
        /// Represents collection of another instances that should be considered the same as the original.
        /// </summary>
        [JsonIgnore]
        public IList<PatientInfo> AdditionalInstances => mAdditionalInstances ?? (mAdditionalInstances = new List<PatientInfo>());


        /// <summary>
        /// Gets hash code identifier of the <see cref="PatientID"/>.
        /// </summary>
        /// <returns>Hash code of the <see cref="PatientID"/></returns>
        public int GetIdentifierHashCode()
        {
            return PatientID?.GetHashCode() ?? 0;
        }

        #endregion


        /// <summary>
        /// Compares with another object and returns whether their ID equals.
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
            return Equals((PatientInfo) obj);
        }


        /// <summary>
        /// Compares with another <see cref="PatientInfo"/> info and returns whether their ID equals.
        /// </summary>
        /// <param name="other">Other patient info to be checked</param>
        /// <returns>True, if patient IDs are equals; otherwise, false</returns>
        protected bool Equals(PatientInfo other)
        {
            return Equals(PatientID, other.PatientID) && Equals(PatientName, other.PatientName);
        }


        /// <summary>
        /// Returns the hash code for the <see cref="PatientInfo"/>.
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

            unchecked
            {
                return ((PatientID?.GetHashCode() ?? 0) * 397) ^ (PatientName?.GetHashCode() ?? 0);
            }
        }
    }
}