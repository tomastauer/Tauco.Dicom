using System;
using System.Linq;

using Castle.Windsor;

using NUnit.Framework;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests.Integration
{
    [TestFixture]
    public class FellowOakDicomdirFileParserTests
    {
        private IDicomdirFileParser mDicomdirFileParser;

        [SetUp]
        public void SetUp()
        {
            var container = new WindsorContainer().Install(new CommonInstaller());
            mDicomdirFileParser = container.Resolve<IDicomdirFileParser>();
        }


        [Test]
        public async void ParseDicomdir_PatientsAreParsedCorrectly()
        {
            // Arrange + Act
            var result = await mDicomdirFileParser.ParseDicomdirAsync("Assets/DICOMDIR");
            var patient = result.Patients.Single();
            var study = result.Studies.Single();
            var series = result.Series.Single();

            // Assert
            Assert.That(patient.PatientName.ToString(), Is.EqualTo("John Doe"));
            Assert.That(patient.PatientID.StringRepresentationWithoutSlash, Is.EqualTo("9107256444"));
            Assert.That(study.StudyInstanceUID.StringRepresentation, Is.EqualTo("1.2"));
            Assert.That(series.StudyInstanceUID.StringRepresentation, Is.EqualTo("1.2"));
            Assert.That(series.SeriesInstanceUID.StringRepresentation, Is.EqualTo("1.3"));
        }
    }
}
