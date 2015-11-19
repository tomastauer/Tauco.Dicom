using System;
using System.Collections.Generic;

using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Console.Tests
{
    [TestFixture]
    public class TypeHandlerSelectorTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(TypeHandlerSelector), null, new List<Func<object>>
            {
                mockProvider.GetPatientInfoProviderFake,
                mockProvider.GetStudyInfoProviderFake,
                mockProvider.GetSeriesInfoProviderFake
            });
        }


        [Test]
        public void SelectTypeHandler_NullType_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var typeHandlerSelector = new TypeHandlerSelector(mockProvider.GetPatientInfoProviderFake(), mockProvider.GetStudyInfoProviderFake(), mockProvider.GetSeriesInfoProviderFake());

            // Act + Assert
            Assert.That(() => typeHandlerSelector.SelectTypeHandler(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [TestCase("patient", typeof(PatientTypeHandler))]
        [TestCase("Patient", typeof(PatientTypeHandler))]
        [TestCase("study", typeof(StudyTypeHandler))]
        [TestCase("STUDY", typeof(StudyTypeHandler))]
        [TestCase("series", typeof(SeriesTypeHandler))]
        [TestCase("sErIeS", typeof(SeriesTypeHandler))]
        public void SelectTypeHandler_ReturnsAccordingToTheType(string type, Type expectedType)
        {
            // Arrange
            var mockProvider = new MockProvider();
            var typeHandlerSelector = new TypeHandlerSelector(mockProvider.GetPatientInfoProviderFake(), mockProvider.GetStudyInfoProviderFake(), mockProvider.GetSeriesInfoProviderFake());

            // Act
            var handlerSelector = typeHandlerSelector.SelectTypeHandler(type);

            // Assert
            Assert.That(handlerSelector, Is.InstanceOf(expectedType));
        }


        [Test]
        public void SelectTypeHandler_UnknownType_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var typeHandlerSelector = new TypeHandlerSelector(mockProvider.GetPatientInfoProviderFake(), mockProvider.GetStudyInfoProviderFake(), mockProvider.GetSeriesInfoProviderFake());

            // Act + Assert
            Assert.That(() => typeHandlerSelector.SelectTypeHandler("unknownType"), Throws.ArgumentException);
        }
    }
}
