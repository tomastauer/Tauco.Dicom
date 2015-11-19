using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Tauco.Cache.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class CacheProviderTests
    {
        [SetUp]
        public void SetUp()
        {
            mPersistentDictionaryFactory = new PersistentDictionaryFactory();
            mCacheProvider = new CacheProvider(mPersistentDictionaryFactory);
        }


        [TearDown]
        public void TearDown()
        {
            mCacheProvider.Dispose();
            mPersistentDictionaryFactory.Dispose();
            
            DirectoryInfo directory = new DirectoryInfo("Cache");
            if (directory.Exists)
            {
                directory.Delete(true);
            }
        }


        private ICacheProvider mCacheProvider;
        private IPersistentDictionaryFactory mPersistentDictionaryFactory;


        [Test]
        public void Constructor_NullArgument_ThrowsException()
        {
            // Act + Assert
            Assert.That(() => new CacheProvider(null), Throws.TypeOf<ArgumentNullException>());
        }


        [TestCase(null, null)]
        [TestCase("test", null)]
        [TestCase(null, "test")]
        public void Store_NullKeys_ThrowsArgumentNullException(string key, object value)
        {
            // Act + Assert
            Assert.That(() => mCacheProvider.Store(key, value), Throws.TypeOf<ArgumentNullException>());
        }
        

        [Test]
        public void ClearCache()
        {
            // Act
            mCacheProvider.Store("test", "test");
            mCacheProvider.ClearCache<string>();

            var items = mCacheProvider.Retrieve<string>().ToList();

            // Assert
            CollectionAssert.IsEmpty(items);
        }


        [Test]
        public void Retrieve_All_EmptyCache()
        {
            // Act
            var items = mCacheProvider.Retrieve<string>();

            // Assert
            CollectionAssert.IsEmpty(items);
        }


        [Test]
        public void Retrieve_All_FullCache()
        {
            // Act
            mCacheProvider.Store("test", "test");
            mCacheProvider.Store("test2", "test");
            mCacheProvider.Store("test3", "test");
            var items = mCacheProvider.Retrieve<string>().ToList();

            // Assert
            Assert.That(items.Count, Is.EqualTo(3));
        }


        [Test]
        public void Retrieve_NullKey_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.That(() => mCacheProvider.Retrieve<string>(null), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Retrieve_One_EmptyCache()
        {
            // Act
            var item = mCacheProvider.Retrieve<string>("test");

            // Assert
            Assert.That(item, Is.Null);
        }


        [Test]
        public void Store_CreatesDictionary()
        {
            // Act
            mCacheProvider.Store("test", "test");

            // Assert
            Assert.That(Directory.Exists("Cache\\String"), Is.True);
        }


        [Test]
        public void Store_ItemCanBeRetrieved()
        {
            // Act
            mCacheProvider.Store("test", "test");
            var result = mCacheProvider.Retrieve<string>("test");

            // Assert
            Assert.That(result, Is.EqualTo("test"));
        }


        [Test]
        public void Store_SameKey_NotRewritten()
        {
            // Act
            mCacheProvider.Store("test", "test");
            mCacheProvider.Store("test", "test2", false);
            var result = mCacheProvider.Retrieve<string>("test");

            // Assert
            Assert.That(result, Is.EqualTo("test"));
        }


        [Test]
        public void Store_SameKey_Rewritten()
        {
            // Act
            mCacheProvider.Store("test", "test");
            mCacheProvider.Store("test", "test2", true);
            var result = mCacheProvider.Retrieve<string>("test");

            // Assert
            Assert.That(result, Is.EqualTo("test2"));
        }
    }
}