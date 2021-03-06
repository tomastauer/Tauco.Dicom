﻿using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using Tauco.Dicom.Shared;

namespace Tauco.Dicom.Abstraction.FellowOak.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class FellowOakDicomMoveRequestTests
    {
        [Test]
        public void Constructor_NullDestinationAE_ThrowsException()
        {
            // Act + Arrange + Assert
            Assert.That(() => new FellowOakDicomMoveRequest(null, new InfoIdentifier("identifier")), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Constructor_NullIdentifier_ThrowsException()
        {
            // Act + Arrange + Assert
            Assert.That(() => new FellowOakDicomMoveRequest("destinationAE", null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
