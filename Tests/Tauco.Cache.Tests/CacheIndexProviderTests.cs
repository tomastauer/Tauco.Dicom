using NUnit.Framework;

namespace Tauco.Cache.Tests
{
    [TestFixture]
    public class CacheIndexProviderTests
    {
        [Test]
        public void GetCacheIndex_WithAttribute_ReturnsValue()
        {
            // Arrange
            TestInfoWithAttribute info = new TestInfoWithAttribute
            {
                Index = 1
            };

            CacheIndexProvider cacheIndexProvider = new CacheIndexProvider();

            // Act
            var index = cacheIndexProvider.GetCacheIndex(info);

            // Assert
            Assert.That(index, Is.EqualTo("1"));
        }


        [Test]
        public void GetCacheIndex_WithMoreAttributes_ReturnsFirstValue()
        {
            // Arrange
            TestInfoWithTwoAttributes info = new TestInfoWithTwoAttributes
            {
                Index = 1,
                Index2 = 2
            };

            CacheIndexProvider cacheIndexProvider = new CacheIndexProvider();

            // Act
            var index = cacheIndexProvider.GetCacheIndex(info);

            // Assert
            Assert.That(index, Is.EqualTo("1"));
        }


        [Test]
        public void GetCacheIndex_WithoutAttribute_ThrowsException()
        {
            // Arrange
            TestInfoWithoutAttribute info = new TestInfoWithoutAttribute
            {
                Index = 1
            };

            CacheIndexProvider cacheIndexProvider = new CacheIndexProvider();

            // Act + Assert
            Assert.That(() => cacheIndexProvider.GetCacheIndex(info), Throws.ArgumentException);
        }

        private class TestInfoWithoutAttribute
        {
            public int Index
            {
                get;
                set;
            }
        }


        private class TestInfoWithAttribute
        {
            [CacheIndex]
            public int Index
            {
                get;
                set;
            }
        }


        private class TestInfoWithTwoAttributes
        {
            [CacheIndex]
            public int Index
            {
                get;
                set;
            }


            [CacheIndex]
            public int Index2
            {
                get;
                set;
            }
        }
    }
}