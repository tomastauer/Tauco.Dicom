using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Castle.Core;

using NUnit.Framework;

using Tauco.Dicom.Utilities;

namespace Tauco.Dicom.Tests.Utilities
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class InterceptorSelectorTests
    {
        [Test]
        public void HasInterceptors_NullComponentModel_ThrowsException()
        {
            // Arrange
            var interceptorSelector = new InterceptorSelector();
         
            // Act + Assert
            Assert.That(() => interceptorSelector.HasInterceptors(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void HasInterceptors_ComponentModelIsLoggingAspect_ReturnsFalse()
        {
            // Arrange
            var interceptorSelector = new InterceptorSelector();
            var componentModel = new ComponentModel(new ComponentName("name", true), new List<Type> { typeof(object) }, typeof(LoggingAspect), null);

            // Act
            var result = interceptorSelector.HasInterceptors(componentModel);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void HasInterceptors_ComponentModelIsNotLoggingAspect_ReturnsTrue()
        {
            // Arrange
            var interceptorSelector = new InterceptorSelector();
            var componentModel = new ComponentModel(new ComponentName("name", true), new List<Type> { typeof(object) }, typeof(object), null);

            // Act
            var result = interceptorSelector.HasInterceptors(componentModel);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void SelectInterceptors_AddsLoggingAspectToInterceptors()
        {
            // Arrange
            var interceptorSelector = new InterceptorSelector();
            var interceptorReference = new InterceptorReference[0];
            
            // Act
            var result = interceptorSelector.SelectInterceptors(null, interceptorReference);

            // Assert
            Assert.That(result.First().ToString(), Is.EqualTo(typeof(LoggingAspect).FullName));
        }
    }
}