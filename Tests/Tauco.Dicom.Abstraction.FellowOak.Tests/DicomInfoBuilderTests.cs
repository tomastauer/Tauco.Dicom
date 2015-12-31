using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Dicom;

using NUnit.Framework;

using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomInfoBuilderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomInfoBuilder), null, new List<Func<object>>
            {
                mockProvider.GetMappingEngine,
                fellowOakMockProvider.GetDicomTagAdapterFake
            });
        }


        [Test]
        public void BuildInfo_NullSource_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            var dicomInfoBuilder = new DicomInfoBuilder(mockProvider.GetMappingEngine(), fellowOakMockProvider.GetDicomTagAdapterFake());

            // Act + Assert
            Assert.That(() => dicomInfoBuilder.BuildInfo<TestInfo>(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void BuildInfo_SourceNotConvertibleToDataset_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            var dicomInfoBuilder = new DicomInfoBuilder(mockProvider.GetMappingEngine(), fellowOakMockProvider.GetDicomTagAdapterFake());

            // Act + Assert
            Assert.That(() => dicomInfoBuilder.BuildInfo<TestInfo>(new object()), Throws.ArgumentException);
        }


        [Test]
        public void BuildInfo_DataSetPassed_BuildsCorrectTestInfo()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var fellowOakMockProvider = new FellowOakMockProvider();
            var dicomInfoBuilder = new DicomInfoBuilder(mockProvider.GetMappingEngine(), fellowOakMockProvider.GetDicomTagAdapterFake());
            var dataset = new DicomDataset
            {
                {
                    DicomTag.PatientID, "666"
                },
                {
                    DicomTag.PatientName, "testName"
                }
            };

            // Act
            var result = dicomInfoBuilder.BuildInfo<TestInfo>(dataset);

            // Assert
            Assert.That(result.PatientID, Is.EqualTo(666));
            Assert.That(result.PatientName, Is.EqualTo("testName"));
        }
    }
}
