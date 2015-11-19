using System;
using System.Linq;
using System.Reflection;

namespace Tauco.Cache
{
    /// <summary>
    /// Provides method for determining cache index of objects based on the presence of <see cref="CacheIndexAttribute"/>.
    /// </summary>
    public class CacheIndexProvider : ICacheIndexProvider
    {
        /// <summary>
        /// Gets storage index that will be used for caching the given <paramref name="item" />.
        /// </summary>
        /// <remarks>
        /// Item has to have at least on property decorated with the <see cref="CacheIndexAttribute" />.
        /// </remarks>
        /// <param name="item">Item to be cached</param>
        /// <returns>Value of the property decorated with cache index attribute</returns>
        /// <exception cref="ArgumentException"><paramref name="item"/> has to have at least one property with CacheIndexAttribute defined</exception>
        public string GetCacheIndex(object item)
        {
            var properties = item.GetType().GetProperties().Where(
               prop => Attribute.IsDefined(prop, typeof(CacheIndexAttribute))).ToList();

            if (!properties.Any())
            {
                throw new ArgumentException("There has to be at least one property with CacheIndexAttribute defined.", nameof(item));
            }

            PropertyInfo property = properties.First();

            return property.GetValue(item)?.ToString();
        }
    }
}
