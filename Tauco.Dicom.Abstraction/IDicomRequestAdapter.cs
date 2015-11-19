using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides methods for creating third party DICOM compatible requests.
    /// </summary>
    /// <typeparam name="TInfo">Specifies type of DICOM object the request is related to</typeparam>
    public interface IDicomRequestAdapter<TInfo> where TInfo : IDicomInfo, new()
    {
        /// <summary>
        /// Get C-FIND DICOM request from given <paramref name="findRequest"/>.
        /// </summary>
        /// <param name="findRequest">Find request to be used for creating new compatible request</param>
        /// <returns>Compatible request corresponding to the given <paramref name="findRequest"/></returns>
        object CreateFindDicomRequest([NotNull] IDicomFindRequest<TInfo> findRequest);


        /// <summary>
        /// Get C-MOVE DICOM request from given <paramref name="moveRequest"/>.
        /// </summary>
        /// <param name="moveRequest">Move request to be used for creating new compatible request</param>
        /// <returns>Compatible request corresponding to the given <paramref name="moveRequest"/></returns>
        object CreateMoveDicomRequest([NotNull] IDicomMoveRequest moveRequest);
    }
}