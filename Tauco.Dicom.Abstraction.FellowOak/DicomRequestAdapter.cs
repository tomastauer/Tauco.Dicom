using System;

using AutoMapper;

using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides methods for creating Fellow Oak DICOM compatible <see cref="DicomRequest"/>.
    /// </summary>
    /// <typeparam name="TInfo">Specifies type of DICOM objet the request is related to</typeparam>
    internal class DicomRequestAdapter<TInfo> : IDicomRequestAdapter<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly IDicomTagAdapter mDicomTagAdapter;
        private readonly IGeneralizedInfoProvider mGeneralizedInfoProvider;
        private readonly IMappingEngine mMappingEngine;
        private readonly IDicomSOPClassUIDProvider mDicomSopClassUidProvider;


        /// <summary>
        /// Initializes new instance of <see cref="DicomRequestAdapter{TInfo}"/>.
        /// </summary>
        /// <param name="dicomTagAdapter">Provides method for obtaining third party DICOM tag representation from the <see cref="DicomTags"/></param>
        /// <param name="generalizedInfoProvider">Collection containing single object for all implementation of <see cref="IDicomInfo" /></param>
        /// <param name="mappingEngine">Performs mapping based on configuration</param>
        /// <param name="dicomSopClassUidProvider">Provides method for obtaining Dicom Service-object pair class UID for given <see cref="IDicomInfo"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomTagAdapter"/> is <see langword="null"/> -or- <paramref name="generalizedInfoProvider"/> is <see langword="null"/> -or- <paramref name="mappingEngine"/> is <see langword="null"/> -or- <paramref name="dicomSopClassUidProvider"/> is <see langword="null"/></exception>
        public DicomRequestAdapter([NotNull] IDicomTagAdapter dicomTagAdapter, [NotNull] IGeneralizedInfoProvider generalizedInfoProvider, [NotNull] IMappingEngine mappingEngine,
            [NotNull] IDicomSOPClassUIDProvider dicomSopClassUidProvider)
        {
            if (dicomTagAdapter == null)
            {
                throw new ArgumentNullException(nameof(dicomTagAdapter));
            }
            if (generalizedInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(generalizedInfoProvider));
            }
            if (mappingEngine == null)
            {
                throw new ArgumentNullException(nameof(mappingEngine));
            }
            if (dicomSopClassUidProvider == null)
            {
                throw new ArgumentNullException(nameof(dicomSopClassUidProvider));
            }

            mDicomTagAdapter = dicomTagAdapter;
            mGeneralizedInfoProvider = generalizedInfoProvider;
            mMappingEngine = mappingEngine;
            mDicomSopClassUidProvider = dicomSopClassUidProvider;
        }


        /// <summary>
        /// Get C-FIND DICOM request from given <paramref name="findRequest"/>.
        /// </summary>
        /// <param name="findRequest">Find request to be used for creating new <see cref="DicomRequest"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="findRequest"/> is <see langword="null"/></exception>
        /// <returns><see cref="DicomRequest"/> corresponding to the given <paramref name="findRequest"/></returns>
        public object CreateFindDicomRequest(IDicomFindRequest<TInfo> findRequest)
        {
            if (findRequest == null)
            {
                throw new ArgumentNullException(nameof(findRequest));
            }

            var request = new FellowOakDicomFindRequest<TInfo>(new DicomMapping(typeof (TInfo)), mDicomTagAdapter, mGeneralizedInfoProvider, mMappingEngine, mDicomSopClassUidProvider,
                findRequest.ResponseCallback, findRequest.DicomWhereCollection);

            return request.InnerRequest;
        }


        /// <summary>
        /// Get C-MOVE DICOM request from given <paramref name="moveRequest"/>.
        /// </summary>
        /// <param name="moveRequest">Move request to be used for creating new <see cref="DicomRequest"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="moveRequest"/> is <see langword="null"/></exception>
        /// <returns>Compatible request corresponding to the given <paramref name="moveRequest"/></returns>
        public object CreateMoveDicomRequest(IDicomMoveRequest moveRequest)
        {
            if (moveRequest == null)
            {
                throw new ArgumentNullException(nameof(moveRequest));
            }

            var request = new FellowOakDicomMoveRequest(moveRequest.DestinationAE, moveRequest.Identifier);
            return request.InnerRequest;
        }
    }
}
