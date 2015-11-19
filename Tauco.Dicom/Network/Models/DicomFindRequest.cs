using System;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Represents DICOM C-FIND request entity.
    /// </summary>
    /// <typeparam name="TInfo">Type of requested object</typeparam>
    internal class DicomFindRequest<TInfo> : IDicomFindRequest<TInfo> where TInfo : IDicomInfo
    {
        /// <summary>
        /// Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.
        /// </summary>
        public IDicomWhereCollection DicomWhereCollection
        {
            get;
            set;
        }


        /// <summary>
        /// Method called once the request was fulfilled. Contains item obtained from the response.
        /// </summary>
        public Action<TInfo> ResponseCallback
        {
            get;
            set;
        }
    }
}
