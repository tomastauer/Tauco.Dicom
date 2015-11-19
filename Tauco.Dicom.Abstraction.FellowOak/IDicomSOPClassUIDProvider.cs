using Dicom;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;   

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for obtaining Dicom Service-object pair class UID for given <see cref="IDicomInfo"/>.
    /// </summary>
    internal interface IDicomSOPClassUIDProvider
    {
        /// <summary>
        /// Gets Dicom Service-object pair class UID for the given type of <paramref name="dicomInfo"/>. This object is required for 
        /// processing of DICOM request.
        /// </summary>
        /// <param name="dicomInfo">Instance of dicom info</param>
        /// <returns>DicomUID corresponding to the given <paramref name="dicomInfo"/></returns>
        DicomUID GetDicomSOPClassUid([NotNull] IDicomInfo dicomInfo);
    }
}