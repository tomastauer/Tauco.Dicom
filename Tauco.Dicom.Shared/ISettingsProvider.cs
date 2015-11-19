namespace Tauco.Dicom.Shared
{
    /// <summary>
    /// Provides access to the setting values.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// AE of the server created the request.
        /// </summary>
        string CallingApplicationEntity
        {
            get;
        }


        /// <summary>
        /// AE of the server accepting the files from C-MOVE request.
        /// </summary>
        string DestinationApplicationEntity
        {
            get;
        }


        /// <summary>
        /// AE of the server the request is addressed for.
        /// </summary>
        string CalledApplicationEntity
        {
            get;
        }


        /// <summary>
        /// Address of the called server.
        /// </summary>
        string RemoteAddress
        {
            get;
        }


        /// <summary>
        /// Address of the calling server.
        /// </summary>
        string LocalAddress
        {
            get;
        }


        /// <summary>
        /// Port of the called server.
        /// </summary>
        int RemotePort
        {
            get;
        }


        /// <summary>
        /// Port of the calling server.
        /// </summary>
        int LocalPort
        {
            get;
        }
    }
}