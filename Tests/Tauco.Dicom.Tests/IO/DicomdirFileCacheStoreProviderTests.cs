using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.IO;
using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests
{
    [TestFixture]
    public class DicomdirFileCacheStoreProviderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomdirFileCacheStoreProvider), null, new List<Func<object>>
            {
                mockProvider.GetDicomdirFileParserFake,
                mockProvider.GetCacheProviderFake,
                mockProvider.GetCacheIndexProviderFake
            });
        }


        [Test]
        public async void StoreItemsInCache_StoresAllItemsInTheCache()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomdirParser = mockProvider.GetDicomdirFileParserFake();
            dicomdirParser.ParseDicomdirAsync(Arg.Any<string>()).Returns(Task.FromResult(GetDicomdirInfosFake()));
            var cacheProvider = mockProvider.GetCacheProviderFake();
            var cacheIndexProvider = mockProvider.GetCacheIndexProviderFake();
            cacheIndexProvider.GetCacheIndex(Arg.Any<object>()).Returns(c => c.Arg<object>().GetType().Name);

            var dicomdirFileCacheStoreProvider = new DicomdirFileCacheStoreProvider(dicomdirParser, cacheProvider, cacheIndexProvider);

            // Act
            var result = await dicomdirFileCacheStoreProvider.StoreItemsInCache("");
            var patient = result.Patients.Single();
            var study = result.Studies.Single();
            var series = result.Series.First();

            // Assert
            Assert.That(patient.PatientName.ToString(), Is.EqualTo("John Doe"));
            Assert.That(patient.PatientID.StringRepresentationWithoutSlash, Is.EqualTo("9107256444"));
            Assert.That(study.StudyInstanceUID.StringRepresentation, Is.EqualTo("1.2"));
            Assert.That(series.StudyInstanceUID.StringRepresentation, Is.EqualTo("1.2"));
            Assert.That(series.SeriesInstanceUID.StringRepresentation, Is.EqualTo("1.3"));
            Assert.That(() => cacheProvider.Received(1).Store("PatientInfo", Arg.Any<object>(), true), Throws.Nothing);
            Assert.That(() => cacheProvider.Received(1).Store("StudyInfo", Arg.Any<object>(), true), Throws.Nothing);
            Assert.That(() => cacheProvider.Received(2).Store("SeriesInfo", Arg.Any<object>(), true), Throws.Nothing);
        }


        private DicomdirInfos GetDicomdirInfosFake()
        {
            return new DicomdirInfos
            {
                Patients = new List<PatientInfo>
                {
                    new PatientInfo
                    {
                        PatientName = new PatientName("Doe^John"),
                        PatientID = new BirthNumber("9107256444")
                    }
                },
                Studies = new List<StudyInfo>
                {
                    new StudyInfo
                    {
                        StudyInstanceUID = new InfoIdentifier("1.2")
                    }
                },
                Series = new List<SeriesInfo>
                {
                    new SeriesInfo
                    {
                        StudyInstanceUID = new InfoIdentifier("1.2"),
                        SeriesInstanceUID = new InfoIdentifier("1.3")
                    },
                    new SeriesInfo
                    {
                        StudyInstanceUID = new InfoIdentifier("1.2"),
                        SeriesInstanceUID = new InfoIdentifier("1.4")
                    }
                }
            };
        }
    }
}
