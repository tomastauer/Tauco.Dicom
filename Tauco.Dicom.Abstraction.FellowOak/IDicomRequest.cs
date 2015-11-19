using Dicom.Network;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Represent all Fellow Oak DICOM requests.
    /// </summary>
    internal interface IDicomRequest
    {
        /// <summary>
        /// Inner Fellow Oak DICOM request.
        /// </summary>
        DicomRequest InnerRequest
        {
            get;
        }
    }
}