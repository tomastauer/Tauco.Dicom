using System;
using System.Collections.Generic;
using System.Linq;

using CommandLine;
using CommandLine.Text;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Console
{
    /// <summary>
    /// Represents input arguments passed when starting the program.
    /// </summary>
    internal class InputArguments
    {
        private readonly ISettingsProvider mSettingsProvider;
        private readonly IBirthNumberParser mBirthNumberParser;
        private string mCallingApplicationEntity;
        private string mDestinationApplicationEntity;
        private string mCalledApplicationEntity;
        private string mRemoteAddress;
        private int? mRemotePort;
        private string mLocalAddress;
        private int? mLocalPort;
        private readonly string[] mAcceptedTypes =
        {
            "patient", "study", "series"
        };


        /// <summary>
        /// Instantiates new instance of <see cref="InputArguments"/>.
        /// </summary>
        /// <param name="settingsProvider">Provides access to the setting values</param>
        /// <param name="birthNumberParser">Service for parsing czech birth numbers</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingsProvider"/> is <see langword="null"/> -or- <paramref name="birthNumberParser"/> is <see langword="null"/></exception>
        public InputArguments(ISettingsProvider settingsProvider, IBirthNumberParser birthNumberParser)
        {
            if (settingsProvider == null)
            {
                throw new ArgumentNullException(nameof(settingsProvider));
            }
            if (birthNumberParser == null)
            {
                throw new ArgumentNullException(nameof(birthNumberParser));
            }

            mSettingsProvider = settingsProvider;
            mBirthNumberParser = birthNumberParser;
        }


        /// <summary>
        /// AE of the server created the request.
        /// </summary>
        [Option("callingAE", HelpText = "Application entity of the server created the request.")]
        public string CallingApplicationEntity
        {
            get
            {
                return mCallingApplicationEntity ?? mSettingsProvider.CallingApplicationEntity;
            }
            set
            {
                mCallingApplicationEntity = value;
            }
        }


        /// <summary>
        /// AE of the server accepting the files from C-MOVE request.
        /// </summary>
        [Option("destinationAE", HelpText = "Application entity of the C-MOVE request destination server.")]
        public string DestinationApplicationEntity
        {
            get
            {
                return mDestinationApplicationEntity ?? mSettingsProvider.DestinationApplicationEntity;
            }
            set
            {
                mDestinationApplicationEntity = value;
            }
        }


        /// <summary>
        /// AE of the server the request is addressed for.
        /// </summary>
        [Option("calledAE", HelpText = "Application entity of the server the request id addressed for.")]
        public string CalledApplicationEntity
        {
            get
            {
                return mCalledApplicationEntity ?? mSettingsProvider.CalledApplicationEntity;
            }
            set
            {
                mCalledApplicationEntity = value;
            }
        }


        /// <summary>
        /// Address of the called server.
        /// </summary>
        [Option("remoteAddr", HelpText = "Address of the called server.")]
        public string RemoteAddress
        {
            get
            {
                return mRemoteAddress ?? mSettingsProvider.RemoteAddress;
            }
            set
            {
                mRemoteAddress = value;
            }
        }


        /// <summary>
        /// Port of the called server.
        /// </summary>
        [Option("remotePort", HelpText = "Port of the called server.")]
        public int? RemotePort
        {
            get
            {
                return mRemotePort ?? mSettingsProvider.RemotePort;
            }
            set
            {
                mRemotePort = value;
            }
        }


        /// <summary>
        /// Address of the calling server.
        /// </summary>
        [Option("localAddr", HelpText = "Address of the calling server.")]
        public string LocalAddress
        {
            get
            {
                return mLocalAddress ?? mSettingsProvider.LocalAddress;
            }
            set
            {
                mLocalAddress = value;
            }
        }


        /// <summary>
        /// Port of the calling server.
        /// </summary>
        [Option("localPort", HelpText = "Port of the calling server.")]
        public int? LocalPort
        {
            get
            {
                return mLocalPort ?? mSettingsProvider.LocalPort;
            }
            set
            {
                mLocalPort = value;
            }
        }


        /// <summary>
        /// Type of the query. Has to be one of the following values: 'patient', 'study', 'series'.
        /// </summary>
        [Option('t', "type", HelpText = "Type of the query. Has to be one of the following values: 'patient', 'study', 'series'.", Required = true)]
        public string Type
        {
            get;
            set;
        }


        /// <summary>
        /// Identifier of the single object. Has to be either birth number or UID.
        /// </summary>
        [Option('i', "identifier", HelpText = "Identifier of the single object. Has to be either birth number or UID.")]
        public string Identifier
        {
            get;
            set;
        }


        /// <summary>
        /// Determines whether the result should be loaded from cache or from the server.
        /// </summary>
        [Option('c', "useCache", HelpText = "Determines whether the result should be loaded from cache or from the server.")]
        public bool UseCache
        {
            get;
            set;
        }


        /// <summary>
        /// Determines whether the downloading of DICOM images should be initiated for the object.
        /// </summary>
        [Option('d', "download", HelpText = "Determines whether the downloading of DICOM images should be initiated for the object.")]
        public bool Download
        {
            get;
            set;
        }


        /// <summary>
        /// Identifies parent of the object, e.g. patient for studies or study for series.
        /// </summary>
        [Option('p', "parent", HelpText = "Identifies parent of the object, e.g. patient for studies or study for series.")]
        public string ParentIdentifier
        {
            get;
            set;
        }


        /// <summary>
        /// Validates all possible combinations of input parameters.
        /// </summary>
        /// <returns>Collection containint validation errors</returns>
        public IEnumerable<string> Validate()
        {
            if (string.IsNullOrEmpty(CallingApplicationEntity))
            {
                yield return "Calling application entity has to be set either in app.config or with the input argument";
            }
            if (string.IsNullOrEmpty(CalledApplicationEntity))
            {
                yield return "Called application entity has to be set either in app.config or with the input argument";
            }
            if (string.IsNullOrEmpty(RemoteAddress))
            {
                yield return "Remote address has to be set either in app.config or with the input argument";
            }
            if (!RemotePort.HasValue || RemotePort == 0)
            {
                yield return "Remote port has to be set either in app.config or with the input argument";
            }

            if (Download)
            {
                if (string.IsNullOrEmpty(DestinationApplicationEntity))
                {
                    yield return "For downloading images, destination application entity has to be set either in app.config or with the input argument";
                }

                if (string.IsNullOrEmpty(LocalAddress))
                {
                    yield return "For downloading images, local address has to be set either in app.config or with the input argument";
                }

                if (!LocalPort.HasValue || LocalPort == 0)
                {
                    yield return "For downloading images, local port has to be set either in app.config or with the input argument";
                }

                if (string.IsNullOrEmpty(Identifier))
                {
                    yield return "For downloading images, identifier has to be specified";
                }

                if (UseCache)
                {
                    yield return "Images cannot be downloaded from the cache";
                }
            }

            if (!mAcceptedTypes.Contains(Type, StringComparer.OrdinalIgnoreCase))
            {
                yield return "Type has to be 'patient', 'study' or 'series'";
                yield break;
            }

            if (Type.Equals("patient", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(Identifier))
                {
                    var birthNumber = mBirthNumberParser.GetBirthNumber(Identifier);
                    if (birthNumber == null)
                    {
                        yield return "Provided identifier is not a valid birth number";
                    }
                }
                if (!string.IsNullOrEmpty(ParentIdentifier))
                {
                    yield return "Patients don't have parent.";
                }
            }
            else if (Type.Equals("study", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(ParentIdentifier))
                {
                    var birthNumber = mBirthNumberParser.GetBirthNumber(ParentIdentifier);
                    if (birthNumber == null)
                    {
                        yield return "Provided parent identifier is not a valid birth number";
                    }
                }
            }
            else if (Type.Equals("series", StringComparison.OrdinalIgnoreCase))
            {
                if (Download)
                {
                    yield return "Images downloading is permitted for patients and studies only";
                }
            }
        }


        /// <summary>
        /// Represents the parser state.
        /// </summary>
        [ParserState]
        public IParserState LastParserState { get; set; }


        /// <summary>
        /// Generates help message.
        /// </summary>
        /// <returns></returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
