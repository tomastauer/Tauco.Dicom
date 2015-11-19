using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public class InfoIdentifierTests
    {
        [Test]
        public void Constructor_NullIdentifier_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new InfoIdentifier(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Constructor_SetsStringRepresentation()
        {
            // Arrange
            string id = "testId";
            var infoIdentifier = new InfoIdentifier(id);

            // Act + Assert
            Assert.That(infoIdentifier.StringRepresentation, Is.EqualTo(id));
        }


        [Test]
        public void ImplicitOperator_FromString()
        {
            // Arrange
            InfoIdentifier id = "testId";
            var infoIdentifier = new InfoIdentifier("testId");
            
            // Act + Assert
            Assert.That(id, Is.EqualTo(infoIdentifier));
        }


        [Test]
        public void ImplicitOperator_ToString()
        {
            // Arrange
            string id = "testId";
            string infoIdentifier = new InfoIdentifier(id);

            // Act + Assert
            Assert.That(id, Is.EqualTo(infoIdentifier));
        }


        [TestCase("testId", "testId", true)]
        [TestCase("testId", "anotherId", false)]
        public void Equals_MatchEqualObject(string firstId, string secondId, bool expectedResult)
        {
            // Arrange
            var firstInfoIdentifier = new InfoIdentifier(firstId);
            var secondInfoIdentifier = new InfoIdentifier(secondId);

            // Act
            bool result = firstInfoIdentifier.Equals(secondInfoIdentifier);
            
            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [Test]
        public void Equals_DoesNotMatchNull()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("test");

            // Act
            var result = infoIdentifier.Equals(null);

            // Arrange
            Assert.That(result, Is.False);
        }

        
        [Test]
        public void Equals_DoesMatchString()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("test");

            // Act
            var result = infoIdentifier.Equals("test");

            // Arrange
            Assert.That(result, Is.True);
        }



        [Test]
        public void Equals_DoesMatchItself()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("test");

            // Act
            var result = infoIdentifier.Equals(infoIdentifier);

            // Arrange
            Assert.That(result, Is.True);
        }



        [Test]
        public void Equals_DoesNotMatchDifferentType()
        {
            // Arrange
            var infoIdentifier = new InfoIdentifier("test");

            // Act
            var result = infoIdentifier.Equals(1);

            // Arrange
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_SameCodeForEqualObjects()
        {
            // Arrange
            string id = "testId";
            var firstInfoIdentifier = new InfoIdentifier(id);
            var secondInfoIdentifier = new InfoIdentifier(id);

            // Act
            int firstHashCode = firstInfoIdentifier.GetHashCode();
            int secondHashCode = secondInfoIdentifier.GetHashCode();

            // Assert
            Assert.That(firstHashCode, Is.EqualTo(secondHashCode));
        }


        [Test]
        public void ToString_ReturnsStringRepresentation()
        {
            // Arrange
            string id = "testId";
            var infoIdentifier = new InfoIdentifier(id);

            // Act
            var result = infoIdentifier.ToString();
            
            // Assert
            Assert.That(result, Is.EqualTo(id));
        }
    }
}
