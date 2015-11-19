using System;
using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using NUnit.Framework;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared.Tests.Converters
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class InfoIdentifierTypeConverterTests
    {
        [Test]
        public void Convert_NullContext_ThrowsException()
        {
            // Arrange
            var infoIdentifierTypeConverter = new InfoIdentifierTypeConverter();

            // Act + Assert
            Assert.That(() => infoIdentifierTypeConverter.Convert(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Convert_ConvertsStringToInfoIdentifier()
        {
            // Arrange
            Mapper.CreateMap<string, InfoIdentifier>().ConvertUsing<InfoIdentifierTypeConverter>();

            // Act
            var identifier = Mapper.DynamicMap<string, InfoIdentifier>("1.2.3");
            
            // Assert
            Assert.That(identifier.StringRepresentation, Is.EqualTo("1.2.3"));
        }
    }
}
