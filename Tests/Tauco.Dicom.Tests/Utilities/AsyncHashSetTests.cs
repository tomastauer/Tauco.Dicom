using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Nito.AsyncEx;

using NUnit.Framework;

using Tauco.Dicom.Shared;
using Tauco.Dicom.Utilities;

namespace Tauco.Dicom.Tests.Utilities
{
    [TestFixture]
    [SuppressMessage("ReSharper", "StringLiteralsWordIsNotInDictionary")]
    public class AsyncHashSetTests
    {
        private class MultipleInstance : IMultipleInstance<MultipleInstance>
        {
            public MultipleInstance()
            {
                AdditionalInstances = new List<MultipleInstance>();
            }


            public string Item
            {
                get;
                set;
            }


            public int GetIdentifierHashCode()
            {
                return Item.GetHashCode();
            }


            public IList<MultipleInstance> AdditionalInstances
            {
                get;
            }
        }


        [Test]
        public void Add_TwoItemsWithDifferentHashCode_ReturnsBoth()
        {
            // Arrange
            var hashSet = new AsyncHashSet<object>();
            hashSet.Add("aaa");
            hashSet.Add("bbb");

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();

            // Assert
            Assert.That(list[0], Is.EqualTo("aaa"));
            Assert.That(list[1], Is.EqualTo("bbb"));
        }


        [Test]
        public void Add_TwoItemsWithSameHashCode_ReturnsBoth()
        {
            // Arrange
            var hashSet = new AsyncHashSet<object>();
            hashSet.Add("aaa");
            hashSet.Add("aaa");

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();

            // Assert
            Assert.That(list[0], Is.EqualTo("aaa"));
            Assert.That(list[1], Is.EqualTo("aaa"));
        }


        [Test]
        public void Add_TwoItemsWithSameIdentifierHashCode_ImplementingIMultipleInstance_ReturnsFirstWithAdditionalItem()
        {
            // Arrange
            var hashSet = new AsyncHashSet<MultipleInstance>();
            MultipleInstance first = new MultipleInstance
            {
                Item = "aaa"
            };

            MultipleInstance second = new MultipleInstance
            {
                Item = "aaa"
            };

            hashSet.Add(first);
            hashSet.Add(second);

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();
            MultipleInstance item = list.Single();

            // Assert
            Assert.That(item.Item, Is.EqualTo("aaa"));
            Assert.That(item.AdditionalInstances.First().Item, Is.EqualTo("aaa"));
        }


        [Test]
        public async void AddAsync_TwoItemsWithDifferentHashCode_ReturnsBoth()
        {
            // Arrange
            var hashSet = new AsyncHashSet<object>();
            await hashSet.AddAsync("aaa");
            await hashSet.AddAsync("bbb");

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();

            // Assert
            Assert.That(list[0], Is.EqualTo("aaa"));
            Assert.That(list[1], Is.EqualTo("bbb"));
        }


        [Test]
        public async void AddAsync_TwoItemsWithSameHashCode_ReturnsBoth()
        {
            // Arrange
            var hashSet = new AsyncHashSet<object>();
            await hashSet.AddAsync("aaa");
            await hashSet.AddAsync("aaa");

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();

            // Assert
            Assert.That(list[0], Is.EqualTo("aaa"));
            Assert.That(list[1], Is.EqualTo("aaa"));
        }


        [Test]
        public async void AddAsync_TwoItemsWithSameIdentifierHashCode_ImplementingIMultipleInstance_ReturnsFirstWithAdditionalItem()
        {
            // Arrange
            var hashSet = new AsyncHashSet<MultipleInstance>();
            MultipleInstance first = new MultipleInstance
            {
                Item = "aaa"
            };

            MultipleInstance second = new MultipleInstance
            {
                Item = "aaa"
            };

            await hashSet.AddAsync(first);
            await hashSet.AddAsync(second);

            // Act
            var list = hashSet.GetConsumingEnumerable().ToList();
            MultipleInstance item = list.Single();

            // Assert
            Assert.That(item.Item, Is.EqualTo("aaa"));
            Assert.That(item.AdditionalInstances.First().Item, Is.EqualTo("aaa"));
        }


        [Test]
        public async void TakeAsync_CompletedAdding_ReturnsNull()
        {
            // Arrange
            var hashSet = new AsyncHashSet<string>();
            hashSet.Add("aaa");
            hashSet.Add("bbb");
            hashSet.Add("ccc");

            // Act
            var result = hashSet.GetConsumingEnumerable().ToList();
            AsyncCollection<string>.TakeResult singleItem = await hashSet.TakeAsync();

            // Assert
            Assert.That(singleItem.Item, Is.Null);
            Assert.That(result.Count, Is.EqualTo(3));
        }


        [Test]
        public async void TakeAsync_ReturnFirstItem_RemovesItemFromCollection()
        {
            // Arrange
            var hashSet = new AsyncHashSet<string>();
            hashSet.Add("aaa");
            hashSet.Add("bbb");
            hashSet.Add("ccc");

            // Act
            AsyncCollection<string>.TakeResult singleItem = await hashSet.TakeAsync();
            var result = hashSet.GetConsumingEnumerable().ToList();

            // Assert
            Assert.That(singleItem.Item, Is.EqualTo("aaa"));
            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}