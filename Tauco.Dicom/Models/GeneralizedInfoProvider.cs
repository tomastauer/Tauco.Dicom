using System;
using System.Collections.Generic;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides method for obtaining single instance of every implementation of <see cref="IDicomInfo"/>. If not such a object exitst, creates a new one.
    /// </summary>
    internal class GeneralizedInfoProvider : IGeneralizedInfoProvider
    {
        private static readonly Dictionary<string, object> mInnerDictionary = new Dictionary<string, object>();


        /// <summary>
        /// Returns instance of <typeparamref name="TInfo"/>.
        /// </summary>
        /// <typeparam name="TInfo">Object info to be obtained</typeparam>
        /// <returns>Instance of <typeparamref name="TInfo"/></returns>
        public TInfo GetGeneralizedInfo<TInfo>() where TInfo : IDicomInfo
        {
            var typeName = typeof (TInfo).FullName;
            
            if (mInnerDictionary.ContainsKey(typeName))
            {
                return (TInfo)mInnerDictionary[typeName];
            }

            var info = Activator.CreateInstance<TInfo>();
            mInnerDictionary.Add(typeName, info);

            return info;
        }
    }
}
