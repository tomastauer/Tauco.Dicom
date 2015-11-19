using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Represents query to the DICOM server. Initialization is lazy, <see cref="GetEnumerator" /> or
    /// <see cref="ToListAsync" /> has to be called before accessing the output collection.
    /// </summary>
    /// <typeparam name="TInfo">Type of the info the query is constructed for</typeparam>
    internal class DicomQuery<TInfo> : IDicomQuery<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly IDicomDataLoader<TInfo> mDicomDataLoader;
        private bool mUseCache;

        /// <summary>
        /// Initializes new instance of <see cref="DicomQuery{TINfo}" />.
        /// </summary>
        /// <param name="generalizedInfoProvider">Provides method for obtaining single instance of every implementation of <see cref="IDicomInfo"/></param>
        /// <param name="dicomDataLoader">Provides methods for both synchronous and asynchronous load of items from DICOM server</param>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generalizedInfoProvider" /> is <see langword="null" /> -or-
        /// <paramref name="dicomDataLoader" /> is <see langword="null" /> -or-
        /// <paramref name="whereCollection" /> is <see langword="null" />
        /// </exception>
        internal DicomQuery([NotNull] IGeneralizedInfoProvider generalizedInfoProvider, [NotNull] IDicomDataLoader<TInfo> dicomDataLoader,
            [NotNull] IWhereCollection<TInfo> whereCollection)
        {
            if (generalizedInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(generalizedInfoProvider));
            }
            if (dicomDataLoader == null)
            {
                throw new ArgumentNullException(nameof(dicomDataLoader));
            }
            if (whereCollection == null)
            {
                throw new ArgumentNullException(nameof(whereCollection));
            }

            mDicomDataLoader = dicomDataLoader;
            WhereCollection = whereCollection;
        }


        /// <summary>
        /// Gets underlying collection which specifies how the query should be filtered.
        /// </summary>
        public IWhereCollection<TInfo> WhereCollection
        {
            get;
        }


        /// <summary>
        /// Specifies that dicom data will be loaded from the cache while using this <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>Current instance of <see cref="DicomQuery{TInfo}"/></returns>
        public IDicomQuery<TInfo> LoadFromCache()
        {
            mUseCache = true;
            return this;
        }


        /// <summary>
        /// Specifies that dicom data will be loaded from the server while using this <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>Current instance of <see cref="DicomQuery{TInfo}"/></returns>
        public IDicomQuery<TInfo> LoadFromServer()
        {
            mUseCache = false;
            return this;
        } 
        

        /// <summary>
        /// Asynchronously returns an enumerator that iterates through the collection. According to the setting either loads data from the server
        /// or from the cache.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> containing underlying collection.
        /// </returns>
        public async Task<List<TInfo>> ToListAsync()
        {
            var result = new List<TInfo>();

            result.AddRange(mUseCache ? 
                await mDicomDataLoader.LoadDataFromCacheAsync(WhereCollection) : 
                await mDicomDataLoader.LoadDataFromServerAsync(WhereCollection)
            );
            
            return result;
        }


        /// <summary>
        /// Overrides default <see cref="ToList"/> method to avoid <see cref="PureAttribute"/> flag
        /// since this method writes to the cache.
        /// </summary>
        /// <returns>Materialized underlying collection</returns>
        public IEnumerable<TInfo> ToList()
        {
            return Enumerable.ToList(this);
        } 


        #region IEnumerable implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection. According to the setting either loads data from the server
        /// or from the cache.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" />structure for the <see cref="T:DicomQuery" />.
        /// </returns>
        public IEnumerator<TInfo> GetEnumerator()
        {
            var result = new List<TInfo>();

            result.AddRange(mUseCache ? 
                mDicomDataLoader.LoadDataFromCache(WhereCollection) : 
                mDicomDataLoader.LoadDataFromServer(WhereCollection)
            );

            return result.GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection. According to the setting either loads data from the server
        /// or from the cache.
        /// </summary>
        /// <exception cref="T:System.ObjectDisposedException">The current instance of wait handle has already been disposed. </exception>
        /// <exception cref="T:System.Threading.AbandonedMutexException">
        /// The wait completed because a thread exited without releasing a mutex. This exception is
        /// not thrown on Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The current instance is a transparent proxy for a
        /// <see cref="T:System.Threading.WaitHandle" /> in another application domain.
        /// </exception>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" />structure for the <see cref="T:DicomQuery" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}