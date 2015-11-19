using System;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Loads server settings from given <see cref="InputArguments"/> instead of using config file.
    /// </summary>
    internal class InputSettingsProvider : ISettingsProvider
    {
        private readonly InputArguments mInputArguments;


        /// <summary>
        /// Instantiates new instance of <see cref="InputArguments"/>.
        /// </summary>
        /// <param name="inputArguments">Represents input artuments passed when starting the program</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputArguments"/> is <see langword="null"/></exception>
        public InputSettingsProvider(InputArguments inputArguments)
        {
            if (inputArguments == null)
            {
                throw new ArgumentNullException(nameof(inputArguments));
            }

            mInputArguments = inputArguments;
        }


        /// <summary>
        /// AE of the server created the request.
        /// </summary>
        public string CallingApplicationEntity => mInputArguments.CallingApplicationEntity;


        /// <summary>
        /// AE of the server accepting the files from C-MOVE request.
        /// </summary>
        public string DestinationApplicationEntity => mInputArguments.DestinationApplicationEntity;


        /// <summary>
        /// AE of the server the request is addressed for.
        /// </summary>
        public string CalledApplicationEntity => mInputArguments.CalledApplicationEntity;


        /// <summary>
        /// Address of the called server.
        /// </summary>
        public string RemoteAddress => mInputArguments.RemoteAddress;


        /// <summary>
        /// Address of the calling server.
        /// </summary>
        public string LocalAddress => mInputArguments.LocalAddress;


        /// <summary>
        /// Port of the called server.
        /// </summary>
        public int RemotePort => mInputArguments.RemotePort ?? 0;


        /// <summary>
        /// Port of the calling server.
        /// </summary>
        public int LocalPort => mInputArguments.LocalPort ?? 0;
    }
}
