using System.Collections.Generic;
using System.Reflection;

using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Provides mapping from DICOM properties to ObjectInfo properties.
    /// </summary>
    public interface IDicomMapping : IEnumerable<KeyValuePair<PropertyInfo, DicomTags>>
    {
    }
}