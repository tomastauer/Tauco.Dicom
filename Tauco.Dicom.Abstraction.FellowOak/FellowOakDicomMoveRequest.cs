using System;

using Dicom.Network;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    internal class FellowOakDicomMoveRequest : IDicomRequest
    {        
        private readonly DicomCMoveRequest mInnerRequest;
        
        /// <summary>
        /// Inner fo-dicom request;
        /// </summary>
        public DicomRequest InnerRequest => mInnerRequest;


        /// <summary>
        /// Constructor. Creates Dicom C-MOVE request for given <paramref name="destinationAE"/> and <paramref name="identifier"/>.
        /// </summary>
        /// <param name="destinationAE">Application entity of the destination server</param>
        /// <param name="identifier">Identifier of the object to be moved</param>
        /// <exception cref="ArgumentNullException"><paramref name="destinationAE"/> is <see langword="null"/> -or- <paramref name="identifier"/> is <see langword="null"/></exception>
        public FellowOakDicomMoveRequest([NotNull] string destinationAE, [NotNull] InfoIdentifier identifier)
        {
            if (destinationAE == null)
            {
                throw new ArgumentNullException(nameof(destinationAE));
            }
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }
            
            mInnerRequest = new DicomCMoveRequest(destinationAE, identifier.StringRepresentation);
        }
    }
}