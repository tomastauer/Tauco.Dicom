using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public class BirthNumberTests
    {
        [TestCase("910725/6444", "9107256444")]
        [TestCase("9107256444", "9107256444")]
        public void StringRepresentationWithoutSlash_ReturnsBirthNumberWithoutSlash(string input, string expectedResult)
        {
            // Arrange
            var birthNumber = new BirthNumber(input);

            // Act
            var result = birthNumber.StringRepresentationWithoutSlash;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCase("910725/6444", "910725/6444")]
        [TestCase("9107256444", "910725/6444")]
        public void StringRepresentationWithSlash_ReturnsBirthNumberWithSlash(string input, string expectedResult)
        {
            // Arrange
            var birthNumber = new BirthNumber(input);

            // Act
            var result = birthNumber.StringRepresentationWithSlash;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [TestCase("910725/6444", "2564", true)]
        [TestCase("910725/6444", "25/64", true)]
        [TestCase("9107256444", "2564", true)]
        [TestCase("9107256444", "25/64", true)]
        [TestCase("9107256444", "666", false)]
        [TestCase("9107256444", 666, false)]
        public void Contains_ReturnsExpectedValue(string input, object value, bool expectedValue)
        {
            // Arrange
            var birthNumber = new BirthNumber(input);

            // Act
            var result = birthNumber.Contains(value);

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));
        }


        [TestCase("910725/6444", "9107256444", true)]
        [TestCase("910725/6444", "910725/6444", true)]
        [TestCase("9107256444", "910725/6444", true)]
        [TestCase("9107256444", "9107256444", true)]
        [TestCase("9107256444", "9107256442", false)]
        public void Equals_MatchEqualObject(string firstInput, string secondInput, bool expectedResult)
        {
            // Arrange
            var firstBirthNumber = new BirthNumber(firstInput);
            var secondBirthNumber = new BirthNumber(secondInput);

            // Act
            bool result = firstBirthNumber.Equals(secondBirthNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [Test]
        public void Equals_DoesNotMatchNull()
        {
            // Arrange
            var birthNumber = new BirthNumber("910725/6444");

            // Act
            var result = birthNumber.Equals(null);

            // Arrange
            Assert.That(result, Is.False);
        }


        [Test]
        public void Equals_DoesMatchItself()
        {
            // Arrange
            var birthNumber = new BirthNumber("910725/6444");

            // Act
            var result = birthNumber.Equals(birthNumber);

            // Arrange
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_DoesMatchString([Values("910725/6444", "9107256444")] string firstNumber, [Values("910725/6444", "9107256444")] string secondNumber)
        {
            // Arrange
            var birthNumber = new BirthNumber(firstNumber);

            // Act
            var result = birthNumber.Equals(secondNumber);

            // Arrange
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_DoesNotMatchDifferentType()
        {
            // Arrange
            var birthNumber = new BirthNumber("910725/6444");

            // Act
            var result = birthNumber.Equals(1);

            // Arrange
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_SameCodeForEqualObjects()
        {
            // Arrange
            var firtBirthNumber = new BirthNumber("910725/6444");
            var secondBirthNumber = new BirthNumber("9107256444");

            // Act
            int firstHashCode = firtBirthNumber.GetHashCode();
            int secondHashCode = secondBirthNumber.GetHashCode();

            // Assert
            Assert.That(firstHashCode, Is.EqualTo(secondHashCode));
        }
    }
}
