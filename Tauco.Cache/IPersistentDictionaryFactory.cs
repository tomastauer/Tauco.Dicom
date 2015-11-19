using System;
using System.Collections.Generic;

namespace Tauco.Cache
{
    /// <summary>
    /// Factory for obtaining instances of Persistent dictionaries.
    /// </summary>
    public interface IPersistentDictionaryFactory : IDisposable
    {
        /// <summary>
        /// Gets persistent dictionary instance for given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of persistence dictionary</param>
        /// <returns>Persistence dictionary for given <paramref name="key"/></returns>
        IDictionary<string, string> GetPersistentDictionary(string key);


        /// <summary>
        /// Flushes all persistent dictionaries causing they save its content to the file system.
        /// </summary>
        void FlushAll();
    }
}