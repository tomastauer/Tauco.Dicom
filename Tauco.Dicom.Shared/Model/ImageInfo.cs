using System;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Represents single Dicom image.
    /// </summary>
    public class ImageInfo : IDicomInfo
    {
        private InfoIdentifier mReferencedSopInstanceUidInFile;
        private bool mGetHashCodeCalled;
        
        /// <summary>
        /// Gets or sets image unique identifier.
        /// </summary>
        [Dicom(DicomTags.ReferencedSOPInstanceUIDInFile)]
        public InfoIdentifier ReferencedSOPInstanceUIDInFile
        {
            get
            {
                return mReferencedSopInstanceUidInFile;
            }
            set
            {
                if (mGetHashCodeCalled)
                {
                    throw new InvalidOperationException("Hash code for the object has already been obtained, cannot change the dependent properties");
                }

                mReferencedSopInstanceUidInFile = value;
            }
        }


        /// <summary>
        /// Gets or sets series unique identifier.
        /// </summary>
        [Dicom(DicomTags.SeriesInstanceUID)]
        public InfoIdentifier SeriesInstanceUID
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets physical location of the image file.
        /// </summary>
        [Dicom(DicomTags.ReferencedFileID)]
        public string ReferencedFileID
        {
            get;
            set;
        }
        
        #region IDicomInfo implementation

        /// <summary>
        /// Represents type of the dicom object.
        /// </summary>
        public DicomInfoType DicomType => DicomInfoType.Image;

        #endregion
        
        /// <summary>
        /// Compares with another <see cref="ImageInfo"/> info and returns whether their UID equals.
        /// </summary>
        /// <param name="other">Other patient info to be checked</param>
        /// <returns>True, if patient IDs are equals; otherwise, false</returns>
        protected bool Equals(ImageInfo other)
        {
            return Equals(ReferencedSOPInstanceUIDInFile, other.ReferencedSOPInstanceUIDInFile);
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
        /// Returns the hash code for the <see cref="ImageInfo"/>.
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

            return ReferencedSOPInstanceUIDInFile?.GetHashCode() ?? 0;
        }
    }
}
