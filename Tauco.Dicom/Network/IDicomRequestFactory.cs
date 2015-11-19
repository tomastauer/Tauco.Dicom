using System;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Contains method for creating new instance of <see cref="DicomFindRequest{TInfo}"/>.
    /// </summary>
    internal interface IDicomRequestFactory
    {
        /// <summary>
        /// Creates new instance of <see cref="DicomFindRequest{TInfo}"/>.
        /// </summary>
        /// <typeparam name="TInfo">Type of requested object</typeparam>
        /// <param name="dicomWhereCollection">Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.</param>
        /// <param name="responseCallback">Method called once the request was fulfilled. Contains item obtained from the response</param>
        /// <returns>New instance of <see cref="DicomFindRequest{TInfo}"/></returns>
        IDicomFindRequest<TInfo> CreateDicomFindRequest<TInfo>(IDicomWhereCollection dicomWhereCollection, Action<TInfo> responseCallback) where TInfo : IDicomInfo;


        /// <summary>
        /// Creates new instance of <see cref="DicomMoveRequest"/>.
        /// </summary>
        /// <param name="identifier">UID identifier specifying for which object will be the request made</param>
        /// <returns>New instance of <see cref="DicomMoveRequest"/></returns>
        IDicomMoveRequest CreateDicomMoveRequest(InfoIdentifier identifier);
    }
}