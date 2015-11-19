using System.IO;
using System.Threading.Tasks;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Provides method for downloading data related to the type of implementation.
    /// </summary>
    internal interface ITypeHandler
    {
        /// <summary>
        /// Handles downloading of data related to the type.
        /// </summary>
        /// <param name="inputArguments">Parsed program input arguments</param>
        /// <param name="writer">Writer the serialized result will be written into</param>
        /// <returns>Represents an asynchronous operation</returns>
        Task HandleTypeAsync(InputArguments inputArguments, TextWriter writer);
    }
}