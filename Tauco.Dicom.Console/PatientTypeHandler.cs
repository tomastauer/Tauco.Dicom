using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Tauco.Dicom.Models;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Contains method for handling patient type.
    /// </summary>
    internal class PatientTypeHandler : ITypeHandler
    {
        private readonly IPatientInfoProvider mPatientInfoProvider;
        
        /// <summary>
        /// Instantiates new instance of <see cref="PatientTypeHandler"/>.
        /// </summary>
        /// <param name="patientInfoProvider">Provides methods for retrieving the patients</param>
        /// <exception cref="ArgumentNullException"><paramref name="patientInfoProvider"/> is <see langword="null"/></exception>
        public PatientTypeHandler(IPatientInfoProvider patientInfoProvider)
        {
            if (patientInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(patientInfoProvider));
            }

            mPatientInfoProvider = patientInfoProvider;
        }


        /// <summary>
        /// Handles downloading of patient related data.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <exception cref="ArgumentException">Wrong <see cref="InputArguments.Type"/> in <paramref name="inputArguments"/>. Expected value is 'patient'.</exception>
        /// <returns>Represents an asynchronous operation</returns>
        public async Task HandleTypeAsync(InputArguments inputArguments, TextWriter writer)
        {
            if (!inputArguments.Type.Equals("patient", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Type has to be 'patient'", nameof(inputArguments));
            }

            if (inputArguments.Identifier != null)
            {
                await HandleSinglePatient(inputArguments, writer);
            }
            else
            {
                await HandleAllPatients(inputArguments, writer);
            }
        }


        /// <summary>
        /// Handles retrieving of all patients.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleAllPatients(InputArguments inputArguments, TextWriter writer)
        {
            var query = mPatientInfoProvider.GetPatients();
            if (inputArguments.UseCache)
            {
                query = query.LoadFromCache();
            }

            await writer.WriteAsync(JsonConvert.SerializeObject(await query.ToListAsync()));
        }


        /// <summary>
        /// Handles retrieving of single patient or downloading images for single patient.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        private async Task HandleSinglePatient(InputArguments inputArguments, TextWriter writer)
        {
            var patient = await mPatientInfoProvider.GetPatientByBirthNumberAsync(inputArguments.Identifier, inputArguments.UseCache);

            if (inputArguments.Download)
            {
                await mPatientInfoProvider.DownloadImagesAsync(patient);
            }
            else
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(patient));
            }
        }
    }
}