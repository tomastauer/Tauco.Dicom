using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Console.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class StudyTypeHandlerTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(StudyTypeHandler), null, new List<Func<object>>
            {
                mockProvider.GetStudyInfoProviderFake,
                mockProvider.GetPatientInfoProviderFake,
            });
        }


        [Test]
        public void HandleTypeAsync_InvalidType_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyTypeHandler = new StudyTypeHandler(mockProvider.GetStudyInfoProviderFake(), mockProvider.GetPatientInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "UnknownType"
            };

            // Act
            var exception = studyTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter()).Exception;

            // Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentException>());
        }


        [Test]
        public async void HandleTypeAsync_SingleStudy([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyInfoProvider = mockProvider.GetStudyInfoProviderFake();
            var studyTypeHandler = new StudyTypeHandler(studyInfoProvider, mockProvider.GetPatientInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "study",
                Identifier = "123",
                UseCache = useCache
            };

            // Act
            await studyTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            Assert.That(() => studyInfoProvider.Received(1).GetStudyByIDAsync(new InfoIdentifier("123"), useCache), Throws.Nothing);
        }
        

        [Test]
        public async void HandleTypeAsync_SingleStudy_Download()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyInfoProvider = mockProvider.GetStudyInfoProviderFake();
            var studyTypeHandler = new StudyTypeHandler(studyInfoProvider, mockProvider.GetPatientInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "study",
                Identifier = "123",
                Download = true
            };

            // Act
            await studyTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            Assert.That(() => studyInfoProvider.Received(1).GetStudyByIDAsync(new InfoIdentifier("123")), Throws.Nothing);
            Assert.That(() => studyInfoProvider.Received(1).DownloadImagesAsync(Arg.Any<StudyInfo>()), Throws.Nothing);
        }


        [Test]
        public async void HandleTypeAsync_AllStudies([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyInfoProvider = mockProvider.GetStudyInfoProviderFake();
            var query = Substitute.For<IDicomQuery<StudyInfo>>();

            studyInfoProvider.GetStudies().Returns(query);
            var studyTypeHandler = new StudyTypeHandler(studyInfoProvider, mockProvider.GetPatientInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "study",
                UseCache = useCache
            };

            // Act
            await studyTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            if (useCache)
            {
                Assert.That(() => query.Received(1).LoadFromCache(), Throws.Nothing);
            }
            Assert.That(() => studyInfoProvider.Received(1).GetStudies(), Throws.Nothing);
        }


        [Test]
        public async void HandleTypeAsync_ForPatient([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var studyInfoProvider = mockProvider.GetStudyInfoProviderFake();
            var studyQuery = Substitute.For<IDicomQuery<StudyInfo>>();

            studyInfoProvider.GetStudiesForPatient(Arg.Any<PatientInfo>()).Returns(studyQuery);
            var patientInfoProvider = mockProvider.GetPatientInfoProviderFake();
            var studyTypeHandler = new StudyTypeHandler(studyInfoProvider, patientInfoProvider);
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "study",
                ParentIdentifier = "123",
                UseCache = useCache
            };

            // Act
            await studyTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            if (useCache)
            {
                Assert.That(() => studyQuery.Received(1).LoadFromCache(), Throws.Nothing);
            }
            Assert.That(() => patientInfoProvider.Received(1).GetPatientByBirthNumberAsync("123", useCache), Throws.Nothing);
            Assert.That(() => studyInfoProvider.Received(1).GetStudiesForPatient(Arg.Any<PatientInfo>()), Throws.Nothing);
        }
        

        public TextWriter GetMemoryTextWriter()
        {
            return new StreamWriter(new MemoryStream());
        }

    }
}
