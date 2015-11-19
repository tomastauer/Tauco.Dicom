using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    public class PatientNameTests
    {
        [Test]
        public void Constructor_NullInput_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new PatientName(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [TestCase("Doe^John", "Doe", "John", null, null, null, "John Doe")]
        [TestCase("Obama^Barack^Hussein^President^II", "Obama", "Barack", "Hussein", "President", "II", "President Barack Hussein Obama II")]
        [TestCase("Windsor^Elisabeth^^Queen^II", "Windsor", "Elisabeth", null, "Queen", "II", "Queen Elisabeth Windsor II")]
        [TestCase("Simpson^Homer^Jay", "Simpson", "Homer", "Jay", null, null, "Homer Jay Simpson")]
        [TestCase("^Charles^^Emperor^IV", null, "Charles", null, "Emperor", "IV", "Emperor Charles IV")]
        public void Constructor_NameIsCorrectlyParsed(string input, string expectedLastName, string expectedFirstName, string expectedMiddleName, string expectedPrefix, string expectedSuffix,
            string expectedToString)
        {
            // Arrange + Act
            var patientName = new PatientName(input);

            // Assert
            Assert.That(patientName.LastName, Is.EqualTo(expectedLastName));
            Assert.That(patientName.FirstName, Is.EqualTo(expectedFirstName));
            Assert.That(patientName.MiddleName, Is.EqualTo(expectedMiddleName));
            Assert.That(patientName.Prefix, Is.EqualTo(expectedPrefix));
            Assert.That(patientName.Suffix, Is.EqualTo(expectedSuffix));
            Assert.That(patientName.DicomString, Is.EqualTo(input));
            Assert.That(patientName.ToString(), Is.EqualTo(expectedToString));
        }


        [TestCase("Doe^John", "Doe", true)]
        [TestCase("Doe^John", "Jack", false)]
        [TestCase("Doe^John", "^", true)]
        [TestCase("Doe^John", 1, false)]
        public void Contains_ReturnsExpectedValue(string name, object value, bool expectedValue)
        {
            // Arrange
            var patientName = new PatientName(name);

            // Act
            var result = patientName.Contains(value);

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));
        }


        [TestCase("testId", "testId", true)]
        [TestCase("testId", "anotherId", false)]
        public void Equals_MatchEqualObject(string firstName, string secondName, bool expectedResult)
        {
            // Arrange
            var firstPatientName = new PatientName(firstName);
            var secondPatientName = new PatientName(secondName);

            // Act
            bool result = firstPatientName.Equals(secondPatientName);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [Test]
        public void Equals_DoesNotMatchNull()
        {
            // Arrange
            var patientName = new PatientName("test");

            // Act
            var result = patientName.Equals(null);

            // Arrange
            Assert.That(result, Is.False);
        }

        
        [Test]
        public void Equals_DoesMatchItself()
        {
            // Arrange
            var patientName = new PatientName("test");

            // Act
            var result = patientName.Equals(patientName);

            // Arrange
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_DoesMatchString()
        {
            // Arrange
            var patientName = new PatientName("test");

            // Act
            var result = patientName.Equals("test");

            // Arrange
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_DoesNotMatchDifferentType()
        {
            // Arrange
            var patientName = new PatientName("test");

            // Act
            var result = patientName.Equals(1);

            // Arrange
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_SameCodeForEqualObjects()
        {
            // Arrange
            string name = "testName";
            var firstPatientName = new PatientName(name);
            var secondPatientName = new PatientName(name);

            // Act
            int firstHashCode = firstPatientName.GetHashCode();
            int secondHashCode = secondPatientName.GetHashCode();

            // Assert
            Assert.That(firstHashCode, Is.EqualTo(secondHashCode));
        }
    }
}
