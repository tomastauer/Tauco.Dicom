using System;
using System.Configuration;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom
{
    /// <summary>
    /// Provides access to the values provided in the app.config file.
    /// </summary>
    internal class SettingsProvider : ISettingsProvider
    {
        /// <summary>
        /// AE of the server created the request.
        /// </summary>
        public string CallingApplicationEntity => GetStringValue("CallingApplicationEntity");


        /// <summary>
        /// AE of the C-MOVE request destination server.
        /// </summary>
        public string DestinationApplicationEntity => GetStringValue("DestinationApplicationEntity");


        /// <summary>
        /// AE of the server the request id addressed for.
        /// </summary>
        public string CalledApplicationEntity => GetStringValue("CalledApplicationEntity");


        /// <summary>
        /// Address of the called server.
        /// </summary>
        public string RemoteAddress => GetStringValue("RemoteAddress");


        /// <summary>
        /// Address of the calling server.
        /// </summary>
        public string LocalAddress => GetStringValue("LocalAddress");


        /// <summary>
        /// Port of the called server.
        /// </summary>
        public int RemotePort => GetIntegerValue("RemotePort");


        /// <summary>
        /// Port of the calling server.
        /// </summary>
        public int LocalPort => GetIntegerValue("LocalPort");


        /// <summary>
        /// Gets string setting value from the app.config file according to the given key.
        /// </summary>
        /// <param name="key">Key of the setting</param>
        /// <exception cref="ArgumentException">There is no setting with the <paramref name="key"/> available.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null</exception>
        /// <returns>Value from the app.config</returns>
        private string GetStringValue([NotNull] string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = ConfigurationManager.AppSettings[key];
            
            return value;
        }


        /// <summary>
        /// Gets integer setting value from the app.config file according to the given key.
        /// </summary>
        /// <param name="key">Key of the setting</param>
        /// <exception cref="ArgumentException">There is no setting with the <paramref name="key"/> available -or- setting value cannot be converted to the integer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null</exception>
        /// <returns>Value from the app.config</returns>
        private int GetIntegerValue([NotNull] string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var stringValue = GetStringValue(key);

            int result;
            if (int.TryParse(stringValue, out result))
            {
            }

            return result;
        }
    }
}
