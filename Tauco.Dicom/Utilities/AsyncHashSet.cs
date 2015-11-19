using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nito.AsyncEx;

using Tauco.Dicom.Models.InfoObjects;

namespace Tauco.Dicom.Utilities
{
    /// <summary>
    /// An async-compatible producer/consumer collection.
    /// </summary>
    /// <typeparam name="TInfo">The type of elements contained in the collection.</typeparam>
    internal class AsyncHashSet<TInfo>
    {
        private readonly Dictionary<int, TInfo> mAddedItems = new Dictionary<int, TInfo>();
        private readonly AsyncCollection<TInfo> mCollection = new AsyncCollection<TInfo>();

        private readonly List<TInfo> mToBeAdded = new List<TInfo>();


        /// <summary>
        /// Adds an item to the producer/consumer collection. If item with the same hash code was already added, tries to add the
        /// current item before adding completion of all items. This method may block the calling thread.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="InvalidOperationException">
        /// The producer/consumer collection has completed adding or if the item was
        /// rejected by the underlying collection.
        /// </exception>
        public void Add(TInfo item)
        {
            if (AddInternal(item))
            {
                mCollection.Add(item);
            }
        }


        /// <summary>
        /// Takes an item from the producer/consumer collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The producer/consumer collection has completed adding or if the item was
        /// rejected by the underlying collection.
        /// </exception>
        public Task<AsyncCollection<TInfo>.TakeResult> TakeAsync()
        {
            return mCollection.TryTakeAsync();
        }


        /// <summary>
        /// Adds an item to the producer/consumer collection. If item with the same hash code was already added, tries to add the
        /// current item before adding completion of all items.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="InvalidOperationException">
        /// The producer/consumer collection has completed adding or if the item was
        /// rejected by the underlying collection.
        /// </exception>
        public Task AddAsync(TInfo item)
        {
            return AddInternal(item) ? mCollection.AddAsync(item) : TaskConstants.Completed;
        }


        /// <summary>
        /// Provides a (synchronous) consuming enumerable for items in the producer/consumer queue.
        /// </summary>
        public IEnumerable<TInfo> GetConsumingEnumerable()
        {
            CompleteAdding();
            return mCollection.GetConsumingEnumerable();
        }


        /// <summary>
        /// Adds an item to the producer/consumer collection. If item with the same hash code was already added, tries to add the
        /// current item before adding completion of all items.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>True, if item was added to the collection; otherwise, false</returns>
        private bool AddInternal(TInfo item)
        {
            int hashCode;

            if (TryGetIdentifierHashCode(item, out hashCode))
            {
                if (mAddedItems.ContainsKey(hashCode))
                {
                    mToBeAdded.Add(item);
                    return false;
                }
            }

            if (!mAddedItems.ContainsKey(hashCode))
            {
                mAddedItems.Add(hashCode, item);
            }
            return true;
        }


        /// <summary>
        /// Synchronously marks the producer/consumer collection as complete for adding.
        /// </summary>
        private void CompleteAdding()
        {
            HandleMultipleItems();
            mCollection.CompleteAdding();
        }


        /// <summary>
        /// It <see cref="TInfo" /> implements <see cref="IMultipleInstance{TInfo}" />, for every item that cannot be added due to
        /// the object with same hash code already existed in underlying collection, tries add the item to the AdditionalInstances
        /// collection of original item.
        /// Otherwise tries to add item at the end of the collection.
        /// </summary>
        private void HandleMultipleItems()
        {
            foreach (TInfo item in mToBeAdded)
            {
                int hashCode;
                TryGetIdentifierHashCode(item, out hashCode);

                var original = mAddedItems[hashCode] as IMultipleInstance<TInfo>;
                original?.AdditionalInstances.Add(item);
            }
            mToBeAdded.Clear();
        }


        /// <summary>
        /// Gets hash code of the given item. If item implements <see cref="IMultipleInstance{Tinfo}" />, returns hash code
        /// obtained from <see cref="IMultipleInstance{Tinfo}.GetIdentifierHashCode" />.
        /// </summary>
        /// <param name="item">Item the hash code should be obtained from</param>
        /// <param name="hashCode"></param>
        /// <returns>
        /// True, if item was implementing <see cref="IMultipleInstance{Tinfo}" /> and
        /// <see cref="IMultipleInstance{TInfo}.GetIdentifierHashCode" /> was used for obtaining the hash. Otherwise; false
        /// </returns>
        private bool TryGetIdentifierHashCode(TInfo item, out int hashCode)
        {
            hashCode = item.GetHashCode();
            var multipleInstanceObject = item as IMultipleInstance<TInfo>;
            if (multipleInstanceObject != null)
            {
                hashCode = multipleInstanceObject.GetIdentifierHashCode();
                return true;
            }

            return false;
        }
    }
}