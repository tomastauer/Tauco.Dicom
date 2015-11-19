using System;

using Dicom;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for obtaining Dicom Service-object pair class UID for given <see cref="IDicomInfo"/>.
    /// </summary>
    internal class DicomSOPClassUIDProvider : IDicomSOPClassUIDProvider
    {
        /// <summary>
        /// Gets Dicom Service-object pair class UID for the given type of <paramref name="dicomInfo"/>. This object is required for 
        /// processing of DICOM request.
        /// </summary>
        /// <param name="dicomInfo">Instance of dicom info</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomInfo"/> is <see langword="null"/></exception>
        /// <returns>DicomUID corresponding to the given <paramref name="dicomInfo"/></returns>
        public DicomUID GetDicomSOPClassUid(IDicomInfo dicomInfo)
        {
            if (dicomInfo == null)
            {
                throw new ArgumentNullException(nameof(dicomInfo));
            }

            switch (dicomInfo.DicomType)
            {
                case DicomInfoType.Patient:
                    return DicomUID.PatientRootQueryRetrieveInformationModelFIND;
                case DicomInfoType.Study:
                case DicomInfoType.Series:
                    return DicomUID.StudyRootQueryRetrieveInformationModelFIND;
            }

            throw new ArgumentException("Invalid type of dicom info.", nameof(dicomInfo));
        }
    }
}