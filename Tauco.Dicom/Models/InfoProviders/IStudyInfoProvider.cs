using System.Threading.Tasks;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides methods for retrieving the studies.
    /// </summary>
    public interface IStudyInfoProvider : IDownloadProvider<StudyInfo>
    {
        /// <summary>
        /// Gets all studies.
        /// </summary>
        /// <returns>Collecion containing all the studies</returns>
        IDicomQuery<StudyInfo> GetStudies();

        
        /// <summary>
        /// Gets all studies for the given patient.
        /// </summary>
        /// <param name="patient">Patient the studies are related to</param>
        /// <returns>Collection containing all the studies of the patient</returns>
        IDicomQuery<StudyInfo> GetStudiesForPatient(PatientInfo patient);


        /// <summary>
        /// Gets single study by given identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Identifier of the study</param>
        /// <param name="loadFromCache">Determines whether the <see cref="StudyInfo"/> should be loaded from cache or from server</param>
        /// <returns>Studies with given identifier, if such study exists; otherwise, null</returns>
        Task<StudyInfo> GetStudyByIDAsync(InfoIdentifier identifier, bool loadFromCache = false);
    }
}