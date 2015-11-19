using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Represents DICOM C-MOVE request entity.
    /// </summary>
    internal class DicomMoveRequest : IDicomMoveRequest
    {
        /// <summary>
        /// Application entity of the destination server.
        /// </summary>
        public string DestinationAE
        {
            get;
            set;
        }


        /// <summary>
        /// UID identifier specifying for which object will be the request made.
        /// </summary>
        public InfoIdentifier Identifier
        {
            get;
            set;
        }
    }
}