using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Castle.Core.Internal;

using JetBrains.Annotations;

using Tauco.Cache;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;
using Tauco.Dicom.Utilities;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides methods for both synchronous and asynchronous load of items from DICOM server.
    /// </summary>
    /// <typeparam name="TInfo">Type of the info object to be loaded</typeparam>
    internal class DicomDataLoader<TInfo> : IDicomDataLoader<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly ICacheIndexProvider mCacheIndexProvider;
        private readonly ICacheProvider mCacheProvider;
        private readonly IDicomClientFactory<TInfo> mDicomClientFactory;
        private readonly IDicomRequestFactory mDicomRequestFactory;
        private AsyncHashSet<TInfo> mInnerAsyncHashSet;
        private bool mIsLiveDataDownloadPending;


        /// <summary>
        /// Initializes new instance of <see cref="DicomDataLoader{TInfo}" /> .
        /// </summary>
        /// <param name="cacheProvider">Provides methods for persistent storage of items</param>
        /// <param name="cacheIndexProvider">Provides method for determining cache index of objects</param>
        /// <param name="dicomClientFactory">Provides method for obtaining new instance of <see cref="IDicomClient{TInfo}"/> implementation</param>
        /// <param name="dicomRequestFactory">
        /// Contains method for creating new instance of DicomRequest.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cacheProvider" /> is <see langword="null" /> -or- <paramref name="cacheIndexProvider" /> is
        /// <see langword="null" /> -or-
        /// <paramref name="dicomClientFactory" /> is <see langword="null" /> -or- <paramref name="dicomRequestFactory" /> is
        /// <see langword="null" />
        /// </exception>
        public DicomDataLoader([NotNull] ICacheProvider cacheProvider, [NotNull] ICacheIndexProvider cacheIndexProvider, [NotNull] IDicomClientFactory<TInfo> dicomClientFactory,
            [NotNull] IDicomRequestFactory dicomRequestFactory)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }
            if (cacheIndexProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheIndexProvider));
            }
            if (dicomClientFactory == null)
            {
                throw new ArgumentNullException(nameof(dicomClientFactory));
            }
            if (dicomRequestFactory == null)
            {
                throw new ArgumentNullException(nameof(dicomRequestFactory));
            }
      
            mCacheProvider = cacheProvider;
            mCacheIndexProvider = cacheIndexProvider;
            mDicomClientFactory = dicomClientFactory;
            mDicomRequestFactory = dicomRequestFactory;
        }


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from server.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <exception cref="ArgumentNullException"><paramref name="whereCollection"/> is <see langword="null" /></exception>
        /// <returns>
        /// Collection containing requested data.
        /// </returns>
        public IImmutableList<TInfo> LoadDataFromServer([NotNull] IWhereCollection<TInfo> whereCollection)
        {
            if (whereCollection == null)
            {
                throw new ArgumentNullException(nameof(whereCollection));
            }

            LoadFromServer(whereCollection).Wait();

            return mInnerAsyncHashSet.GetConsumingEnumerable().ToImmutableList();
        }


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> asynchronously.
        /// </summary>
        /// <remarks>
        /// At first items are loaded from cache. After obtained result from server, they are rewritten with newer value.
        /// </remarks>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <exception cref="ArgumentNullException"><paramref name="whereCollection"/> is <see langword="null" /></exception>
        /// <returns>
        /// Task containing collection with requested data.
        /// </returns>
        public async Task<IImmutableList<TInfo>> LoadDataFromServerAsync([NotNull] IWhereCollection<TInfo> whereCollection)
        {
            if (whereCollection == null)
            {
                throw new ArgumentNullException(nameof(whereCollection));
            }

            await LoadFromServer(whereCollection);

            return mInnerAsyncHashSet.GetConsumingEnumerable().ToImmutableList();
        }


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from cache.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <exception cref="ArgumentNullException"><paramref name="whereCollection"/> is <see langword="null" /></exception>
        /// <returns>
        /// Collection containing requested data.
        /// </returns>
        public IImmutableList<TInfo> LoadDataFromCache(IWhereCollection<TInfo> whereCollection)
        {
            if (whereCollection == null)
            {
                throw new ArgumentNullException(nameof(whereCollection));
            }

            return LoadFromCache(whereCollection).ToImmutableList();
        }


        /// <summary>
        /// Loads items matching conditions in given <paramref name="whereCollection" /> from cache asynchronously.
        /// </summary>
        /// <param name="whereCollection">Collection containing constraints used for filtering requested items</param>
        /// <exception cref="ArgumentNullException"><paramref name="whereCollection"/> is <see langword="null" /></exception>
        /// <returns>
        /// Task containing collection with requested data.
        /// </returns>
        public Task<IImmutableList<TInfo>> LoadDataFromCacheAsync([NotNull] IWhereCollection<TInfo> whereCollection)
        {
            if (whereCollection == null)
            {
                throw new ArgumentNullException(nameof(whereCollection));
            }

            return Task.Factory.StartNew(() => LoadDataFromCache(whereCollection));
        }


        /// <summary>
        /// Loads all items from the cache matching the <see cref="whereCollection" /> and put them into asynchronous collection.
        /// </summary>
        private IEnumerable<TInfo> LoadFromCache(IWhereCollection<TInfo> whereCollection)
        {
            mInnerAsyncHashSet = new AsyncHashSet<TInfo>();

            mCacheProvider.Retrieve<TInfo>().Where(whereCollection.Predicate).ForEach(item =>
            {
                mInnerAsyncHashSet.Add(item);
            });

            mCacheProvider.Dispose();   
            return mInnerAsyncHashSet.GetConsumingEnumerable();
        }


        /// <summary>
        /// Start loading all items from the server matching the <see cref="whereCollection" /> conditions.
        /// </summary>
        /// <returns>
        /// Task completed once all items are loaded.
        /// </returns>
        private async Task LoadFromServer(IWhereCollection<TInfo> whereCollection)
        {
            mInnerAsyncHashSet = new AsyncHashSet<TInfo>();

            if (!mIsLiveDataDownloadPending)
            {
                mIsLiveDataDownloadPending = true;
                await GetResponse(whereCollection).ContinueWith(task =>
                {
                    mCacheProvider.Dispose();
                    mIsLiveDataDownloadPending = false;
                });
            }
        }


        /// <summary>
        /// Sends request asynchronously.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Underlying instance of <see cref="IDicomClient{TInfo}" /> was disposed when it was accessed
        /// </exception>
        private async Task GetResponse(IWhereCollection<TInfo> whereCollection)
        {
            using (var dicomClient = mDicomClientFactory.CreateDicomClient())
            {
                foreach (var dicomWhereCollection in whereCollection.GetDicomWhereCollections())
                {
                    dicomClient.AddFindRequest(mDicomRequestFactory.CreateDicomFindRequest<TInfo>(dicomWhereCollection, ResponseLoaded));
                }

                await dicomClient.SendAsync();
            }
        }


        /// <summary>
        /// Adds loaded item to the asynchronous collection and tries to store it in the cache.
        /// </summary>
        /// <param name="responseItem">Loaded item</param>
        private void ResponseLoaded(TInfo responseItem)
        {
            mInnerAsyncHashSet.Add(responseItem);
            mCacheProvider.Store(mCacheIndexProvider.GetCacheIndex(responseItem), responseItem, true);
        }
    }
}