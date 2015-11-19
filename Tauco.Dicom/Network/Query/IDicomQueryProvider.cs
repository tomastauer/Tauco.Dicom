using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides method for creating new instance of <see cref="DicomQuery{TInfo}" />.
    /// </summary>
    /// <typeparam name="TInfo">Dicom info type the <see cref="DicomQuery{TInfo}" /> should be created for</typeparam>
    internal interface IDicomQueryProvider<TInfo> where TInfo : IDicomInfo, new()
    {
        /// <summary>
        /// Gets new instance of <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>New instance of <see cref="DicomQuery{TInfo}"/></returns>
        DicomQuery<TInfo> GetDicomQuery();
    }
}