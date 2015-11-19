using System.Runtime.Serialization;

using Tauco.Cache;
using Tauco.Dicom.Shared;

namespace Tauco.Tests.Fakes
{
    [DataContract]
    public class TestInfo : IDicomInfo
    {
        [CacheIndex]
        [Dicom(DicomTags.PatientID)]
        public int PatientID
        {
            get;
            set;
        }

        [Dicom(DicomTags.PatientName)]
        public string PatientName
        {
            get;
            set;
        }

        public DicomInfoType DicomType => DicomInfoType.Patient;
    }
}
