using NUnit.Framework;

using Tauco.Dicom.Models;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Tests.Models
{
    [TestFixture]
    public class GeneralizedInfoProviderTests
    {
        [Test]
        public void GetGeneralizedInfo_TwoCalls_ReturnsSameInstance()
        {
            // Arrange
            var generalizedInfoProvider = new GeneralizedInfoProvider();

            // Act
            var firstInfo = generalizedInfoProvider.GetGeneralizedInfo<TestInfo>();
            var secondInfo = generalizedInfoProvider.GetGeneralizedInfo<TestInfo>();

            // Assert
            Assert.That(firstInfo, Is.SameAs(secondInfo));
        }
    }
}
