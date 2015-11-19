using System;
using System.Reflection;

using Dicom;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for obtaining Fellow Oak DICOM item instance from the <see cref="DicomTags"/>.
    /// </summary>
    internal class DicomTagAdapter : IDicomTagAdapter
    {
        /// <summary>
        /// Gets <see cref="DicomTag"/> instance from the given <paramref name="dicomTag"/>.
        /// </summary>
        /// <param name="dicomTag">Dicom tag to be converted to the <see cref="DicomTag"/></param>
        /// <exception cref="ArgumentException"><paramref name="dicomTag"/> cannot be set to <see cref="DicomTags.Undefined"/></exception>
        /// <returns>Instance of <see cref="DicomTag"/> corresponding to the given <paramref name="dicomTag"/></returns>
        public object GetDicomTag(DicomTags dicomTag)
        {
            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException($"Dicom tag cannot be set to {DicomTags.Undefined}", nameof(dicomTag));
            }

            FieldInfo fieldInfo = typeof(DicomTag).GetField(dicomTag.ToString(), BindingFlags.Static | BindingFlags.Public);
            return (DicomTag) fieldInfo?.GetValue(null);    
        }
    }
}