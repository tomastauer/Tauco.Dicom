using System;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Represents DICOM C-FIND request entity.
    /// </summary>
    /// <typeparam name="TInfo">Type of requested object</typeparam>
    public interface IDicomFindRequest<in TInfo>  where TInfo : IDicomInfo
    {
        /// <summary>
        /// Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.
        /// </summary>
        IDicomWhereCollection DicomWhereCollection
        {
            get;
        }


        /// <summary>
        /// Method called once the request was fulfilled. Contains item obtained from the response.
        /// </summary>
        Action<TInfo> ResponseCallback
        {
            get;
        }
    }
}
