using System;

namespace Tauco.Cache
{
    /// <summary>
    /// <see cref="Attribute"/> determining the decorated property should be considered as index for the caching purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CacheIndexAttribute : Attribute
    {
    }
}
