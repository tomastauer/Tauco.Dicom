using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Contains method for handling study type.
    /// </summary>
    internal class StudyTypeHandler : ITypeHandler
    {
        private readonly IStudyInfoProvider mStudyInfoProvider;
        private readonly IPatientInfoProvider mPatientInfoProvider;


        /// <summary>
        /// Instantiates new instance of <see cref="StudyTypeHandler"/>.
        /// </summary>
        /// <param name="studyInfoProvider">Provides methods for retrieving the studies</param>
        /// <param name="patientInfoProvider">Provides methods for retrieving the patients</param>
        /// <exception cref="ArgumentNullException"><paramref name="studyInfoProvider"/> is <see langword="null"/> -or- <paramref name="patientInfoProvider"/> is <see langword="null"/></exception>
        public StudyTypeHandler(IStudyInfoProvider studyInfoProvider, IPatientInfoProvider patientInfoProvider)
        {
            if (studyInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(studyInfoProvider));
            }
            if (patientInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(patientInfoProvider));
            }

            mStudyInfoProvider = studyInfoProvider;
            mPatientInfoProvider = patientInfoProvider;
        }



        /// <summary>
        /// Handles downloading of study related data.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <exception cref="ArgumentException">Wrong <see cref="InputArguments.Type"/> in <paramref name="inputArguments"/>. Expected value is 'study'.</exception>
        /// <returns>Represents an asynchronous operation</returns>
        public async Task HandleTypeAsync(InputArguments inputArguments, TextWriter writer)
        {
            if (!inputArguments.Type.Equals("study", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Type has to be 'study'", nameof(inputArguments));
            }

            if (inputArguments.Identifier != null)
            {
                await HandleSingleStudy(inputArguments, writer);
            }
            else
            {
                await HandleMultipleStudies(inputArguments, writer);
            }
        }


        /// <summary>
        /// Handles retrieving of multiple studies.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleMultipleStudies(InputArguments inputArguments, TextWriter writer)
        {
            IDicomQuery<StudyInfo> query;
            if (inputArguments.ParentIdentifier != null)
            {
                var patient = await mPatientInfoProvider.GetPatientByBirthNumberAsync(inputArguments.ParentIdentifier, inputArguments.UseCache);
                query = mStudyInfoProvider.GetStudiesForPatient(patient);
            }
            else
            {
                query = mStudyInfoProvider.GetStudies();
            }

            if (inputArguments.UseCache)
            {
                query = query.LoadFromCache();
            }

            await writer.WriteAsync(JsonConvert.SerializeObject(await query.ToListAsync()));
        }


        /// <summary>
        /// Handles retrieving of single study or downloading images for single study.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleSingleStudy(InputArguments inputArguments, TextWriter writer)
        {
            var study = await mStudyInfoProvider.GetStudyByIDAsync(inputArguments.Identifier, inputArguments.UseCache);

            if (inputArguments.Download)
            {
                await mStudyInfoProvider.DownloadImagesAsync(study);
            }
            else
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(study));
            }
        }
    }
}