﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Tests.Fakes;

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
            var mockProvider = new MockProvider();
            var dicomInfoBuilderFake = mockProvider.GetDicomInfoBuilderFake();
            dicomInfoBuilderFake.BuildInfo<StudyInfo>(Arg.Any<object>()).Returns(new StudyInfo { StudyInstanceUID = "1.2" });
            dicomInfoBuilderFake.BuildInfo<SeriesInfo>(Arg.Any<object>()).Returns(new SeriesInfo() { SeriesInstanceUID = "1.2" });

            var fellowOakDicomdirFileParser = new FellowOakDicomdirFileParser(dicomInfoBuilderFake);
        
            // Act
            await fellowOakDicomdirFileParser.ParseDicomdirAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/DICOMDIR"));

            // Assert
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<PatientInfo>(Arg.Any<object>()), Throws.Nothing);
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<StudyInfo>(Arg.Any<object>()), Throws.Nothing);
            Assert.That(() => dicomInfoBuilderFake.Received(1).BuildInfo<SeriesInfo>(Arg.Any<object>()), Throws.Nothing);
        }
    }
}