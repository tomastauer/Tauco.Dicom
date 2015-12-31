using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Dicom;
using Dicom.Media;

using JetBrains.Annotations;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak
{
    /// <summary>
    /// Provides method for parsing given dicomdir file to collections for every corresponding info object.
    /// </summary>
    internal class FellowOakDicomdirFileParser : IDicomdirFileParser
    {
        private readonly IDicomInfoBuilder mDicomInfoBuilder;


        /// <summary>
        /// Instantiates new instance of <see cref="FellowOakDicomdirFileParser"/>.
        /// </summary>
        /// <param name="dicomInfoBuilder">Provides method for creating strongly typed instance of <see cref="IDicomInfo"/> from given dataset</param>
        /// <exception cref="ArgumentNullException"><paramref name="dicomInfoBuilder"/> is null</exception>
        public FellowOakDicomdirFileParser([NotNull] IDicomInfoBuilder dicomInfoBuilder)
        {
            if (dicomInfoBuilder == null)
            {
                throw new ArgumentNullException(nameof(dicomInfoBuilder));
            }

            mDicomInfoBuilder = dicomInfoBuilder;
        }


        /// <summary>
        /// Parses dicomdir file located at given <paramref name="dicomdirPath"/> and returns <see cref="DicomdirInfos"/> composed of collections
        /// containing all <see cref="PatientInfo"/>, <see cref="StudyInfo"/> and <see cref="SeriesInfo"/> from the dicomdir file.
        /// </summary>
        /// <param name="dicomdirPath">Path leading to the dicomdir file</param>
        /// <exception cref="System.IO.FileNotFoundException">Could not found file specified in <paramref name="dicomdirPath"/></exception>
        /// <returns>Instance of <see cref="DicomdirInfos"/> composed of collections containing all parsed infos.</returns>
        public async Task<DicomdirInfos> ParseDicomdirAsync(string dicomdirPath)
        {
            var dicomDirectory = await Task.Factory.FromAsync(DicomDirectory.BeginOpen(dicomdirPath, Encoding.UTF8, null, null), DicomDirectory.EndOpen);
            var patientsList = new List<PatientInfo>();
            var studiesList = new List<StudyInfo>();
            var seriesList = new List<SeriesInfo>();

            foreach (var patient in dicomDirectory.RootDirectoryRecordCollection)
            {
                patientsList.Add(mDicomInfoBuilder.BuildInfo<PatientInfo>(patient));

                foreach (var study in patient.LowerLevelDirectoryRecordCollection)
                {
                    var studyInfo = mDicomInfoBuilder.BuildInfo<StudyInfo>(study);
                    studiesList.Add(studyInfo);

                    foreach (var series in study.LowerLevelDirectoryRecordCollection)
                    {
                        series.Add(DicomTag.StudyInstanceUID, new DicomUID(studyInfo.StudyInstanceUID, null, DicomUidType.Unknown));
                        seriesList.Add(mDicomInfoBuilder.BuildInfo<SeriesInfo>(series));
                    }
                }
            }

            return new DicomdirInfos
            {
                Patients = patientsList,
                Series = seriesList,
                Studies = studiesList
            };
        }
    }
}
