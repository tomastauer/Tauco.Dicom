using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Tests;
using Tauco.Tests.Fakes;

namespace Tauco.Dicom.Console.Tests
{
    [TestFixture]
    public class InputArgumentTests
    {
        [Test]
        public void Constructor_ArgumentsCombinations()
        {
            var mockProvider = new MockProvider();
            TestUtilities.TestConstructorArgumentsNullCombinations(typeof(InputArguments), null, new List<Func<object>>
            {
                mockProvider.GetSettingsProviderFake,
                mockProvider.GetBirthNumberParserFake
            });
        }


        [Test]
        public void Validate_Download_Cache()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Download = true;
            inputArguments.UseCache = true;
            inputArguments.Identifier = "identifier";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Contains("Images cannot be downloaded from the cache"));
            Assert.That(result.Contains("Type has to be 'patient', 'study' or 'series'"));
        }


        [Test]
        public void Validate_Download_NoIdentifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Download = true;
            inputArguments.Identifier = null;

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Contains("For downloading images, identifier has to be specified"));
            Assert.That(result.Contains("Type has to be 'patient', 'study' or 'series'"));
        }


        [Test]
        public void Validate_Download_NullDestinationServerLocation()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Download = true;
            inputArguments.DestinationApplicationEntity = null;
            inputArguments.LocalPort = null;
            inputArguments.LocalAddress = null;

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.Contains("For downloading images, destination application entity has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("For downloading images, local address has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("For downloading images, local port has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("For downloading images, identifier has to be specified"));
            Assert.That(result.Contains("Type has to be 'patient', 'study' or 'series'"));
        }


        [Test]
        public void Validate_NullServerLocation()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.CalledApplicationEntity = null;
            inputArguments.CallingApplicationEntity = null;
            inputArguments.RemoteAddress = null;
            inputArguments.RemotePort = null;

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.Contains("Calling application entity has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("Called application entity has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("Remote address has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("Remote port has to be set either in app.config or with the input argument"));
            Assert.That(result.Contains("Type has to be 'patient', 'study' or 'series'"));
        }


        [Test]
        public void Validate_Patient_Identifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "patient";
            inputArguments.Identifier = "9107256444";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void Validate_Patient_ParentIdentifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "patient";
            inputArguments.ParentIdentifier = "identifier";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("Patients don't have parent."));
        }


        [Test]
        public void Validate_Patient_WrongIdentifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "patient";
            inputArguments.Identifier = "identifier";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("Provided identifier is not a valid birth number"));
        }


        [Test]
        public void Validate_Series_Download()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "series";
            inputArguments.Identifier = "identifier";
            inputArguments.Download = true;

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("Images downloading is permitted for patients and studies only"));
        }


        [Test]
        public void Validate_Study_ParentIdentifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "study";
            inputArguments.ParentIdentifier = "9107256444";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void Validate_Study_WrongParentIdentifier()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "study";
            inputArguments.ParentIdentifier = "identifier";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("Provided parent identifier is not a valid birth number"));
        }


        [Test]
        public void Validate_WrongType()
        {
            // Arrange
            var inputArguments = GetDefaultInputArguments();
            inputArguments.Type = "Unknown";

            // Act
            var result = inputArguments.Validate().ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("Type has to be 'patient', 'study' or 'series'"));
        }


        private InputArguments GetDefaultInputArguments()
        {
            var mockProvider = new MockProvider();
            var inputArguments = new InputArguments(mockProvider.GetSettingsProviderFake(), new BirthNumberParser())
            {
                CalledApplicationEntity = "calledAE",
                CallingApplicationEntity = "callingAE",
                RemoteAddress = "remoteAddr",
                RemotePort = 666,
                DestinationApplicationEntity = "destinationAE",
                LocalAddress = "localAddr",
                LocalPort = 666
            };

            return inputArguments;
        }
    }
}