using System;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Extension allowing filtering the query based on a satisfying the condition on equality.
    /// </summary>
    internal static class DicomQueryWhereEqualsExtension
    {
        /// <summary>
        /// Filters a sequence of values based on a satisfying the condition on equality of given <paramref name="value"/>.
        /// </summary>
        /// <param name="query">Underlying filtered queryparam></param>
        /// <param name="dicomTag">Dicom tag to be compared</param>
        /// <param name="value">Value the Dicom Tag has to be equal to</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="dicomTag"/> cannot be set to <see cref="DicomTags.Undefined"/></exception>
        /// <returns>An <see cref="T:DicomQuery"/> that contains elements from the input sequence that satisfy the condition.</returns>
        public static IDicomQuery<TInfo> WhereEquals<TInfo>(this IDicomQuery<TInfo> query, DicomTags dicomTag, [NotNull] object value) where TInfo : IDicomInfo, new()
        {
            if (dicomTag == DicomTags.Undefined)
            {
                throw new ArgumentException($"Dicom tag cannot be set to {DicomTags.Undefined}", nameof(dicomTag));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            query.WhereCollection.WhereEquals(dicomTag, value);
            return query;
        }

    }
}
