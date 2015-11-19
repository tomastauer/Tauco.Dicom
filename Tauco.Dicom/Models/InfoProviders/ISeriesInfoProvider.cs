using System.Threading.Tasks;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides methods for retrieving the series.
    /// </summary>
    public interface ISeriesInfoProvider
    {
        /// <summary>
        /// Gets all series.
        /// </summary>
        /// <returns>Collecion containing all the series</returns>
        IDicomQuery<SeriesInfo> GetSeries();


        /// <summary>
        /// Gets all series for the given study.
        /// </summary>
        /// <param name="study">Study the series are related to</param>
        /// <returns>Collection containing all the series of the study</returns>
        IDicomQuery<SeriesInfo> GetSeriesForStudy(StudyInfo study);


        /// <summary>
        /// Gets single series by given identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Identifier of the series</param>
        /// <param name="loadFromCache">Determines whether the <see cref="SeriesInfo"/> should be loaded from cache or from server</param>
        /// <returns>Series with given identifier, if such series exists; otherwise, null</returns>
        Task<SeriesInfo> GetSeriesByIDAsync(InfoIdentifier identifier, bool loadFromCache = false);
    }
}