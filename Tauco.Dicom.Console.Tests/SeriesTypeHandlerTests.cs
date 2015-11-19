using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NSubstitute;

using Tauco.Dicom.Models;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Console.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class SeriesTypeHandlerTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(SeriesTypeHandler), null, new List<Func<object>>
            {
                mockProvider.GetSeriesInfoProviderFake,
                mockProvider.GetStudyInfoProviderFake,
            });
        }


        [Test]
        public void HandleTypeAsync_InvalidType_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesTypeHandler = new SeriesTypeHandler(mockProvider.GetSeriesInfoProviderFake(), mockProvider.GetStudyInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "UnknownType"
            };

            // Act
            var exception = seriesTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter()).Exception;

            // Assert
            Assert.That(exception.InnerExceptions.First(), Is.TypeOf<ArgumentException>());
        }


        [Test]
        public async void HandleTypeAsync_SingleSeries([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesInfoProvider = mockProvider.GetSeriesInfoProviderFake();
            var seriesTypeHandler = new SeriesTypeHandler(seriesInfoProvider, mockProvider.GetStudyInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "series",
                Identifier = "123",
                UseCache = useCache
            };

            // Act
            await seriesTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            Assert.That(() => seriesInfoProvider.Received(1).GetSeriesByIDAsync(new InfoIdentifier("123"), useCache), Throws.Nothing);
        }

        
        [Test]
        public async void HandleTypeAsync_AllSeries([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesInfoProvider = mockProvider.GetSeriesInfoProviderFake();
            var seriesQuery = Substitute.For<IDicomQuery<SeriesInfo>>();

            seriesInfoProvider.GetSeries().Returns(seriesQuery);
            var seriesTypeHandler = new SeriesTypeHandler(seriesInfoProvider, mockProvider.GetStudyInfoProviderFake());
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "series",
                UseCache = useCache
            };

            // Act
            await seriesTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            if (useCache)
            {
                Assert.That(() => seriesQuery.Received(1).LoadFromCache(), Throws.Nothing);
            }
            Assert.That(() => seriesInfoProvider.Received(1).GetSeries(), Throws.Nothing);
        }


        [Test]
        public async void HandleTypeAsync_ForStudy([Values(true, false)] bool useCache)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var seriesInfoProvider = mockProvider.GetSeriesInfoProviderFake();
            var seriesQuery = Substitute.For<IDicomQuery<SeriesInfo>>();

            seriesInfoProvider.GetSeriesForStudy(Arg.Any<StudyInfo>()).Returns(seriesQuery);
            var studyInfoProvider = mockProvider.GetStudyInfoProviderFake();
            var seriesTypeHandler = new SeriesTypeHandler(seriesInfoProvider, studyInfoProvider);
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), mockProvider.GetBirthNumberParserFake())
            {
                Type = "series",
                ParentIdentifier = "123",
                UseCache = useCache
            };

            // Act
            await seriesTypeHandler.HandleTypeAsync(inputArguments, GetMemoryTextWriter());

            // Assert
            if (useCache)
            {
                Assert.That(() => seriesQuery.Received(1).LoadFromCache(), Throws.Nothing);
            }
            Assert.That(() => studyInfoProvider.Received(1).GetStudyByIDAsync("123", useCache), Throws.Nothing);
            Assert.That(() => seriesInfoProvider.Received(1).GetSeriesForStudy(Arg.Any<StudyInfo>()), Throws.Nothing);
        }


        public TextWriter GetMemoryTextWriter()
        {
            return new StreamWriter(new MemoryStream());
        }

    }
}
