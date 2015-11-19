using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Provides mapping from DICOM properties to ObjectInfo properties.
    /// </summary>
    public class DicomMapping : IDicomMapping
    {
        private readonly Dictionary<PropertyInfo, DicomTags> mMapping = new Dictionary<PropertyInfo, DicomTags>();


        /// <summary>
        /// Initialize new instance of <see cref="DicomMapping"/>. Lists all properties with DicomAttribute specified and adds them to mapping dictionary.
        /// </summary>
        /// <exception cref="ArgumentException">No property was found in given <paramref name="infoObject"/></exception>
        public DicomMapping(Type infoObject)
        {
            var properties = infoObject.GetProperties();
            if (!properties.Any())
            {
                throw new ArgumentException("No property was found", nameof(infoObject));
            }

            properties.ToList().ForEach(property =>
            {
                var dicomTag = GetDicomTag(property);

                if(dicomTag != DicomTags.Undefined){
                    mMapping.Add(property, dicomTag);
                }
            });
        }


        /// <summary>
        /// Tries to find DicomAttribute within given propertyInfo.
        /// </summary>
        /// <param name="propertyInfo">Property member the attribute will be found for</param>
        /// <returns>True, if attribute was successfully found; otherwise, false</returns>
        private DicomTags GetDicomTag([NotNull] PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DicomAttribute>();
            return attribute?.DicomTag ?? DicomTags.Undefined;
        }


        #region IEnumerable implementation

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:DicomMapping"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator"/> structure for the <see cref="T:DicomMapping"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<PropertyInfo, DicomTags>> GetEnumerator()
        {
            return mMapping.GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:DicomMapping"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator"/> structure for the <see cref="T:DicomMapping"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}