using System;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides methods for retrieving the series.
    /// </summary>
    internal class SeriesInfoProvider : ISeriesInfoProvider
    {
        private readonly IDicomQueryProvider<SeriesInfo> mDicomQueryProvider;
        
        /// <summary>
        /// Instantiates new instance of <see cref="SeriesInfoProvider"/>.
        /// </summary>
        /// <param name="dicomQueryProvider">Provides method for creating new instance of <see cref="DicomQuery{SeriesInfo}" /></param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomQueryProvider"/> is <see langword="null"/></exception>
        public SeriesInfoProvider([NotNull] IDicomQueryProvider<SeriesInfo> dicomQueryProvider)
        {
            if (dicomQueryProvider == null)
            {
                throw new ArgumentNullException(nameof(dicomQueryProvider));
            }
           
            mDicomQueryProvider = dicomQueryProvider;
        }


        /// <summary>
        /// Gets all series.
        /// </summary>
        /// <returns>Collecion containing all the series</returns>
        public IDicomQuery<SeriesInfo> GetSeries()
        {
            return mDicomQueryProvider.GetDicomQuery();
        } 


        /// <summary>
        /// Gets all series for the given study.
        /// </summary>
        /// <param name="study">Study the series are related to</param>
        /// <exception cref="ArgumentNullException"><paramref name="study"/> is null</exception>
        /// <returns>Collection containing all the series of the study</returns>
        public IDicomQuery<SeriesInfo> GetSeriesForStudy([NotNull] StudyInfo study)
        {
            if (study == null)
            {
                throw new ArgumentNullException(nameof(study));
            }

            return mDicomQueryProvider.GetDicomQuery().WhereEquals(DicomTags.StudyInstanceUID, study.StudyInstanceUID.StringRepresentation);
        }


        /// <summary>
        /// Gets single series by given identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Identifier of the series</param>
        /// <param name="loadFromCache">Determines whether the <see cref="SeriesInfo"/> should be loaded from cache or from server</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/></exception>
        /// <returns>Series with given identifier, if such series exists; otherwise, null</returns>
        public async Task<SeriesInfo> GetSeriesByIDAsync([NotNull] InfoIdentifier identifier, bool loadFromCache = false)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var query = GetSeries();
            if (loadFromCache)
            {
                query = query.LoadFromCache();
            }

            return (await query
                .WhereEquals(DicomTags.SeriesInstanceUID, identifier.StringRepresentation)
                .ToListAsync()).FirstOrDefault();
        }
    }
}
