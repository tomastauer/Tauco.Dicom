using System;
using System.Reflection;

using Dicom;
using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Represents Fellow Oak DICOM request for given <typeparamref name="TInfo" /> .
    /// </summary>
    /// <typeparam name="TInfo">Object info to be transformed to Dicom request</typeparam>
    internal class FellowOakDicomFindRequest<TInfo> : IDicomRequest, IDicomFindRequest<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly IDicomMapping mDicomMapping;
        private readonly IDicomTagAdapter mDicomTagAdapter;
        private readonly IDicomInfoBuilder mDicomInfoBuilder;
        private readonly DicomCFindRequest mInnerRequest;


        /// <summary>
        /// Instantiates new instance of Dicom C-FIND request for given <typeparamref name="TInfo" /> based on given <paramref name="dicomDicomWhereCollection"/>.
        /// </summary>
        /// <param name="dicomMapping">Provides mapping from DICOM properties to ObjectInfo properties</param>
        /// <param name="dicomTagAdapter">
        /// Provides method for obtaining third party DICOM tag representation from the
        /// <see cref="DicomTags" />
        /// </param>
        /// <param name="dicomInfoBuilder"></param>
        /// <param name="generalizedInfoProvider">
        /// Collection containing single object for all implementation of
        /// <see cref="IDicomInfo" />
        /// </param>
        /// <param name="dicomSopClassUidProvider">Provides method for obtaining Dicom Service-object pair class UID for given <see cref="IDicomInfo"/></param>
        /// <param name="actionCallback">Specifies action fired once the response is loaded</param>
        /// <param name="dicomDicomWhereCollection">Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomMapping"/> is <see langword="null" /> -or- <paramref name="dicomTagAdapter"/> is 
        /// <see langword="null" /> -or- <paramref name="dicomInfoBuilder"/> is <see langword="null" /> -or- <paramref name="generalizedInfoProvider"/> is <see langword="null" /> -or- <see cref="mappingEngine"/> is <see langword="null"/> -or-
        /// <see cref="dicomSopClassUidProvider"/> is <see langword="null"/> -or- <see cref="actionCallback"/> is <see langword="null"/> -or- <see cref="dicomDicomWhereCollection"/> is <see langword="null"/>
        /// </exception>
        public FellowOakDicomFindRequest([NotNull] IDicomMapping dicomMapping, [NotNull] IDicomTagAdapter dicomTagAdapter, [NotNull] IDicomInfoBuilder dicomInfoBuilder, [NotNull] IGeneralizedInfoProvider generalizedInfoProvider, [NotNull] IDicomSOPClassUIDProvider dicomSopClassUidProvider, [NotNull] Action<TInfo> actionCallback,
            [NotNull] IDicomWhereCollection dicomDicomWhereCollection)
        {
            if (dicomMapping == null)
            {
                throw new ArgumentNullException(nameof(dicomMapping));
            }
            if (dicomTagAdapter == null)
            {
                throw new ArgumentNullException(nameof(dicomTagAdapter));
            }
            if (dicomInfoBuilder == null)
            {
                throw new ArgumentNullException(nameof(dicomInfoBuilder));
            }
            if (generalizedInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(generalizedInfoProvider));
            }
            if (dicomSopClassUidProvider == null)
            {
                throw new ArgumentNullException(nameof(dicomSopClassUidProvider));
            }
            if (actionCallback == null)
            {
                throw new ArgumentNullException(nameof(actionCallback));
            }
            if (dicomDicomWhereCollection == null)
            {
                throw new ArgumentNullException(nameof(dicomDicomWhereCollection));
            }

            mDicomMapping = dicomMapping;
            mDicomTagAdapter = dicomTagAdapter;
            mDicomInfoBuilder = dicomInfoBuilder;
            ResponseCallback = actionCallback;
            DicomWhereCollection = dicomDicomWhereCollection;
            TInfo generalizedInfo = generalizedInfoProvider.GetGeneralizedInfo<TInfo>();

            mInnerRequest = new DicomCFindRequest((DicomQueryRetrieveLevel)generalizedInfo.DicomType)
            {
                SOPClassUID = dicomSopClassUidProvider.GetDicomSOPClassUid(generalizedInfo),
                OnResponseReceived = OnResponseReceived(),
            };
                
            AddWhereConditionToInnerRequest();
        }


        /// <summary>
        /// Contains constraints used for filtering of requested items.
        /// </summary>
        public IDicomWhereCollection DicomWhereCollection
        {
            get;
        }


        /// <summary>
        /// Called once the response was proceeded. Contains new instance of <see cref="TInfo" />
        /// </summary>
        public Action<TInfo> ResponseCallback
        {
            get;
        }


        /// <summary>
        /// Inner Fellow Oak DICOM request.
        /// </summary>
        public DicomRequest InnerRequest => mInnerRequest;


        /// <summary>
        /// Delegate raised when response for inner request is received.
        /// </summary>
        /// <returns>
        /// Delegate handling on response received event
        /// </returns>
        private DicomCFindRequest.ResponseDelegate OnResponseReceived()
        {
            return (request, response) =>
            {
                if (response.Dataset == null)
                {
                    return;
                }

                ResponseCallback(mDicomInfoBuilder.BuildInfo<TInfo>(response.Dataset));
            };
        }


        /// <summary>
        /// Adds new where condition for every property within the given <see cref="mDicomMapping" /> for which exists record in the
        /// <see cref="DicomWhereCollection" />.
        /// </summary>
        private void AddWhereConditionToInnerRequest()
        {
            foreach (var item in mDicomMapping)
            {
                var value = DicomWhereCollection?[item.Value];
                mInnerRequest.Dataset.Add((DicomTag) mDicomTagAdapter.GetDicomTag(item.Value), value);
            }
        }
    }
}