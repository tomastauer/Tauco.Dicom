using NUnit.Framework;

namespace Tauco.Dicom.Shared.Tests
{
    [TestFixture]
    public class DicomAttributeTests
    {
        [Test]
        public void Constructor_DicomTagUndefined_ThrowsArgumentException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new DicomAttribute(DicomTags.Undefined), Throws.ArgumentException);
        }
    }
}
