using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides methods for retrieving the patients.
    /// </summary>
    internal class PatientInfoProvider : IPatientInfoProvider
    {
        private readonly IDicomQueryProvider<PatientInfo> mDicomQueryProvider;
        private readonly IStudyInfoProvider mStudyInfoProvider;
        private readonly IBirthNumberParser mBirthNumberParser;


        /// <summary>
        /// Initializes new instance of <see cref="PatientInfoProvider"/>.
        /// </summary>
        /// <param name="dicomQueryProvider">Provides method for creating new instance of <see cref="DicomQuery{PatientInfo}" /></param>
        /// <param name="studyInfoProvider">Provides ability to download images from the server</param>
        /// <param name="birthNumberParser">Service for parsing czech birth numbers</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomQueryProvider"/> is <see langword="null"/> -or- <paramref name="studyInfoProvider"/> is <see langword="null"/>-or- <paramref name="birthNumberParser"/> is <see langword="null"/></exception>
        public PatientInfoProvider([NotNull] IDicomQueryProvider<PatientInfo> dicomQueryProvider, [NotNull] IStudyInfoProvider studyInfoProvider, [NotNull] IBirthNumberParser birthNumberParser)
        {
            if (dicomQueryProvider == null)
            {
                throw new ArgumentNullException(nameof(dicomQueryProvider));
            }
            if (studyInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(studyInfoProvider));
            }
            if (birthNumberParser == null)
            {
                throw new ArgumentNullException(nameof(birthNumberParser));
            }

            mDicomQueryProvider = dicomQueryProvider;
            mStudyInfoProvider = studyInfoProvider;
            mBirthNumberParser = birthNumberParser;
        }


        /// <summary>
        /// Gets all patients.
        /// </summary>
        /// <returns>Collection containing all the patients</returns>
        public IDicomQuery<PatientInfo> GetPatients()
        {
            return mDicomQueryProvider.GetDicomQuery();
        }


        /// <summary>
        /// Gets single patient by given <paramref name="birthNumber"/>.
        /// </summary>
        /// <param name="birthNumber">Birth number of the patient</param>
        /// <param name="loadFromCache">Determines whether the <see cref="PatientInfo"/> should be loaded from cache or from server</param>
        /// <exception cref="ArgumentNullException"><paramref name="birthNumber" /> is null</exception>
        /// <returns>Patient with given <paramref name="birthNumber"/>, if such patient exists; otherwise, <see langword="null"/></returns>
        public async Task<PatientInfo> GetPatientByBirthNumberAsync([NotNull] string birthNumber, bool loadFromCache = false)
        {
            if (birthNumber == null)
            {
                throw new ArgumentNullException(nameof(birthNumber));
            }

            var birthNumberParsed = mBirthNumberParser.GetBirthNumber(birthNumber);
            if (birthNumberParsed == null)
            {
                throw new ArgumentException("Given birth number is in invalid format", nameof(birthNumber));    
            }

            var query = GetPatients();
            if (loadFromCache)  
            {
                query = query.LoadFromCache();
            }

            var result = await query
                .WhereEquals(DicomTags.PatientID, birthNumberParsed.StringRepresentationWithoutSlash)
                .WhereEquals(DicomTags.PatientID, birthNumberParsed.StringRepresentationWithSlash)
                .ToListAsync();

            return result.FirstOrDefault();
        }


        /// <summary>
        /// Performs downloading of images related to the given <paramref name="patient"/>.
        /// </summary>
        /// <param name="patient">Patient the images are downloaded for</param>
        /// <param name="downloadAction">
        /// Specifies what action should be made with the downloaded image. 
        /// If left blank, files will be downloaded to the default location
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="patient"/> is <see langword="null"/></exception>
        public async Task DownloadImagesAsync(PatientInfo patient, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction = null)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            var studies = await mStudyInfoProvider.GetStudiesForPatient(patient).ToListAsync();
            foreach (var study in studies)
            {
                await mStudyInfoProvider.DownloadImagesAsync(study, downloadAction);
            }
        }
    }
}