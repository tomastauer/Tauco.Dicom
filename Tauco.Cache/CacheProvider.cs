using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Tauco.Cache
{
    /// <summary>
    /// Provides methods for persistent storage of items. Users PersistentDictionary internally.
    /// </summary>
    public class CacheProvider : ICacheProvider
    {
        private const string CACHE_DIRECTORY = "Cache";
        private readonly IPersistentDictionaryFactory mFactory;


        /// <summary>
        /// Initializes new instance of <see cref="CacheProvider"/>.
        /// </summary>
        /// <param name="factory">Factory for obtaining instances of Persistent dictionaries.</param>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is null</exception>
        public CacheProvider([NotNull] IPersistentDictionaryFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            mFactory = factory;
        }


        /// <summary>
        /// Stores given <paramref name="item"/> under the given <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// Persistent dictionary allows only basic types to be stored. Therefore JSON serialization is performed for every item.
        /// </remarks>
        /// <param name="key">Key of the item</param>
        /// <param name="item">Item to be stored</param>
        /// <param name="rewrite">Determines whether already existing record with the same key should be rewrited</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null -or- <paramref name="item"/> is null</exception>
        public void Store(string key, object item, bool rewrite = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var dictionary = mFactory.GetPersistentDictionary(GetDictionaryName(item.GetType()));

            if (dictionary.ContainsKey(key))
            {
                if (!rewrite)
                {
                    return;
                }
                dictionary.Remove(key);
            }

            dictionary.Add(key, JsonConvert.SerializeObject(item));
        }


        /// <summary>
        /// Gets item for the given <paramref name="key"/> from the cache.
        /// </summary>
        /// <remarks>
        /// Persistent dictionary allows only basic types to be stored. Therefore JSON deserialization is performed for every item.
        /// </remarks>
        /// <typeparam name="T">Type of the stored object</typeparam>
        /// <param name="key">Key the item is stored for</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null</exception>
        /// <returns>Typed item obtained from the cache, if such item was found. Default value otherwise</returns>
        public T Retrieve<T>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var dictionary = mFactory.GetPersistentDictionary(GetDictionaryName(typeof(T)));

            if (!dictionary.ContainsKey(key))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(dictionary[key]);
        }


        /// <summary>
        /// Gets all items for the given type from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the stored objects</typeparam>
        /// <returns>Collection of items obtained from the cache</returns>
        public IEnumerable<T> Retrieve<T>()
        {
            var dictionary = mFactory.GetPersistentDictionary(GetDictionaryName(typeof (T)));
            return dictionary.Values.Select(JsonConvert.DeserializeObject<T>);
        }


        /// <summary>
        /// Clears the cache for the given type.
        /// </summary>
        /// <typeparam name="T">Type the cache should be cleared for</typeparam>
        public void ClearCache<T>()
        {
            var dictionary = mFactory.GetPersistentDictionary(GetDictionaryName(typeof(T)));
            dictionary.Clear();
        }


        /// <summary>
        /// Gets name of persistent dictionary.
        /// </summary>
        /// <remarks>
        /// Dictionary name determines physical location of cache files. Cache\{Type name} is used in this case.
        /// </remarks>
        /// <param name="type">Type of the <see langword="object"/> to be stored in the cache.</param>
        /// <returns>Name of dictionary in format Cache/{Type name}</returns>
        private string GetDictionaryName(Type type)
        {
            return $@"{CACHE_DIRECTORY}\{type.Name}";
        }


        /// <summary>
        /// Performs flush of all Persistent dictionaries in use.
        /// </summary>
        public void Dispose()
        {
            mFactory.FlushAll();
            mFactory.Dispose();
        }
    }
}