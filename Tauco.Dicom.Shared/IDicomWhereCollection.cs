using System.Collections.Generic;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Collection containing constraints used for filtering requested items suitable for DICOM C-FIND requests.
    /// </summary>
    public interface IDicomWhereCollection : IList<DicomWhereItem>
    {
        string this[DicomTags dicomTag]
        {
            get;
        }

    }
}