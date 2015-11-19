using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides method for obtaining new instance of <see cref="IDicomClient{TInfo}"/> implementation.
    /// </summary>
    public interface IDicomClientFactory<TInfo> where TInfo : IDicomInfo, new()
    {
        /// <summary>
        /// Creates new instance of <see cref="IDicomClient{TInfo}"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IDicomClient{TInfo}"/></returns>
        IDicomClient<TInfo> CreateDicomClient();
    }
}