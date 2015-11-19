using System;

using JetBrains.Annotations;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Contains method for creating new instance of <see cref="DicomFindRequest{TInfo}"/>.
    /// </summary>
    internal class DicomRequestFactory : IDicomRequestFactory
    {
        private readonly ISettingsProvider mSettingsProvider;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomRequestFactory"/>.
        /// </summary>
        /// <param name="settingsProvider">Provides access to the setting values</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingsProvider"/> is <see langword="null" /></exception>
        public DicomRequestFactory([NotNull] ISettingsProvider settingsProvider)
        {
            if (settingsProvider == null)
            {
                throw new ArgumentNullException(nameof(settingsProvider));
            }

            mSettingsProvider = settingsProvider;
        }


        /// <summary>
        /// Creates new instance of <see cref="DicomFindRequest{TInfo}"/>.
        /// </summary>
        /// <typeparam name="TInfo">Type of requested object</typeparam>
        /// <param name="dicomWhereCollection">Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.</param>
        /// <param name="responseCallback">Method called once the request was fulfilled. Contains item obtained from the response</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomWhereCollection"/> is <see langword="null" /> -or- <paramref name="responseCallback"/> is <see langword="null" /></exception>
        /// <returns>New instance of <see cref="DicomFindRequest{TInfo}"/></returns>
        public IDicomFindRequest<TInfo> CreateDicomFindRequest<TInfo>([NotNull] IDicomWhereCollection dicomWhereCollection, [NotNull] Action<TInfo> responseCallback) where TInfo : IDicomInfo
        {
            if (dicomWhereCollection == null)
            {
                throw new ArgumentNullException(nameof(dicomWhereCollection));
            }
            if (responseCallback == null)
            {
                throw new ArgumentNullException(nameof(responseCallback));
            }

            return new DicomFindRequest<TInfo>
            {
                ResponseCallback = responseCallback,
                DicomWhereCollection = dicomWhereCollection
            };
        }


        /// <summary>
        /// Creates new instance of <see cref="DicomMoveRequest"/>.
        /// </summary>
        /// <param name="identifier">UID identifier specifying for which object will be the request made</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null" /></exception>
        /// <returns>New instance of <see cref="DicomMoveRequest"/></returns>
        public IDicomMoveRequest CreateDicomMoveRequest([NotNull] InfoIdentifier identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return new DicomMoveRequest {
                DestinationAE = mSettingsProvider.DestinationApplicationEntity,
                Identifier = identifier
            };
        }
    }
}
