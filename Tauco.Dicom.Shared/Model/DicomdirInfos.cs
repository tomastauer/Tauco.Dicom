using System.Collections.Generic;

namespace Tauco.Dicom.Shared
{
    public class DicomdirInfos
    {
        public IEnumerable<PatientInfo> Patients
        {
            get;
            set;
        }


        public IEnumerable<StudyInfo> Studies
        {
            get;
            set;
        }


        public IEnumerable<SeriesInfo> Series
        {
            get;
            set;
        } 
    }
}
