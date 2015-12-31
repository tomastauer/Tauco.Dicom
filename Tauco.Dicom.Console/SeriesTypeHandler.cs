using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Contains method for handling series type.
    /// </summary>
    internal class SeriesTypeHandler : ITypeHandler
    {
        private readonly ISeriesInfoProvider mSeriesInfoProvider;
        private readonly IStudyInfoProvider mStudyInfoProvider;


        /// <summary>
        /// Instantiates new instance of <see cref="SeriesTypeHandler" />.
        /// </summary>
        /// <param name="seriesInfoProvider">Provides methods for retrieving the studies</param>
        /// <param name="studyInfoProvider">Provides methods for retrieving the studies</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seriesInfoProvider" /> is <see langword="null" /> -or-
        /// <paramref name="studyInfoProvider" /> is <see langword="null" />
        /// </exception>
        public SeriesTypeHandler(ISeriesInfoProvider seriesInfoProvider, IStudyInfoProvider studyInfoProvider)
        {
            if (seriesInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(seriesInfoProvider));
            }
            if (studyInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(studyInfoProvider));
            }

            mSeriesInfoProvider = seriesInfoProvider;
            mStudyInfoProvider = studyInfoProvider;
        }


        /// <summary>
        /// Handles downloading of series related data.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <exception cref="ArgumentException">
        /// Wrong <see cref="InputArguments.Type" /> in <paramref name="inputArguments" />.
        /// Expected value is 'series'.
        /// </exception>
        /// <returns>Represents an asynchronous operation</returns>
        public async Task HandleTypeAsync(InputArguments inputArguments, TextWriter writer)
        {
            if (!inputArguments.Type.Equals("series", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Type has to be 'series'", nameof(inputArguments));
            }

            if (inputArguments.Identifier != null)
            {
                await HandleSingleSeries(inputArguments, writer);
            }
            else
            {
                await HandleMultipleSeries(inputArguments, writer);
            }
        }


        /// <summary>
        /// Handles retrieving of single series.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleSingleSeries(InputArguments inputArguments, TextWriter writer)
        {
            var series = await mSeriesInfoProvider.GetSeriesByIDAsync(inputArguments.Identifier, inputArguments.UseCache);
            await writer.WriteAsync(JsonConvert.SerializeObject(series));
        }


        /// <summary>
        /// Handles retrieving of multiple series.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleMultipleSeries(InputArguments inputArguments, TextWriter writer)
        {
            IDicomQuery<SeriesInfo> query;
            if (inputArguments.ParentIdentifier != null)
            {
                var study = await mStudyInfoProvider.GetStudyByIDAsync(inputArguments.ParentIdentifier, inputArguments.UseCache);
                query = mSeriesInfoProvider.GetSeriesForStudy(study);
            }
            else
            {
                query = mSeriesInfoProvider.GetSeries();
            }

            if (inputArguments.UseCache)
            {
                query = query.LoadFromCache();
            }

            await writer.WriteAsync(JsonConvert.SerializeObject(await query.ToListAsync()));
        }
    }
}