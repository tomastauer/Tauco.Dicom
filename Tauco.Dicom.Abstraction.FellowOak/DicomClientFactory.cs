using System;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for obtaining new instance of <see cref="IDicomClient{TInfo}"/> implementation.
    /// </summary>
    internal class DicomClientFactory<TInfo> : IDicomClientFactory<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly ISettingsProvider mSettingsProvider;
        private readonly IDicomRequestAdapter<TInfo> mDicomRequestAdapter;
        
        /// <summary>
        /// Instantiates new instance if <see cref="DicomClientFactory{TInfo}"/>.
        /// </summary>
        /// <param name="settingsProvider">Provides access to the setting values</param>
        /// <param name="dicomRequestAdapter"></param>
        /// <exception cref="ArgumentNullException"><paramref name="settingsProvider"/> is null -or- <paramref name="dicomRequestAdapter"/> is null</exception>
        public DicomClientFactory([NotNull] ISettingsProvider settingsProvider, [NotNull] IDicomRequestAdapter<TInfo> dicomRequestAdapter)
        {
            if (settingsProvider == null)
            {
                throw new ArgumentNullException(nameof(settingsProvider));
            }
            if (dicomRequestAdapter == null)
            {
                throw new ArgumentNullException(nameof(dicomRequestAdapter));
            }

            mSettingsProvider = settingsProvider;
            mDicomRequestAdapter = dicomRequestAdapter;
        }


        /// <summary>
        /// Creates new instance of <see cref="IDicomClient{TInfo}"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IDicomClient{TInfo}"/></returns>
        public IDicomClient<TInfo> CreateDicomClient()
        {
            return new DicomClient<TInfo>(mSettingsProvider, mDicomRequestAdapter);
        }
    }
}
