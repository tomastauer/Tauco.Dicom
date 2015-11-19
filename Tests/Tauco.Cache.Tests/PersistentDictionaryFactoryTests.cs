using System;
using System.IO;

using Microsoft.Isam.Esent.Collections.Generic;

using NUnit.Framework;

namespace Tauco.Cache.Tests
{
    [TestFixture]
    public class PersistentDictionaryFactoryTests
    {
        [TearDown]
        public void TearDown()
        {
            DirectoryInfo directory = new DirectoryInfo("test");
            if (directory.Exists)
            {
                directory.Delete(true);
            }
        }


        [Test]
        public void GetPersistentDictionary_NullKey_ThrowsException()
        {
            // Arrange
            var persistentDictionaryFactory = new PersistentDictionaryFactory();

            // Act + Assert
            Assert.That(() => persistentDictionaryFactory.GetPersistentDictionary(null), Throws.TypeOf<ArgumentNullException>());
        }
        

        [Test]
        public void GetPersistentDictionary_NotNullKey_ReturnsDictionary()
        {
            // Arrange
            var persistentDictionaryFactory = new PersistentDictionaryFactory();

            // Act
            var dictionary = persistentDictionaryFactory.GetPersistentDictionary("test");

            // Assert
            Assert.That(dictionary, Is.Not.Null);
            Assert.That(dictionary, Is.InstanceOf<PersistentDictionary<string, string>>());

            persistentDictionaryFactory.Dispose();
        }
        

        [Test]
        public void GetPersistentDictionary_NotNullKey_ReturnsSameDictionaries()
        {
            // Arrange
            var persistentDictionaryFactory = new PersistentDictionaryFactory();

            // Act
            var firstDictionary = persistentDictionaryFactory.GetPersistentDictionary("test");
            var secondDictionary = persistentDictionaryFactory.GetPersistentDictionary("test");

            // Assert
            Assert.That(firstDictionary, Is.SameAs(secondDictionary));

            persistentDictionaryFactory.Dispose();
        }
    }
}
