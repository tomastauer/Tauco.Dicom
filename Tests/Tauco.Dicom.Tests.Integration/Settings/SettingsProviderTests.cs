using NUnit.Framework;

namespace Tauco.Dicom.Tests.Integration
{
    [TestFixture]
    public class SettingsProviderTests
    {
        [Test]
        public void CalledApplicationEntity_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.CalledApplicationEntity;

            // Assert
            Assert.That(result, Is.EqualTo("ORTHANC"));
        }


        [Test]
        public void CallingApplicationEntity_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.CallingApplicationEntity;

            // Assert
            Assert.That(result, Is.EqualTo("TAUCO"));
        }


        [Test]
        public void DestinationApplicationEntity_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.DestinationApplicationEntity;

            // Assert
            Assert.That(result, Is.EqualTo("TAUCO"));
        }


        [Test]
        public void LocalAddress_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.LocalAddress;

            // Assert
            Assert.That(result, Is.EqualTo("127.0.0.1"));
        }


        [Test]
        public void LocalPort_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.LocalPort;

            // Assert
            Assert.That(result, Is.EqualTo(104));
        }


        [Test]
        public void RemoteAddress_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.RemoteAddress;

            // Assert
            Assert.That(result, Is.EqualTo("127.0.0.1"));
        }


        [Test]
        public void RemotePort_ReturnsExpectedValue()
        {
            // Arrange
            SettingsProvider settingsProvider = new SettingsProvider();

            // Act
            var result = settingsProvider.RemotePort;

            // Assert
            Assert.That(result, Is.EqualTo(4242));
        }
    }
}