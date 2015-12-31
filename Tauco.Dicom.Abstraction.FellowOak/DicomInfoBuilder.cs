using System;
using System.Reflection;

using AutoMapper;

using Dicom;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for creating strongly typed instance of <see cref="IDicomInfo"/> from given dataset.
    /// </summary>
    internal class DicomInfoBuilder : IDicomInfoBuilder
    {
        private readonly IMappingEngine mMappingEngine;
        private readonly IDicomTagAdapter mDicomTagAdapter;
        
        /// <summary>
        /// Instantiates new instance of <see cref="DicomInfoBuilder"/>.
        /// </summary>
        /// <param name="mappingEngine">Performs mapping based on configuration</param>
        /// <param name="dicomTagAdapter">Provides method for obtaining third party DICOM tag representation from the <see cref="DicomTags"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="mappingEngine"/> is <see langword="null" /> -or- <paramref name="dicomTagAdapter"/> is <see langword="null" /></exception>
        public DicomInfoBuilder([NotNull] IMappingEngine mappingEngine, [NotNull] IDicomTagAdapter dicomTagAdapter)
        {
            if (mappingEngine == null)
            {
                throw new ArgumentNullException(nameof(mappingEngine));
            }
            if (dicomTagAdapter == null)
            {
                throw new ArgumentNullException(nameof(dicomTagAdapter));
            }

            mMappingEngine = mappingEngine;
            mDicomTagAdapter = dicomTagAdapter;
        }


        /// <summary>
        /// Builds strongly typed instance of <typeparamref name="TInfo"/> from given <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TInfo">Type of strongly built instance</typeparam>
        /// <param name="source">Source dataset</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        /// <returns>Strongly typed instance of <typeparamref name="TInfo"/> built from given <paramref name="source"/>.</returns>
        public TInfo BuildInfo<TInfo>([NotNull] object source) where TInfo : IDicomInfo, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataset = source as DicomDataset;
            if (dataset == null)
            {
                throw new ArgumentException("Given source has to be convertible to DicomDataset", nameof(source));
            }
            
            TInfo resultItem = new TInfo();
            Type resultType = resultItem.GetType();
            var mDicomMapping = new DicomMapping(typeof(TInfo));
            
            foreach (var item in mDicomMapping)
            {
                PropertyInfo propertyInfo = resultType.GetProperty(item.Key.Name);
                object result = mMappingEngine.DynamicMap(dataset.Get<string>((DicomTag)mDicomTagAdapter.GetDicomTag(item.Value)), typeof(string),
                    propertyInfo.PropertyType);

                propertyInfo.SetValue(resultItem, result);
            }

            return resultItem;
        }
    }
}
