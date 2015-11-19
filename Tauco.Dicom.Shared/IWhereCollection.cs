using System;
using System.Collections.Generic;

namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Collection containing constraints used for filtering requested items.
    /// </summary>
    /// <typeparam name="TInfo">The type of elements contained in the collection.</typeparam>
    public interface IWhereCollection<in TInfo> : IList<WhereItem> where TInfo : IDicomInfo
    {
        /// <summary>
        /// Gets predicate matching all condition within the collection usable for LINQ expressions.
        /// </summary>
        Func<TInfo, bool> Predicate
        {
            get;
        }


        /// <summary>
        /// Indexer returning collection of <see cref="WhereItem" /> according to the given <paramref name="dicomTag" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which <see cref="WhereItem" /> should be selected</param>
        /// <returns>Instance of <see cref="WhereItem" /> corresponding to the given <paramref name="dicomTag" /></returns>
        IEnumerable<WhereItem> this[DicomTags dicomTag]
        {
            get;
        }

        /// <summary>
        /// Adds new like condition to the collection. Will match all objects with given <paramref name="dicomTag" /> which equals
        /// to given <paramref name="value" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which property will be used for filering</param>
        /// <param name="value">Value needed to equal to the property value</param>
        void WhereEquals(DicomTags dicomTag, object value);


        /// <summary>
        /// Adds new like condition to the collection. Will match all objects with given <paramref name="dicomTag" /> containing
        /// given <paramref name="value" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag specifying which property will be used for filering</param>
        /// <param name="value">Value needed to be contained within the property value</param>
        void WhereLike(DicomTags dicomTag, object value);


        /// <summary>
        /// Determines whether the collection contains the given <paramref name="dicomTag" />.
        /// </summary>
        /// <param name="dicomTag">Dicom tag the request is made for</param>
        /// <exception cref="ArgumentException"><paramref name="dicomTag" /> equals <see cref="DicomTags.Undefined" /></exception>
        /// <returns>True, if collection contains <paramref name="dicomTag" />; otherwise, false</returns>
        bool ContainsTag(DicomTags dicomTag);


        /// <summary>
        /// Gets all where collections suitable for DICOM C-FIND request created from current <see cref="IWhereCollection{TInfo}"/>.
        /// New combination has to be created for every OR statement within the collection (when one <see cref="DicomTags"/> has specified more than one value).
        /// </summary>
        /// <returns>Collection containing all combinations obtained from current <see cref="IWhereCollection{TInfo}"/></returns>
        IEnumerable<IDicomWhereCollection> GetDicomWhereCollections();
    }
}