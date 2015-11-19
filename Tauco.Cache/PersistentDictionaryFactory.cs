using System;
using System.Collections.Generic;

using Microsoft.Isam.Esent.Collections.Generic;

namespace Tauco.Cache
{
    /// <summary>
    /// Factory for obtaining instances of Persistent dictionaries.
    /// </summary>
    public class PersistentDictionaryFactory : IPersistentDictionaryFactory
    {
        private readonly Lazy<Dictionary<string, PersistentDictionary<string, string>>> mDictionaries = new Lazy<Dictionary<string, PersistentDictionary<string, string>>>();


        /// <summary>
        /// Gets persistent dictionary instance for given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of persistence dictionary</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null</exception>
        /// <returns>Persistence dictionary for given key</returns>
        public IDictionary<string, string> GetPersistentDictionary(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var dictionaries = mDictionaries.Value;

            if (!dictionaries.ContainsKey(key))
            {
                dictionaries.Add(key, new PersistentDictionary<string, string>(key));
            }
            return dictionaries[key];
        }


        /// <summary>
        /// Flushes all persistent dictionaries causing they save its content to the file system.
        /// </summary>
        public void FlushAll()
        {
            foreach (var dictionary in mDictionaries.Value)
            {
                dictionary.Value.Flush();
            }
        }


        /// <summary>
        /// Performs disposing of all Persistent dictionaries in use.
        /// </summary>
        public void Dispose()
        {
            foreach (var dictionary in mDictionaries.Value)
            {
                dictionary.Value.Dispose();
            }

            mDictionaries.Value.Clear();
        }
    }
}
