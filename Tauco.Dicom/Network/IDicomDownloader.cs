using System;
using System.IO;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Network
{
    /// <summary>
    /// Provides method for downloading files from the DICOM server.
    /// </summary>
    internal interface IDicomDownloader<TInfo>
    {
        /// <summary>
        /// Performs asynchronous file download identified by given <paramref name="identifier"/> from the DICOM server.
        /// </summary>
        /// <param name="identifier">Specifies file to be downloaded</param>
        /// <param name="downloadAction">Specifies action which will be performed once server gets the file</param>
        /// <returns>Async result of the request</returns>
        Task DownloadAsync([NotNull] InfoIdentifier identifier, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction = null);
    }
}