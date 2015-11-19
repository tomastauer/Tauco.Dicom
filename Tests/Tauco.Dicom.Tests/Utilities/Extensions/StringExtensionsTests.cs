using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NUnit.Framework;

using Tauco.Dicom.Utilities.Extensions;

namespace Tauco.Dicom.Tests.Utilities
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class StringExtensionsTests
    {
        [Test]
        public void Contains_NullValue_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => StringExtensions.Contains(string.Empty, null, CultureInfo.CurrentCulture), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Contains_NullCulture_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => StringExtensions.Contains(string.Empty, string.Empty, null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Contains_SameCulture_ReturnsTrue()
        {
            // Arrange
            string value1 = "TIN";
            string value2 = "tin";
            var culture = CultureInfo.GetCultureInfo("en-US");

            // Act
            bool result = StringExtensions.Contains(value1, value2, culture);
            
            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Contains_DifferentCultures_ReturnsFalse()
        {
            // Arrange
            string value1 = "TIN";
            string value2 = "tin";
            var culture = CultureInfo.GetCultureInfo("tr-TR");

            // Act
            bool result = StringExtensions.Contains(value1, value2, culture);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
