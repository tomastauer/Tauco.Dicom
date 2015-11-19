using System;
using System.IO;

using Castle.Core.Logging;

using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Represents server requried for getting response from the C-MOVE request.
    /// </summary>
    internal class DicomServer<T> : global::Dicom.Network.DicomServer<T>, IDicomServer where T : DicomService, IDicomServiceProvider
    {
        private readonly Func<InfoIdentifier, InfoIdentifier, Stream> mDownloadAction;
        private readonly ILogger mLogger;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomServer{T}"/> and start listening on the given <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Number of port server will be listening on</param>
        /// <param name="downloadAction">Specifies action which will be performed once server gets C-MOVE request</param>
        /// <param name="logger">Logging service</param>
        /// <exception cref="ArgumentNullException"><paramref name="downloadAction"/> is <see langword="null"/> -or- <paramref name="logger"/> is <see langword="null"/></exception>
        public DicomServer(int port, [NotNull] Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction, [NotNull] ILogger logger)
            :base(port)
        {
            if (downloadAction == null)
            {
                throw new ArgumentNullException(nameof(downloadAction));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            mDownloadAction = downloadAction;
            mLogger = logger;
        }


        /// <summary>
        /// Creates new instance of Service class provider which will handle all incoming C-MOVE requests.
        /// </summary>
        /// <param name="stream">Network stream</param>
        /// <returns>New instance of <see cref="IDicomServiceProvider"/></returns>
        protected override T CreateScp(Stream stream)
        {
            return (T)Activator.CreateInstance(typeof(T), stream, mDownloadAction, mLogger);
        }
    }
}
