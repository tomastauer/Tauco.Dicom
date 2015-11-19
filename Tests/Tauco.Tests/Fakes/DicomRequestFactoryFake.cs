using System;

using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Tests.Fakes
{
    internal class DicomRequestFactoryFake : IDicomRequestFactory
    {
        public object WhereCollection
        {
            get;
            set;
        }

        public IDicomFindRequest<TInfo> CreateDicomFindRequest<TInfo>(IDicomWhereCollection dicomWhereCollection, Action<TInfo> responseCallback) where TInfo : IDicomInfo
        {
            WhereCollection = dicomWhereCollection;
            return null;
        }

        
        public IDicomMoveRequest CreateDicomMoveRequest(InfoIdentifier identifier)
        {
            throw new NotImplementedException();
        }
    }
}
