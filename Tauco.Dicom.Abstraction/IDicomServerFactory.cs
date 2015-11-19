using System;
using System.IO;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Shared.Model;

namespace Tauco.Dicom.Abstraction
{
    /// <summary>
    /// Provides method for obtaining new isntance of <see cref="IDicomServer"/> implementation.
    /// </summary>
    public interface IDicomServerFactory
    {
        /// <summary>
        /// Creates new instance of <see cref="IDicomServer"/>.
        /// </summary>
        /// <param name="downloadAction">Specifies action which will be performed once server gets C-MOVE request</param>
        /// <returns>New instance of <see cref="IDicomServer"/></returns>
        IDicomServer CreateDicomServer([NotNull] Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction);
    }
}