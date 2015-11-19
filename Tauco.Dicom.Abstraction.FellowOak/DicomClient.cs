using System;
using System.Threading.Tasks;

using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Represents DICOM client with methods using Fellow Oak DICOM library.
    /// </summary>
    /// <typeparam name="TInfo">Specifies type of DICOM objet the request is related to</typeparam>
    internal class DicomClient<TInfo> : IDicomClient<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly DicomClient mClient;
        private readonly ISettingsProvider mSettingsProvider;
        private readonly IDicomRequestAdapter<TInfo> mDicomRequestAdapter;


        /// <summary>
        /// Initializes new instance of <see cref="DicomClient{TInfo}"/>.
        /// </summary>
        /// <param name="settingsProvider">Settings access provider</param>
        /// <param name="dicomRequestAdapter">Provides methods for creating third party DICOM compatible requests</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingsProvider"/> is null -or- <paramref name="dicomRequestAdapter"/> is null</exception>
        public DicomClient([NotNull] ISettingsProvider settingsProvider, [NotNull] IDicomRequestAdapter<TInfo> dicomRequestAdapter)
        {
            if (settingsProvider == null)
            {
                throw new ArgumentNullException(nameof(settingsProvider));
            }
            if (dicomRequestAdapter == null)
            {
                throw new ArgumentNullException(nameof(dicomRequestAdapter));
            }

            mClient = new DicomClient();
            mClient.NegotiateAsyncOps();
            mSettingsProvider = settingsProvider;
            mDicomRequestAdapter = dicomRequestAdapter;
        }


        /// <summary>
        /// Adds new C-FIND request to the client's queue.
        /// </summary>
        /// <param name="dicomRequest">Request to be sent</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomRequest"/> is <see langword="null"/></exception>
        public void AddFindRequest(IDicomFindRequest<TInfo> dicomRequest)
        {
            if (dicomRequest == null)
            {
                throw new ArgumentNullException(nameof(dicomRequest));
            }

            mClient.AddRequest((DicomRequest)mDicomRequestAdapter.CreateFindDicomRequest(dicomRequest));
        }


        /// <summary>
        /// Adds new C-MOVE request to the client's queue.
        /// </summary>
        /// <param name="dicomRequest">Request to be sent</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomRequest"/> is <see langword="null"/></exception>
        public void AddMoveRequest(IDicomMoveRequest dicomRequest)
        {
            if (dicomRequest == null)
            {
                throw new ArgumentNullException(nameof(dicomRequest));
            }

            mClient.AddRequest((DicomRequest)mDicomRequestAdapter.CreateMoveDicomRequest(dicomRequest));
        }


        /// <summary>
        /// Adds new request to the client's queue.
        /// </summary>
        public void AddPingRequest() 
        {
            mClient.AddRequest(new DicomCEchoRequest());
        }


        /// <summary>
        /// Sends all the request to the server given in configuration file
        /// </summary>
        /// <returns>Async result of the request</returns>
        public async Task SendAsync()
        {
            await SendAsync(mSettingsProvider.RemoteAddress, mSettingsProvider.RemotePort, mSettingsProvider.CallingApplicationEntity, mSettingsProvider.CalledApplicationEntity);
        }


        /// <summary>
        /// Sends all the request to the given server with given configuration.
        /// </summary>
        /// <param name="serverIP">IP address of the server</param>
        /// <param name="serverPort">Port number the server is listening on</param>
        /// <param name="callingAE">Application entity title of calling client</param>
        /// <param name="calledAE">Application entity title of called server</param>
        /// <returns>Async result of the request</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serverIP"/> is null -or- <paramref name="callingAE"/> is null -or- <paramref name="calledAE"/> is null.</exception>
        public async Task SendAsync(string serverIP, int serverPort, string callingAE, string calledAE)
        {
            if (serverIP == null)
            {
                throw new ArgumentNullException(nameof(serverIP));
            }
            if (callingAE == null)
            {
                throw new ArgumentNullException(nameof(callingAE));
            }
            if (calledAE == null)
            {
                throw new ArgumentNullException(nameof(calledAE));
            }

            await Task.Factory.FromAsync(mClient.BeginSend(serverIP, serverPort, false, callingAE, calledAE, null, null), mClient.EndSend);
        }


        #region IDisposable implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            mClient.Release();
        } 

        #endregion
    }
}
