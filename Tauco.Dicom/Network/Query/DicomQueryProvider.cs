using System;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides method for creating new instance of <see cref="DicomQuery{TInfo}" />.
    /// </summary>
    /// <typeparam name="TInfo">Dicom info type the <see cref="DicomQuery{TInfo}" /> should be created for</typeparam>
    internal class DicomQueryProvider<TInfo> : IDicomQueryProvider<TInfo> where TInfo : IDicomInfo, new()
    {
        private readonly IGeneralizedInfoProvider mGeneralizedInfoProvider;
        private readonly IDicomDataLoader<TInfo> mDicomDataLoader;


        /// <summary>
        /// Initializes new instance of <see cref="DicomQueryProvider{TInfo}" />
        /// </summary>
        /// <param name="generalizedInfoProvider">Provides method for obtaining single instance of every implementation of <see cref="IDicomInfo"/></param>
        /// <param name="dicomDataLoader"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generalizedInfoProvider"/> is <see langword="null" /> -or-
        /// <paramref name="dicomDataLoader"/> is <see langword="null" />
        /// </exception>
        public DicomQueryProvider([NotNull] IGeneralizedInfoProvider generalizedInfoProvider, [NotNull] IDicomDataLoader<TInfo> dicomDataLoader)
        {
            if (generalizedInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(generalizedInfoProvider));
            }

            if (dicomDataLoader == null)
            {
                throw new ArgumentNullException(nameof(dicomDataLoader));
            }

            mGeneralizedInfoProvider = generalizedInfoProvider;
            mDicomDataLoader = dicomDataLoader;
        }


        /// <summary>
        /// Gets new instance of <see cref="DicomQuery{TInfo}"/>.
        /// </summary>
        /// <returns>New instance of <see cref="DicomQuery{TInfo}"/></returns>
        public DicomQuery<TInfo> GetDicomQuery()
        {
            return new DicomQuery<TInfo>(mGeneralizedInfoProvider, mDicomDataLoader, new WhereCollection<TInfo>(new DicomMapping(typeof(TInfo))));
        }
    }
}