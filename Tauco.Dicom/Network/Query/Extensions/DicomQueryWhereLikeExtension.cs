using System;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Extension allowing filtering the query based on a satisfying the condition on containing.
    /// </summary>
    internal static class DicomQueryWhereLikeExtension
    {
        /// <summary>
        /// Filters a sequence of values based on a satisfying the condition on containing the given <paramref name="value"/>.
        /// </summary>
        /// <param name="query">Underlying filtered queryparam></param>
        /// <param name="dicomTag">Dicom tag to be compared</param>
        /// <param name="value">Value the Dicom Tag has to contain</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="dicomTag"/> cannot be set to <see cref="DicomTags.Undefined"/></exception>
        /// <returns>An <see cref="T:DicomQuery"/> that contains elements from the input sequence that satisfy the condition.</returns>
        public static IDicomQuery<TInfo> WhereLike<TInfo>(this IDicomQuery<TInfo> query, DicomTags dicomTag, [NotNull] object value) where TInfo : IDicomInfo, new()
        {
            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException($"Dicom tag cannot be set to {DicomTags.Undefined}", nameof(dicomTag));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            query.WhereCollection.WhereLike(dicomTag, value);
            return query;
        }
    }
}