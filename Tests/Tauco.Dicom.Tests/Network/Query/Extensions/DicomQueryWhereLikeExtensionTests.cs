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
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class DicomQueryWhereLikeExtensionTests
    {
        [Test]
        public void WhereLike_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act + Assert
            Assert.That(() => dicomQuery.WhereLike(DicomTags.Undefined, 0), Throws.ArgumentException);
        }


        [Test]
        public void WhereLike_NullValue_ThrowsException()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act + Assert
            Assert.That(() => dicomQuery.WhereLike(DicomTags.PatientName, null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void WhereLike_AddsConstraintToCollection()
        {
            // Arrange
            var mockProvider = new MockProvider();
            var dicomQuery = mockProvider.GetDicomQuery();

            // Act
            dicomQuery = dicomQuery.WhereLike(DicomTags.PatientID, 1);
            var whereItem = dicomQuery.WhereCollection.Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo(1));
            Assert.That(whereItem.Operator, Is.EqualTo(WhereOperator.Like));
        }



        [Test]
        [Ignore]
        public void WhereLike_WithoutCache_AddsConditionToRequest()
        {
            // Arrange
            var requestFactory = new DicomRequestFactoryFake();
            var mockProvider = new MockProvider();
            IDicomQuery<TestInfo> dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetDicomDataLoaderFake(), mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery = dicomQuery.WhereLike(DicomTags.PatientID, 1);
            dicomQuery.ToList();
            var whereItem = ((IWhereCollection<TestInfo>)requestFactory.WhereCollection).Single();

            // Assert
            Assert.That(whereItem.DicomTag, Is.EqualTo(DicomTags.PatientID));
            Assert.That(whereItem.Value, Is.EqualTo(1));
            Assert.That(whereItem.Operator, Is.EqualTo(WhereOperator.Like));
        }


        [Test]
        [Ignore]
        public void WhereLike_WithCache_FilterInMemoryList()
        {
            // Arrange
            var mockProvider = new MockProvider();
            IDicomQuery<TestInfo> dicomQuery = new DicomQuery<TestInfo>(mockProvider.GetGeneralizedInfoProviderFake(), mockProvider.GetDicomDataLoaderFake(), mockProvider.GetWhereCollectionFake());

            // Act
            dicomQuery = dicomQuery.WhereLike(DicomTags.PatientID, 1);
            var result = dicomQuery.ToList().Select(info => info.PatientID).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Contains(1), Is.True);
            Assert.That(result.Contains(10), Is.True);
        }
    }
}