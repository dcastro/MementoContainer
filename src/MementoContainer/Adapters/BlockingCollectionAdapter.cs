using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Adapters
{
    /// <summary>
    /// Wraps a <see cref="BlockingCollection{T}"/> instance, allowing the memento container to read/restore its state.
    /// If the <see cref="BlockingCollection{T}.CompleteAdding"/> has been called, then the adapter will fail silently and thus, the state won't be restored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockingCollectionAdapter<T> : ICollectionAdapter<BlockingCollection<T>, T>
    {
        /// <summary>
        /// Gets or sets the collection being adapted.
        /// </summary>
        public BlockingCollection<T> Collection { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="BlockingCollection{T}"/>.
        /// </summary>
        public int Count
        {
            get { return Collection.Count; }
        }

        /// <summary>
        /// Removes all items from the <see cref="BlockingCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            //Since we won't be able to add the items back in,
            //turn Clear into a no-op and fail silently.
            if (!Collection.IsAddingCompleted)
            {
                T ignored;
                while (Collection.TryTake(out ignored)) ;
            }
        }

        /// <summary>
        /// Adds a set of items to the <see cref="BlockingCollection{T}"/>.
        /// </summary>
        /// <param name="items">The items that should be added to the <see cref="BlockingCollection{T}"/>.</param>
        public void AddRange(IEnumerable<T> items)
        {
            //Since we can't add the items back in,
            //turn AddRange into a no-op and fail silently.
            if (!Collection.IsAddingCompleted)
            {
                foreach (var item in items)
                {
                    Collection.TryAdd(item);
                }
            }
        }

        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="BlockingCollection{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }
    }
}
