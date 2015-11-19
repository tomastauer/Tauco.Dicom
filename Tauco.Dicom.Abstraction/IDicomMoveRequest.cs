using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Represents DICOM C-MOVE request entity.
    /// </summary>
    public interface IDicomMoveRequest
    {
        /// <summary>
        /// Application entity of the destination server.
        /// </summary>
        string DestinationAE
        {
            get;
            set;
        }


        /// <summary>
        /// UID identifier specifying for which object will be the request made.
        /// </summary>
        InfoIdentifier Identifier
        {
            get;
            set;
        }
    }
}