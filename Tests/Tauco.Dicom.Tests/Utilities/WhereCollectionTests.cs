using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

using Tauco.Dicom.Network;
using Tauco.Dicom.Shared;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Utilities
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    internal class WhereCollectionTests
    {
        private static List<TestInfo> GetInfoList()
        {
            var list = new List<TestInfo>
            {
                new TestInfo
                {
                    PatientID = 1,
                    PatientName = "testName"
                },
                new TestInfo
                {
                    PatientID = 2,
                    PatientName = "testName2"
                },
                new TestInfo
                {
                    PatientID = 3,
                    PatientName = "testName3"
                }
            };
            return list;
        }


        [Test]
        public void Constructor_NullArgument_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new WhereCollection<TestInfo>(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Indexer_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection[DicomTags.Undefined], Throws.ArgumentException);
        }


        [Test]
        public void Indexer_ExistingDicomTag_Returns()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientName, "test");
            whereCollection.WhereEquals(DicomTags.PatientName, "test2");

            // Act
            var result = whereCollection[DicomTags.PatientName];

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public void Indexer_NotExistingDicomTag_ReturnsEmpty()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            
            // Act
            var result = whereCollection[DicomTags.PatientName];

            // Assert
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void WhereEquals_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection.WhereEquals(DicomTags.Undefined, ""), Throws.ArgumentException);
        }


        [Test]
        public void WhereEquals_NullValue_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection.WhereEquals(DicomTags.PatientID, null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void WhereEquals_AddsCorrectWhereItem()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act
            whereCollection.WhereEquals(DicomTags.PatientName, "test");
            var result = whereCollection.Single();

            // Assert
            Assert.That(result.Value, Is.EqualTo("test"));
            Assert.That(result.DicomTag, Is.EqualTo(DicomTags.PatientName));
            Assert.That(result.Operator, Is.EqualTo(WhereOperator.Equals));
        }


        [Test]
        public void WhereLike_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection.WhereLike(DicomTags.Undefined, ""), Throws.ArgumentException);
        }


        [Test]
        public void WhereLike_NullValue_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection.WhereLike(DicomTags.PatientID, null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void WhereLike_AddsCorrectWhereItem()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act
            whereCollection.WhereLike(DicomTags.PatientName, "test");
            var result = whereCollection.Single();

            // Assert
            Assert.That(result.Value, Is.EqualTo("test"));
            Assert.That(result.DicomTag, Is.EqualTo(DicomTags.PatientName));
            Assert.That(result.Operator, Is.EqualTo(WhereOperator.Like));
        }


        [Test]
        public void ContainsTag_UndefinedDicomTag_ThrowsException()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act + Assert
            Assert.That(() => whereCollection.ContainsTag(DicomTags.Undefined), Throws.ArgumentException);
        }


        [Test]
        public void ContainsTag_DoesNotContain_ReturnFalse()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            
            // Act
            var result = whereCollection.ContainsTag(DicomTags.PatientName);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void ContainsTag_DoesContain_ReturnTrue()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientName, "test");

            // Act
            var result = whereCollection.ContainsTag(DicomTags.PatientName);

            // Assert
            Assert.That(result, Is.True);
        }



        [Test]
        public void GetDicomWhereCollections_NoConstraints_ReturnsEmptyCollection()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act
            var result = whereCollection.GetDicomWhereCollections();

            // Assert
            Assert.That(result.First(), Is.Empty);
        }


        [Test]
        public void GetDicomWhereCollections_ExistingConstraints_ReturnsAllCombinations()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereEquals(DicomTags.PatientName, "test1");
            whereCollection.WhereEquals(DicomTags.PatientName, "test2");
            whereCollection.WhereLike(DicomTags.PatientName, "test3");
            whereCollection.WhereEquals(DicomTags.PatientID, 1);
            whereCollection.WhereEquals(DicomTags.PatientID, 2);
            whereCollection.WhereLike(DicomTags.PatientID, 3);
            whereCollection.WhereEquals(DicomTags.Modality, "test1");
            whereCollection.WhereEquals(DicomTags.Modality, "test2");
            whereCollection.WhereLike(DicomTags.Modality, "test3");
            
            // Act
            var result = whereCollection.GetDicomWhereCollections();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(27));
        }


        [Test]
        public void GetDicomWhereCollections_ExistingConstraints_ReturnsCorrectConditions()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereLike(DicomTags.PatientName, "test");
            whereCollection.WhereEquals(DicomTags.PatientID, 1);
            
            // Act
            var result = whereCollection.GetDicomWhereCollections().Single();

            // Assert
            Assert.That(result.Any(c=>c.DicomTag == DicomTags.PatientID && c.Value == "1"), Is.True);
            Assert.That(result.Any(c=>c.DicomTag == DicomTags.PatientName && c.Value == "*test*"), Is.True);
        }

        
        [Test]
        public void Predicate_AdvancedEqualsMatch_False()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereEquals(DicomTags.PatientID, 2);
            whereCollection.WhereEquals(DicomTags.PatientName, "testName");

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(0));
        }


        [Test]
        public void Predicate_AdvancedEqualsMatch_True()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereEquals(DicomTags.PatientID, 2);
            whereCollection.WhereEquals(DicomTags.PatientName, "testName2");

            // Act
            TestInfo match = list.Where(whereCollection.Predicate).Single();

            // Assert
            Assert.That(match.PatientID, Is.EqualTo(2));
            Assert.That(match.PatientName, Is.EqualTo("testName2"));
        }


        [Test]
        public void Predicate_AdvancedLikeMatch_False()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereLike(DicomTags.PatientID, 2);
            whereCollection.WhereLike(DicomTags.PatientName, "testtt");

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(0));
        }


        [Test]
        public void Predicate_AdvancedLikeMatch_True()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereLike(DicomTags.PatientID, 2);
            whereCollection.WhereLike(DicomTags.PatientName, "test");

            // Act
            TestInfo match = list.Where(whereCollection.Predicate).Single();

            // Assert
            Assert.That(match.PatientID, Is.EqualTo(2));
            Assert.That(match.PatientName, Is.EqualTo("testName2"));
        }


        [Test]
        public void Predicate_AdvancedMatch_True()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereEquals(DicomTags.PatientID, 1);
            whereCollection.WhereEquals(DicomTags.PatientID, 2);
            whereCollection.WhereLike(DicomTags.PatientName, "test");
            whereCollection.WhereEquals(DicomTags.PatientName, "testName2");

            // Act
            var match = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(match.Count, Is.EqualTo(2));
            Assert.That(match.Any(c=>c.PatientName == "testName" && c.PatientID == 1), Is.True);
            Assert.That(match.Any(c=>c.PatientName == "testName2" && c.PatientID == 2), Is.True);
        }


        [Test]
        public void Predicate_EmptyMatch()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(3));
        }


        [Test]
        public void Predicate_MultipleEqualsMatch()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            list[1].PatientID = 1;

            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(2));
            Assert.That(matches.All(c => c.PatientID == 1), Is.True);
        }


        [Test]
        public void Predicate_SameDicomTag_ReturnsBoth()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereEquals(DicomTags.PatientID, 1);
            whereCollection.WhereEquals(DicomTags.PatientID, 2);

            // Act
            var match = list.Where(whereCollection.Predicate);

            // Assert
            Assert.That(match.Count(), Is.EqualTo(2));
        }


        [Test]
        public void Predicate_SimpleEqualsMatch()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();

            var whereCollection = mockProvider.GetWhereCollection();
            whereCollection.WhereEquals(DicomTags.PatientID, 1);

            // Act
            TestInfo match = list.Where(whereCollection.Predicate).Single();

            Assert.That(match.PatientID, Is.EqualTo(1));
        }


        [Test]
        public void Predicate_SimpleLikeMatch_ExistingContainsMethod()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereLike(DicomTags.PatientName, "testName");

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(3));
        }


        [Test]
        public void Predicate_SimpleLikeMatch_WithoutContainsMethod()
        {
            // Arrange
            MockProvider mockProvider = new MockProvider();
            var list = GetInfoList();
            var whereCollection = mockProvider.GetWhereCollection();

            whereCollection.WhereLike(DicomTags.PatientID, 1);

            // Act
            var matches = list.Where(whereCollection.Predicate).ToList();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(1));
        }
    }
}