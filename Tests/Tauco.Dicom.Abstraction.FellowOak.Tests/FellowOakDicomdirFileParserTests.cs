using System;
using System.Diagnostics.CodeAnalysis;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class FellowOakDicomdirFileParserTests
    {
        [Test]
        public void Constructor_NullDicomInfoBuilder_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new FellowOakDicomdirFileParser(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public async void ParseDicomdir_PatientsAreParsedCorrectly()
        {
            // Arrange
            var fellowOakMockProvider = new FellowOakMockProvider();
            var dicomInfoBuilderFake = fellowOakMockProvider.GetDicomInfoBuilderFake();
            dicomInfoBuilderFake.BuildInfo<StudyInfo>(Arg.Any<object>()).Returns(new StudyInfo { StudyInstanceUID = "1.2" });

            var fellowOakDicomdirFileParser = new FellowOakDicomdirFileParser(dicomInfoBuilderFake);
        
            // Act
            await fellowOakDicomdirFileParser.ParseDicomdirAsync("Assets/DICOMDIR");

            // Assert
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<PatientInfo>(Arg.Any<object>()), Throws.Nothing);
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<StudyInfo>(Arg.Any<object>()), Throws.Nothing);
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<SeriesInfo>(Arg.Any<object>()), Throws.Nothing);
        }
    }
}