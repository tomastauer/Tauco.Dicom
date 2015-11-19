using System;
using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared.Tests.Converters
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class BirthNumberTypeConverterTests
    {
        [Test]
        public void Constructor_NullBirthNumberParser_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new BirthNumberTypeConverter(null), Throws.InstanceOf<ArgumentNullException>());    
        }


        [Test]
        public void Convert_NullContext_ThrowsException()
        {
            // Arrange
            var birthNumberParser = Substitute.For<IBirthNumberParser>();
            var birthNumberTypeConverter = new BirthNumberTypeConverter(birthNumberParser);

            // Act + Assert
            Assert.That(() => birthNumberTypeConverter.Convert(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Convert_ConvertsStringToInfoIdentifier()
        {
            // Arrange
            var birthNumberParser = Substitute.For<IBirthNumberParser>();
            birthNumberParser.GetBirthNumber(Arg.Any<string>()).Returns(new BirthNumber("9107256444")
            {
                Gender = Gender.Male,
                BirthDate = new DateTime(1991, 7, 25),
                Suffix = 644,
                CheckDigit = 4
            });

            Mapper.CreateMap<string, BirthNumber>()
                .ConvertUsing(new BirthNumberTypeConverter(birthNumberParser));
             
            // Act
            var birthNumber = Mapper.Engine.DynamicMap<string, BirthNumber>("910725/6444");

            // Assert
            Assert.That(birthNumber.Gender, Is.EqualTo(Gender.Male));
            Assert.That(birthNumber.BirthDate, Is.EqualTo(new DateTime(1991, 7, 25)));
            Assert.That(birthNumber.Suffix, Is.EqualTo(644));
            Assert.That(birthNumber.CheckDigit, Is.EqualTo(4));
        }
    }
}
