using System.Collections.Generic;
using System.Linq;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.
    /// </summary>
    internal class DicomWhereCollection : List<DicomWhereItem>, IDicomWhereCollection
    {
        public string this[DicomTags dicomTag]
        {
            get
            {
                return this.SingleOrDefault(c => c.DicomTag == dicomTag)?.Value;
            }
        }
    }
}
