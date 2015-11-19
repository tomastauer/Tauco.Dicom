using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Tauco.Dicom.Shared;

namespace Tauco.Tests.Fakes
{
    internal class DicomMappingFake : IDicomMapping
    {
        private readonly Dictionary<PropertyInfo, DicomTags> mappingFake = new Dictionary<PropertyInfo, DicomTags>
            {
                {
                    typeof (TestInfo).GetProperty("PatientID"), DicomTags.PatientID
                }
            };

        public IEnumerator<KeyValuePair<PropertyInfo, DicomTags>> GetEnumerator()
        {
            return mappingFake.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}