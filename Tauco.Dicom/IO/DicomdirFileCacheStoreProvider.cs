using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Cache;
using Tauco.Dicom.Abstraction;
using Tauco.Dicom.Shared;

namespace Tauco.Dicom.IO
{
    /// <summary>
    /// Provides method for storing all the <see cref="PatientInfo"/>, <see cref="StudyInfo"/>, <see cref="SeriesInfo"/> and <see cref="ImageInfo"/> obtained from the given 
    /// DICOMDIR file in the cache. 
    /// </summary>
    /// <remarks>
    /// Once the objects are cached, they can be accessed in the standard info provider way.
    /// </remarks>
    internal class DicomdirFileCacheStoreProvider : IDicomdirFileCacheStoreProvider
    {
        private readonly IDicomdirFileParser mDicomDirFileParser;
        private readonly ICacheProvider mCacheProvider;
        private readonly ICacheIndexProvider mCacheIndexProvider;


        /// <summary>
        /// Instantiates new instance of <see cref="DicomdirFileCacheStoreProvider"/>.
        /// </summary>
        /// <param name="dicomDirFileParser">Provides method for parsing given dicomdir file to collections for every corresponding info object</param>
        /// <param name="cacheProvider">Provides methods for persistent storage of items</param>
        /// <param name="cacheIndexProvider">Provides method for determining cache index of objects</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dicomDirFileParser"/> is <see langword="null"/> -or- 
        /// <paramref name="cacheProvider"/> is <see langword="null"/> -or- 
        /// <paramref name="cacheIndexProvider"/> is <see langword="null"/>
        /// </exception>
        public DicomdirFileCacheStoreProvider([NotNull] IDicomdirFileParser dicomDirFileParser, [NotNull] ICacheProvider cacheProvider, [NotNull] ICacheIndexProvider cacheIndexProvider)
        {
            if (dicomDirFileParser == null)
            {
                throw new ArgumentNullException(nameof(dicomDirFileParser));
            }
            if (cacheProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }
            if (cacheIndexProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheIndexProvider));
            }

            mDicomDirFileParser = dicomDirFileParser;
            mCacheProvider = cacheProvider;
            mCacheIndexProvider = cacheIndexProvider;
        }


        /// <summary>
        /// Parses given <paramref name="dicomdirPath"/> and stores all the parsed <see cref="PatientInfo"/>, <see cref="StudyInfo"/>, <see cref="SeriesInfo"/> and <see cref="ImageInfo"/> into the cache.
        /// </summary>
        /// <param name="dicomdirPath">Path leading to the dicomdir file</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomdirPath"/> is null</exception>
        /// <returns>Object containing all the parsed data</returns>
        public async Task<DicomdirInfos> StoreItemsInCache([NotNull] string dicomdirPath)
        {
            if (dicomdirPath == null)
            {
                throw new ArgumentNullException(nameof(dicomdirPath));
            }

            var parsedData = await mDicomDirFileParser.ParseDicomdirAsync(dicomdirPath);

            StoreCollectionInCache(parsedData.Patients);
            StoreCollectionInCache(parsedData.Studies);
            StoreCollectionInCache(parsedData.Series);

            return parsedData;
        }


        /// <summary>
        /// Stores all items from the given <paramref name="collection"/> into the cache.
        /// </summary>
        /// <param name="collection">Collection containing objects to be stored in the cache</param>
        private void StoreCollectionInCache(IEnumerable<IDicomInfo> collection)
        {
            foreach (var info in collection)
            {
                mCacheProvider.Store(mCacheIndexProvider.GetCacheIndex(info), info, true);
            }
        }
    }
}
