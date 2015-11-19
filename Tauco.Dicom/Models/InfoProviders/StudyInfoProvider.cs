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
    /// Provides methods for retrieving the studies.
    /// </summary>
    internal class StudyInfoProvider : IStudyInfoProvider
    {
        private readonly IDicomDownloader<StudyInfo> mDicomDownloader;
        private readonly IDicomQueryProvider<StudyInfo> mDicomQueryProvider;


        /// <summary>
        /// Instantiates new instance of <see cref="StudyInfoProvider"/>
        /// </summary>
        /// <param name="dicomQueryProvider">Provides method for creating new instance of <see cref="DicomQuery{StudyInfo}" /></param>
        /// <param name="dicomDownloader">Provides method for downloading files from the DICOM server</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomQueryProvider"/> is <see langword="null"/> -or- <paramref name="dicomDownloader"/> is <see langword="null"/></exception>
        public StudyInfoProvider([NotNull] IDicomQueryProvider<StudyInfo> dicomQueryProvider, [NotNull] IDicomDownloader<StudyInfo> dicomDownloader)
        {
            if (dicomQueryProvider == null)
            {
                throw new ArgumentNullException(nameof(dicomQueryProvider));
            }
            if (dicomDownloader == null)
            {
                throw new ArgumentNullException(nameof(dicomDownloader));
            }

            mDicomQueryProvider = dicomQueryProvider;
            mDicomDownloader = dicomDownloader;
        }


        /// <summary>
        /// Gets all studies.
        /// </summary>
        /// <returns>Collecion containing all the studies</returns>
        public IDicomQuery<StudyInfo> GetStudies()
        {
            return mDicomQueryProvider.GetDicomQuery();
        }


        /// <summary>
        /// Gets all studies for the given patient.
        /// </summary>
        /// <param name="patient">Patient the studies are related to</param>
        /// <exception cref="ArgumentNullException"><paramref name="patient"/> is null</exception>
        /// <returns>Collection containing all the studies of the patient</returns>
        public IDicomQuery<StudyInfo> GetStudiesForPatient([NotNull] PatientInfo patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
           
            return mDicomQueryProvider.GetDicomQuery()
                .WhereEquals(DicomTags.PatientID, patient.PatientID.StringRepresentationWithoutSlash)
                .WhereEquals(DicomTags.PatientID, patient.PatientID.StringRepresentationWithSlash);
        }


        /// <summary>
        /// Gets single study by given identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Identifier of the study</param>
        /// <param name="loadFromCache">Determines whether the <see cref="StudyInfo"/> should be loaded from cache or from server</param>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/></exception>
        /// <returns>Studies with given identifier, if such study exists; otherwise, null</returns>
        public async Task<StudyInfo> GetStudyByIDAsync([NotNull] InfoIdentifier identifier, bool loadFromCache = false)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var query = GetStudies();
            if (loadFromCache)
            {
                query = query.LoadFromCache();
            }

            return (await query
                .WhereEquals(DicomTags.StudyInstanceUID, identifier.StringRepresentation)
                .ToListAsync()).FirstOrDefault();
        }


        /// <summary>
        /// Performs downloading of images related to the given study.
        /// </summary>
        /// <param name="study">Study the images are downloaded for</param>
        /// <param name="downloadAction">Specifies action which will be performed once server gets the file</param>
        /// <exception cref="ArgumentNullException"><paramref name="study"/> is null</exception>
        public async Task DownloadImagesAsync(StudyInfo study, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction = null)
        {
            if (study == null)
            {
                throw new ArgumentNullException(nameof(study));
            }

            await mDicomDownloader.DownloadAsync(study.StudyInstanceUID, downloadAction);
        }
    }
}
