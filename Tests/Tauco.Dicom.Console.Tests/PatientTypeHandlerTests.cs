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
    public class PatientTypeHandlerTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(PatientTypeHandler), null, new List<Func<object>>
            {
                mockProvider.GetPatientInfoProviderFake
            });
        }


        [Test]
        public void HandleTypeAsync_InvalidType_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientTypeHandler = new PatientTypeHandler(mockProvider.GetPatientInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "UnknownType"
            };

            // Act
            var exception = patientTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter()).Exception;

            // Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentException>());
        }


        [Test]
        public async void HandleTypeAsync_SinglePatient([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = mockProvider.GetPatientInfoProviderFake();
            var patientTypeHandler = new PatientTypeHandler(patientInfoProvider);
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "patient",
                Identifier = "123",
                UseCache = useCache
            };

            // Act
            await patientTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            Assert.That(() => patientInfoProvider.Received(1).GetPatientByBirthNumberAsync(new InfoIdentifier("123"), useCache), Throws.Nothing);
        }
        

        [Test]
        public async void HandleTypeAsync_SinglePatient_Download()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = mockProvider.GetPatientInfoProviderFake();
            var patientTypeHandler = new PatientTypeHandler(patientInfoProvider);
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "patient",
                Identifier = "123",
                Download = true
            };

            // Act
            await patientTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            Assert.That(() => patientInfoProvider.Received(1).GetPatientByBirthNumberAsync(new InfoIdentifier("123")), Throws.Nothing);
            Assert.That(() => patientInfoProvider.Received(1).DownloadImagesAsync(Arg.Any<PatientInfo>()), Throws.Nothing);
        }


        [Test]
        public async void HandleTypeAsync_AllPatients([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var patientInfoProvider = mockProvider.GetPatientInfoProviderFake();
            var query = Substitute.For<IDicomQuery<PatientInfo>>();

            patientInfoProvider.GetPatients().Returns(query);
            var patientTypeHandler = new PatientTypeHandler(patientInfoProvider);
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "patient",
                UseCache = useCache
            };

            // Act
            await patientTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            if (useCache)
            {
                Assert.That(() => query.Received(1).LoadFromCache(), Throws.Nothing);
            }
            Assert.That(() => patientInfoProvider.Received(1).GetPatients(), Throws.Nothing);
        }

        
        public TextWriter GetMemoryTextWriter()
        {
            return new StreamWriter(new MemoryStream());
        }

    }
}
