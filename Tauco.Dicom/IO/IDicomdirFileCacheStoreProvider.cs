using System.Threading.Tasks;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.IO
{
    /// <summary>
    /// Provides method for storing all the <see cref="PatientInfo"/>, <see cref="StudyInfo"/> and <see cref="SeriesInfo"/> obtained from the given 
    /// DICOMDIR file in the cache. 
    /// </summary>
    /// <remarks>
    /// Once the objects are cached, they can be accessed in the standard info provider way.
    /// </remarks>
    public interface IDicomdirFileCacheStoreProvider
    {
        /// <summary>
        /// Parses given <paramref name="dicomdirPath"/> and stores all the parsed <see cref="PatientInfo"/>, <see cref="StudyInfo"/> and <see cref="SeriesInfo"/> into the cache.
        /// </summary>
        /// <param name="dicomdirPath">Path leading to the dicomdir file</param>
        /// <returns>Object containing all the parsed data</returns>
        Task<DicomdirInfos> StoreItemsInCache(string dicomdirPath);
    }
}