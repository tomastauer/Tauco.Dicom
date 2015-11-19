using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Represents DICOM client with methods suitable for sending request to DICOM servers.
    /// </summary>
    public interface IDicomClient<out TInfo> : IDisposable where TInfo : IDicomInfo
    {
        /// <summary>
        /// Adds new C-FIND request to the client's queue.
        /// </summary>
        /// <param name="dicomRequest">Request to be sent</param>
        void AddFindRequest([NotNull] IDicomFindRequest<TInfo> dicomRequest);


        /// <summary>
        /// Adds new C-MOVE request to the client's queue.
        /// </summary>
        /// <param name="dicomRequest">Request to be sent</param>
        void AddMoveRequest([NotNull] IDicomMoveRequest dicomRequest);
        

        /// <summary>
        /// Adds new C-ECHO request to the client's queue.
        /// </summary>
        void AddPingRequest();

        
        /// <summary>
        /// Sends all the request to the server given in configuration file
        /// </summary>
        /// <returns>Async result of the request</returns>
        Task SendAsync();


        /// <summary>
        /// Sends all the request to the given server with given configuration.
        /// </summary>
        /// <param name="serverIP">IP address of the server</param>
        /// <param name="serverPort">Port number the server is listening on</param>
        /// <param name="callingAE">Application entity title of calling client</param>
        /// <param name="calledAE">Application entity title of called server</param>
        /// <returns>Async result of the request</returns>
        Task SendAsync([NotNull] string serverIP, int serverPort, [NotNull] string callingAE, [NotNull] string calledAE);
    }
}