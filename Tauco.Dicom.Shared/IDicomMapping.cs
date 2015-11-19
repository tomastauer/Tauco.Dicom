using System.Collections.Generic;
using System.Reflection;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Provides mapping from DICOM properties to ObjectInfo properties.
    /// </summary>
    public interface IDicomMapping : IEnumerable<KeyValuePair<PropertyInfo, DicomTags>>
    {
    }
}