using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class BirthNumberParserTests
    {
        [Test]
        public void GetBirthDate_NullBirthNumber_ThrowsException()
        {
            // Arrange
            var birthNumberParser = new BirthNumberParser();

            // Act + Assert
            Assert.That(() => birthNumberParser.GetBirthNumber(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [TestCase("7801233540", "1978-01-23", Gender.Male, 354, 0)] // Not divisible by 11 male
        [TestCase("7955051270", "1979-05-05", Gender.Female, 127, 0)] // Not divisible by 11 female
        [TestCase("490808155", "1949-08-08", Gender.Male, 155, 10)]  // Before 1954 male
        [TestCase("295612449", "1929-06-12", Gender.Female, 449, 10)]  // Before 1954 male
        [TestCase("870207600", "1887-02-07", Gender.Male, 600, 10)] // 19th century male
        [TestCase("605201201", "1860-02-01", Gender.Female, 201, 10)] // 19th century female
        [TestCase("490808150", "1949-08-08", Gender.Male, 150, 10)]  // Before 1954 not divisible by 11 male
        [TestCase("295612445", "1929-06-12", Gender.Female, 445, 10)]  // Before 1954 not divisible by 11 female
        [TestCase("0921124567", "2009-01-12", Gender.Male, 456, 7)] // After 2004 male
        [TestCase("1282121214", "2012-12-12", Gender.Female, 121, 4)] // After 2004 female
        [TestCase("5312249514", "2053-12-24", Gender.Male, 951, 4)] // After 2050 male
        [TestCase("4857094540", "2048-07-09", Gender.Female, 454, 0)] // After 2050 female
        [TestCase("910725/6444", "1991-07-25", Gender.Male, 644, 4)] // With slash male
        [TestCase("945601/1070", "1994-06-01", Gender.Female, 107, 0)] // With slash female
        public void GetBirthDate_CorrectBirthNumberIsParsed(string birthNumber, string expectedDate, Gender expectedGender, short expectedSuffix, byte expectedLastDigit)
        {
            
            // Arrange
            var birthNumberParser = new BirthNumberParser();

            // Act
            var result = birthNumberParser.GetBirthNumber(birthNumber);

            // Assert
            Assert.That(result.BirthDate, Is.EqualTo(DateTime.Parse(expectedDate)));
            Assert.That(result.Gender, Is.EqualTo(expectedGender));
            Assert.That(result.Suffix, Is.EqualTo(expectedSuffix));
            Assert.That(result.CheckDigit, Is.EqualTo(expectedLastDigit == 10 ? (byte?)null : expectedLastDigit));
        }
        

        [TestCase("some text")]
        [TestCase("132 text")]
        [TestCase("")]
        [TestCase("87020763391")] 
        [TestCase("9026128430")] // Wrong gender format 
        [TestCase("9107256440")] // Wrong check number
        [TestCase("9002301110")] // Non existing date
        public void GetBirthNumber_IncorrectBirthNumberIsRefused(string birthNumber)
        {
            // Arrange
            var birthNumberParser = new BirthNumberParser();

            // Act
            var result = birthNumberParser.GetBirthNumber(birthNumber);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
