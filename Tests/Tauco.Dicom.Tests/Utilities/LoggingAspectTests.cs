using System;
using System.Diagnostics.CodeAnalysis;

using Castle.Core.Logging;
using Castle.DynamicProxy;

using NSubstitute;

using NUnit.Framework;

using Tauco.Dicom.Utilities;

namespace Tauco.Dicom.Tests.Utilities
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class LoggingAspectTests
    {
        [Test]
        public void Constructor_NullLogger_ThrowsException()
        {
            // Arrange + Act + Assert
            Assert.That(() => new LoggingAspect(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        public void Intercept_LoggerIsCalled()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var loggingAspect = new LoggingAspect(logger);
            var invocation = Substitute.For<IInvocation>();
            var method = typeof (object).GetMethod("ToString");

            invocation.Method.Returns(method);
            invocation.When(c=>c.Proceed()).Do(c =>
            {
                throw new Exception();
            });

            // Act
            loggingAspect.Intercept(invocation);

            // Assert
            Assert.That(() => logger.Received(2).Debug(Arg.Any<string>()), Throws.Nothing);
            Assert.That(() => logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>()), Throws.Nothing);
        }
    }
}
