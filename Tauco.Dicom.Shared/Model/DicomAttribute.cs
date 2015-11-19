using System;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Attribute determining the decorated property should be mapped to Dicom property via the <see cref="DicomTags"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DicomAttribute : Attribute
    {
        /// <summary>
        /// DicomTag the property should be mapped to.
        /// </summary>
        public DicomTags DicomTag
        {
            get;
            private set;
        }


        /// <summary>
        /// Initializes new instance of <see cref="DicomAttribute"/>. 
        /// </summary>
        /// <param name="dicomTag">Specifies to which dicom tag should be property mapped to</param>
        /// <exception cref="ArgumentException"><paramref name="dicomTag"/> cannot be set to <see cref="DicomTags.Undefined"/></exception>
        public DicomAttribute(DicomTags dicomTag)
        {
            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException($"Dicom tag cannot be set to {DicomTags.Undefined}", nameof(dicomTag));
            }

            DicomTag = dicomTag;
        }
    }
}
