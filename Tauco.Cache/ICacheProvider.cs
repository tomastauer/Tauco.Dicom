using System;
using System.Collections.Generic;

namespace Tauco.Cache
{
    /// <summary>
    /// Provides methods for persistent storage of items.
    /// </summary>
    public interface ICacheProvider: IDisposable
    {
        /// <summary>
        /// Stores given <paramref name="item"/> under the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="item">Item to be stored</param>
        /// <param name="rewrite">Determines whether already existing record with the same key should be rewrited</param>
        void Store(string key, object item, bool rewrite = false);


        /// <summary>
        /// Gets item for the given <paramref name="key"/> from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the stored <see langword="object"/></typeparam>
        /// <param name="key">Key the item is stored for</param>
        /// <returns>Typed item obtained from the cache, if such item was found. Default value otherwise</returns>
        T Retrieve<T>(string key);


        /// <summary>
        /// Gets all items for the given type from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the stored objects</typeparam>
        /// <returns>Collection of items obtained from the cache</returns>
        IEnumerable<T> Retrieve<T>();


        /// <summary>
        /// Clears the cache for the given type.
        /// </summary>
        /// <typeparam name="T">Type the cache should be cleared for</typeparam>
        void ClearCache<T>();
    }
}