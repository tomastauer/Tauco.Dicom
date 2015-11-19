using System;
using System.IO;
using System.Threading.Tasks;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Models
{
    /// <summary>
    /// Provides method for downloading of images related to some <see cref="IDicomInfo"/>
    /// </summary>
    /// <typeparam name="TInfo">Type of the info object</typeparam>
    public interface IDownloadProvider<in TInfo> where TInfo : IDicomInfo
    {
        /// <summary>
        /// Performs downloading of images related to the given info object.
        /// </summary>
        /// <param name="downloadAction">Specifies action which will be performed once server gets the file</param>
        /// <param name="info">Info object for which the images should be downloaded for</param>
        Task DownloadImagesAsync(TInfo info, Func<InfoIdentifier, InfoIdentifier, Stream> downloadAction = null);
    }
}