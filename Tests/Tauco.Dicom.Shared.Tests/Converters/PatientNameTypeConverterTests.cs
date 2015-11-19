using System;
using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using NUnit.Framework;

using Tauco.Dicom.Shared.Converters;

namespace Tauco.Dicom.Shared.Tests.Converters
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class PatientNameTypeConverterTests
    {
        [Test]
        public void Convert_NullContext_ThrowsException()
        {
            // Arrange
            var patientNameTypeConverter = new PatientNameTypeConverter();

            // Act + Assert
            Assert.That(() => patientNameTypeConverter.Convert(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Convert_ConvertsStringToInfoIdentifier()
        {
            // Arrange
            Mapper.CreateMap<string, PatientName>()
                .ConvertUsing<PatientNameTypeConverter>();
             
            // Act
            var patientName = Mapper.Engine.DynamicMap<string, PatientName>("Doe^John");

            // Assert
            Assert.That(patientName.DicomString, Is.EqualTo("Doe^John"));
        }
    }
}
