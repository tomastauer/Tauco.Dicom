using System.Threading.Tasks;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides method for parsing given dicomdir file to collections for every corresponding info object.
    /// </summary>
    public interface IDicomdirFileParser
    {
        /// <summary>
        /// Parses dicomdir file located at given <paramref name="dicomdirPath"/> and returns <see cref="DicomdirInfos"/> composed of collections
        /// containing all <see cref="PatientInfo"/>, <see cref="StudyInfo"/> and <see cref="SeriesInfo"/> from the dicomdir file.
        /// </summary>
        /// <param name="dicomdirPath">Path leading to the dicomdir file</param>
        /// <returns>Instance of <see cref="DicomdirInfos"/> composed of collections containing all parsed infos.</returns>
        Task<DicomdirInfos> ParseDicomdirAsync(string dicomdirPath);
    }
}
