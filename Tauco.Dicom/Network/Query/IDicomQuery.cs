using System.Collections.Generic;
using System.Threading.Tasks;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    public interface IDicomQuery<TInfo> : IEnumerable<TInfo> where TInfo : IDicomInfo, new()
    {
        /// <summary>
        /// Gets underlying collection which specifies how the query should be filtered.
        /// </summary>
        IWhereCollection<TInfo> WhereCollection
        {
            get;
        }

        
        /// <summary>
        /// Specifies that dicom data will be loaded from the cache while using this <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>Current instance of <see cref="DicomQuery{TInfo}"/></returns>
        IDicomQuery<TInfo> LoadFromCache();


        /// <summary>
        /// Specifies that dicom data will be loaded from the server while using this <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>Current instance of <see cref="DicomQuery{TInfo}"/></returns>
        IDicomQuery<TInfo> LoadFromServer();


        /// <summary>
        /// Asynchronously returns an enumerator that iterates through the collection. According to the setting either loads data from the server
        /// or from the cache.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> containing underlying collection.
        /// </returns>
        Task<List<TInfo>> ToListAsync();
    }
}