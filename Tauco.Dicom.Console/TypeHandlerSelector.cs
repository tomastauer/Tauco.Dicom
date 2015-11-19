using System;

using Tauco.Dicom.Models;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Provides method for selecting proper implementation of <see cref="ITypeHandler"/> based on given type.
    /// </summary>
    internal class TypeHandlerSelector : ITypeHandlerSelector
    {
        private readonly IPatientInfoProvider mPatientInfoProvider;
        private readonly IStudyInfoProvider mStudyInfoProvider;
        private readonly ISeriesInfoProvider mSeriesInfoProvider;

        /// <summary>
        /// Instantiates new instance of <see cref="TypeHandlerSelector"/>.
        /// </summary>
        /// <param name="patientInfoProvider">Provides methods for retrieving the patients</param>
        /// <param name="studyInfoProvider">Provides methods for retrieving the studies</param>
        /// <param name="seriesInfoProvider">Provides methods for retrieving the series</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="patientInfoProvider"/> is <see langword="null"/> -or- 
        /// <paramref name="studyInfoProvider"/> is <see langword="null"/> -or- 
        /// <paramref name="seriesInfoProvider"/> is <see langword="null"/>
        /// </exception>
        public TypeHandlerSelector(IPatientInfoProvider patientInfoProvider, IStudyInfoProvider studyInfoProvider, ISeriesInfoProvider seriesInfoProvider)
        {
            if (patientInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(patientInfoProvider));
            }
            if (studyInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(studyInfoProvider));
            }
            if (seriesInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(seriesInfoProvider));
            }

            mPatientInfoProvider = patientInfoProvider;
            mStudyInfoProvider = studyInfoProvider;
            mSeriesInfoProvider = seriesInfoProvider;
        }


        /// <summary>
        /// Selects correct implementation of <see cref="ITypeHandler"/> for given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of handler to be selected</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Unsupported <paramref name="type"/>. Has to be one of the following values: 'patient', 'study', 'series'.</exception>
        /// <returns>Implementation of <see cref="ITypeHandler"/> for given <paramref name="type"/></returns>
        public ITypeHandler SelectTypeHandler(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Equals("patient", StringComparison.OrdinalIgnoreCase))
            {
                return new PatientTypeHandler(mPatientInfoProvider);
            }
            if (type.Equals("study", StringComparison.OrdinalIgnoreCase))
            {
                return new StudyTypeHandler(mStudyInfoProvider, mPatientInfoProvider);
            }
            if (type.Equals("series", StringComparison.OrdinalIgnoreCase))
            {
                return new SeriesTypeHandler(mSeriesInfoProvider, mStudyInfoProvider);
            }

            throw new ArgumentException("Unsupported type. Has to be one of the following values: 'patient', 'study', 'series'.", nameof(type));
        }
    }
}