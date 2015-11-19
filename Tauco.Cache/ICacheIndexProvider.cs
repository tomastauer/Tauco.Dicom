namespace Tauco.Cache
{
    /// <summary>
    /// Provides method for determining cache index of objects.
    /// </summary>
    public interface ICacheIndexProvider
    {
        /// <summary>
        /// Gets storage index that will be used for caching the given <paramref name="item" />.
        /// </summary>
        /// <param name="item">Item to be cached</param>
        /// <returns>Value specifying cache index of given <paramref name="item"/></returns>
        string GetCacheIndex(object item);
    }
}