using System.Collections.Immutable;
using System.Threading.Tasks;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides methods for both synchronous and asynchronous load of items from DICOM server.
    /// </summary>
    /// <typeparam name="TInfo">Type of the info object to be loaded</typeparam>
    internal interface IDicomDataLoader<TInfo> where TInfo : IDicomInfo
    {
        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from server.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <returns>
        /// Collection containing requested data.
        /// </returns>
        IImmutableList<TInfo> LoadDataFromServer(IWhereCollection<TInfo> whereCollection);


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from server asynchronously.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <returns>
        /// Task containing collection with requested data.
        /// </returns>
        Task<IImmutableList<TInfo>> LoadDataFromServerAsync(IWhereCollection<TInfo> whereCollection);


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from cache.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <returns>
        /// Collection containing requested data.
        /// </returns>
        IImmutableList<TInfo> LoadDataFromCache(IWhereCollection<TInfo> whereCollection);


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from cache asynchronously.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <returns>
        /// Task containing collection with requested data.
        /// </returns>
        Task<IImmutableList<TInfo>> LoadDataFromCacheAsync(IWhereCollection<TInfo> whereCollection);
    }
}