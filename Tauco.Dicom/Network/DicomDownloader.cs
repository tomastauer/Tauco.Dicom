using System;
using System.IO;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides method for downloading files from the DICOM server.
    /// </summary>
    internal class DicomDownloader<TInfo> : IDicomDownloader<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly IDicomClientFactory<TInfo> mClientFactory;
        private readonly object mLocker = new object();
        private readonly IDicomRequestFactory mRequestFactory;
        private readonly IDicomServerFactory mServerFactory;
        private int mClients;
        private IDicomServer mDicomServer;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomDownloader{TInfo}" />.
        /// </summary>
        /// <param name="serverFactory">Provides method for obtaining new isntance of <see cref="IDicomServer" /> implementation</param>
        /// <param name="clientFactory">
        /// Provides method for obtaining new instance of <see cref="IDicomClient{TInfo}" />
        /// implementation
        /// </param>
        /// <param name="requestFactory">
        /// Contains method for creating new instance of <see cref="DicomFindRequest{TInfo}" />
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serverFactory" /> is <see langword="null" /> -or-
        /// <paramref name="clientFactory" /> is <see langword="null" /> -or- <paramref name="requestFactory" /> is
        /// <see langword="null" />
        /// </exception>
        public DicomDownloader([NotNull] IDicomServerFactory serverFactory, [NotNull] IDicomClientFactory<TInfo> clientFactory, [NotNull] IDicomRequestFactory requestFactory)
        {
            if (serverFactory == null)
            {
                throw new ArgumentNullException(nameof(serverFactory));
            }
            if (clientFactory == null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            if (requestFactory == null)
            {
                throw new ArgumentNullException(nameof(requestFactory));
            }

            mServerFactory = serverFactory;
            mClientFactory = clientFactory;
            mRequestFactory = requestFactory;
        }


        /// <summary>
        /// Performs asynchronous file download identified by given <paramref name="identifier" /> from the DICOM server.
        /// </summary>
        /// <param name="identifier">Specifies file to be downloaded</param>
        /// <param name="downloadAction">Specifies action which will be performed once server gets the file</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier" /> is <see langword="null" /> -or- <paramref name="downloadAction" /> is <see langword="null" /></exception>
        /// <returns>Async result of the request</returns>
        public async Task DownloadAsync(InfoIdentifier identifier, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction = null)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }
            if (downloadAction == null)
            {
                downloadAction = GetDefaultDownloadAction();
            }

            CreateServer(downloadAction);
            using (var client = mClientFactory.CreateDicomClient())
            {
                IDicomMoveRequest moveRequest = mRequestFactory.CreateDicomMoveRequest(identifier);
                client.AddMoveRequest(moveRequest);

                await client.SendAsync();
            }
            DisposeServer();
        }


        /// <summary>
        /// Gets default action performed once the server gets respone to the C-MOVE request.
        /// </summary>
        /// <returns>Default download action</returns>
        private Func<InfoIdentifier, InfoIdentifier, Stream> GetDefaultDownloadAction()
        {
            return (studyUid, instanceUid) =>
            {
                var path = Path.GetFullPath(@".\DICOM");
                path = Path.Combine(path, studyUid);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return File.Create(Path.Combine(path, instanceUid) + ".dcm");
            };
        }


        /// <summary>
        /// Creates instance of DICOM server, ensures only one instance lives at time.
        /// Increment number of clients using the server.
        /// </summary>
        /// <param name="downloadAction">Specifies action which will be performed once server gets the file</param>
        private void CreateServer(Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction)
        {
            lock (mLocker)
            {
                if (mDicomServer == null)
                {
                    mDicomServer = mServerFactory.CreateDicomServer(downloadAction);
                    mClients++;
                }
            }
        }


        /// <summary>
        /// Decrements number of clients using the server. If hits zero, server is disposed.
        /// </summary>
        private void DisposeServer()
        {
            lock (mLocker)
            {
                mClients--;
                if (mClients == 0)
                {
                    mDicomServer.Dispose();
                    mDicomServer = null;
                }
            }
        }
    }
}