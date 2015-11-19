using System;
using System.IO;

using Castle.Core.Logging;

using Dicom.Log;
using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for obtaining new isntance of <see cref="IDicomServer"/> implementation.
    /// </summary>
    internal class DicomServerFactory : IDicomServerFactory
    {
        private readonly ISettingsProvider mSettingsProvider;
        private readonly ILogger mLogger;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomServerFactory"/>.
        /// </summary>
        /// <param name="settingsProvider">Provides access to the setting values</param>
        /// <param name="logger">Logging service</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingsProvider"/> is <see langword="null"/> -or- <paramref name="logger"/> is <see langword="null"/></exception>
        public DicomServerFactory([NotNull] ISettingsProvider settingsProvider, [NotNull] ILogger logger)
        {
            if (settingsProvider == null)
            {
                throw new ArgumentNullException(nameof(settingsProvider));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            mSettingsProvider = settingsProvider;
            mLogger = logger;
        }


        /// <summary>
        /// Creates new instance of <see cref="IDicomServer"/>.
        /// </summary>
        /// <param name="downloadAction">Specifies action which will be performed once server gets C-MOVE request</param>
        /// <exception cref="ArgumentNullException"><paramref name="downloadAction"/> is <see langword="null"/></exception>
        /// <returns>New instance of <see cref="IDicomServer"/></returns>
        public IDicomServer CreateDicomServer(Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction)
        {
            if (downloadAction == null)
            {
                throw new ArgumentNullException(nameof(downloadAction));
            }

            return new DicomServer<DicomStore>(mSettingsProvider.LocalPort, downloadAction, mLogger);
        }
    }
}