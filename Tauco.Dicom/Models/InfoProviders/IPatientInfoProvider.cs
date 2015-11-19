using System.Threading.Tasks;

using Tauco.Dicom.Network;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides methods for retrieving the patients.
    /// </summary>
    public interface IPatientInfoProvider : IDownloadProvider<PatientInfo>
    {
        /// <summary>
        /// Gets all patients.
        /// </summary>
        /// <returns>Collection containing all the patients</returns>
        IDicomQuery<PatientInfo> GetPatients();


        /// <summary>
        /// Gets single patient by given <paramref name="birthNumber"/> asynchronously.
        /// </summary>
        /// <param name="birthNumber">Birth number of the patient</param>
        /// <param name="loadFromCache">Determines whether the <see cref="PatientInfo"/> should be loaded from cache or from server</param>
        /// <returns>Patient with given <paramref name="birthNumber"/>, if such patient exists; otherwise, <see langword="null"/></returns>
        Task<PatientInfo> GetPatientByBirthNumberAsync(string birthNumber, bool loadFromCache = false);
    }
}