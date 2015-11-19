using System;
using System.IO;

using Castle.Core.Logging;

using Dicom;
using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Enables receiving of C-MOVE requests. Suitable for downloading files from the DICOM server.
    /// </summary>
    internal class DicomStore : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        private readonly Func<InfoIdentifier, InfoIdentifier, Stream> mDownloadAction;
        private readonly ILogger mLogger;


        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes =
        {
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes =
        {
            // Lossless
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEGProcess14SV1,
            DicomTransferSyntax.JPEGProcess14,
            DicomTransferSyntax.RLELossless,

            // Lossy
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.JPEGProcess1,
            DicomTransferSyntax.JPEGProcess2_4,

            // Uncompressed
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };


        /// <summary>
        /// Instantiates new instance of <see cref="DicomStore"/>.
        /// </summary>
        /// <param name="stream">Network stream</param>
        /// <param name="downloadAction">Specifies action which will be performed once server gets C-MOVE request</param>
        /// <param name="logger">Logging service</param>
        /// <exception cref="ArgumentNullException"><paramref name="downloadAction"/> is <see langword="null"/> -or- <paramref name="logger"/> is <see langword="null"/></exception>
        public DicomStore(Stream stream, [NotNull] Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction, [NotNull] ILogger logger)
            : base(stream, null)
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


        #region IDicomServiceProvider implementation

        /// <summary>
        /// Raised when external service tries to create association with the store. According to the request sets proper accepting transfer syntax.
        /// </summary>
        /// <param name="association">Association object specifies requirements of the external service</param>
        /// <exception cref="ArgumentNullException"><paramref name="association"/> is <see langword="null"/></exception>
        public void OnReceiveAssociationRequest([NotNull] DicomAssociation association)
        {
            if (association == null)
            {
                throw new ArgumentNullException(nameof(association));
            }

            foreach (var presentationContext in association.PresentationContexts)
            {
                if (presentationContext.AbstractSyntax == DicomUID.Verification)
                {
                    presentationContext.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                }
                else if (presentationContext.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                {
                    presentationContext.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
                }
            }

            SendAssociationAccept(association);
        }


        /// <summary>
        /// Raised when external service attempts to release association with the store. 
        /// </summary>
        public void OnReceiveAssociationReleaseRequest()
        {
            mLogger.Info($"Dicom store has received an association release request.");
            SendAssociationReleaseResponse();
        }


        /// <summary>
        /// Raised when received abortion request. Without implementation.
        /// </summary>
        /// <param name="source">Specifies source of the abortion</param>
        /// <param name="reason">Specifies reason of the abortion</param>
        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            mLogger.Info($"Dicom store received an abort request by {source} with reason {reason}");
        }


        /// <summary>
        /// Raised when connection to the store is closed. Without implementation.
        /// </summary>
        /// <param name="exception">Exception thrown when connection was closed due to failure</param>
        public void OnConnectionClosed(Exception exception)
        {
            const string MESSAGE = "Connection of external service with Dicom store was closed.";

            if (exception == null)
            {
                mLogger.Info(MESSAGE);
            }
            else
            {
                mLogger.Error(MESSAGE, exception);
            }
        }

        #endregion

        #region IDicomCStoreProvider implementation

        /// <summary>
        /// Raised when C-STORE request is received.
        /// </summary>
        /// <param name="request">Received C-STORE request</param>
        /// <returns>Response of the store to the request</returns>
        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            var studyUid = request.Dataset.Get<string>(DicomTag.StudyInstanceUID);
            var instanceUid = request.SOPInstanceUID.UID;

            using (var stream = mDownloadAction(studyUid, instanceUid))
            {
                request.File.Save(stream);
            }
              
            return new DicomCStoreResponse(request, DicomStatus.Success);
        }


        /// <summary>
        /// Raised when C-STORE request throws an <see cref="e"/>. Without implementation.
        /// </summary>
        /// <param name="tempFileName">Temporary file name containing the request stream</param>
        /// <param name="e">Thrown exception</param>
        public void OnCStoreRequestException(string tempFileName, Exception e)
        {
            mLogger.Error("Request of external service to Dicom store ends with exception.", e);
        }

        #endregion

        #region IDicomCEchoProvider implementation

        /// <summary>
        /// Raised when received C-ECHO request. Returns proper response.
        /// </summary>
        /// <param name="request">Received C-ECHO request</param>
        /// <remarks>
        /// C-ECHO request handling is necessary since external service may make a echo request prior to the file sending and expects some response.
        /// </remarks>
        /// <returns>C-ECHO response</returns>
        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        #endregion 
    }
}
