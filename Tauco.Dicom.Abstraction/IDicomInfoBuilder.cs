using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides method for creating strongly typed instance of <see cref="IDicomInfo"/> from given dataset.
    /// </summary>
    public interface IDicomInfoBuilder
    {
        /// <summary>
        /// Builds strongly typed instance of <typeparamref name="TInfo"/> from given <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TInfo">Type of strongly built instance</typeparam>
        /// <param name="source">Source dataset</param>
        /// <returns>Strongly typed instance of <typeparamref name="TInfo"/> built from given <paramref name="source"/>.</returns>
        TInfo BuildInfo<TInfo>(object source) where TInfo : IDicomInfo, new();
    }
}
