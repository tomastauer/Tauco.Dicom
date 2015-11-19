using System;
using System.Collections.Generic;

using NUnit.Framework;

using Tauco.Dicom.Network;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    public class DicomQueryProviderTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(DicomQueryProvider<>), new[] { typeof(TestInfo) }, new List<Func<object>>
            {
                mockProvider.GetGeneralizedInfoProviderFake,
                mockProvider.GetDicomDataLoaderFake
            });
        }


        [Test]
        public void GetDicomQuery_ReturnsDicomQuery()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQueryProvider = new DicomQueryProvider<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetDicomDataLoaderFake());

            // Act + Assert
            Assert.That(() => dicomQueryProvider.GetDicomQuery(), Throws.Nothing);
        }
    }
}
