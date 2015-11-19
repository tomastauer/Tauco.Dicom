using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests.Fakes;


namespace Tauco.Dicom.Tests.Network
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class DicomQueryWhereEqualsExtensionTests
    {
        [Test]
        public void WhereEquals_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act + Assert
            Assert.That(() => dicomQuery.WhereEquals(DicomTags.Undefined, 0), Throws.ArgumentException);
        }


        [Test]
        public void WhereEquals_NullValue_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act + Assert
            Assert.That(() => dicomQuery.WhereEquals(DicomTags.PatientName, null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void WhereEquals_AddsConstraintToCollection()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act
            dicomQuery = dicomQuery.WhereEquals(DicomTags.PatientID, 1);
            var whereItem = dicomQuery.WhereCollection.Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo(1));
            Assert.That(whereItem.Operator, Is.EqualTo(WhereOperator.Equals));
        }



        [Test]
        [Ignore]
        public void WhereEquals_WithoutCache_AddsConditionToRequest()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var requestFactory = new DicomRequestFactoryFake();
            IDicomQuery<TestInfo> dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetDicomDataLoaderFake(), mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery = dicomQuery.WhereEquals(DicomTags.PatientID, 1);
            dicomQuery.ToList();
            var whereItem = ((IWhereCollection<TestInfo>)requestFactory.WhereCollection).Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo(1));
            Assert.That(whereItem.Operator, Is.EqualTo(WhereOperator.Equals));
        }


        [Test]
        [Ignore]
        public void WhereEquals_WithCache_FilterInMemoryList()
        {
            // Arrange
            var mockProvider = new MockProvider();
            IDicomQuery<TestInfo> dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetDicomDataLoaderFake(), mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery = dicomQuery.WhereEquals(DicomTags.PatientID, 1);
            var result = dicomQuery.ToList().Single();

            // Assert
            Assert.That(result.PatientID, Is.EqualTo(1));
        }
    }
}