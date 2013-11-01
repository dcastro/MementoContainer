using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Adapters
{
    /// <summary>
    /// Wraps a <see cref="IProducerConsumerCollection{T}"/> instance, allowing the memento container to read/restore its state.
    /// As follows, this adapter can be used for <see cref="ConcurrentStack{T}"/>, <see cref="ConcurrentQueue{T}"/> and <see cref="ConcurrentBag{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the colection.</typeparam>
    public class ProducerConsumerCollectionAdapter<T> : ICollectionAdapter<IProducerConsumerCollection<T>, T>
    {
        /// <summary>
        /// Gets or sets the collection being adapted.
        /// </summary>
        public IProducerConsumerCollection<T> Collection { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IProducerConsumerCollection{T}"/>.
        /// </summary>
        public int Count
        {
            get { return Collection.Count; }
        }

        /// <summary>
        /// Removes all items from the <see cref="IProducerConsumerCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            //Try optimized 'Clear' implementation if available
            if (Collection is ConcurrentStack<T>)
            {
                var stack = Collection as ConcurrentStack<T>;
                stack.Clear();
            }
            else
            {
                //Use the default approach - take one item at a time
                T ignored;
                while (Collection.TryTake(out ignored)) ;
            }
        }

        /// <summary>
        /// Adds a set of items to the <see cref="IProducerConsumerCollection{T}"/>.
        /// </summary>
        /// <param name="items">The items that should be added to the <see cref="IProducerConsumerCollection{T}"/>.</param>
        public void AddRange(IEnumerable<T> items)
        {
            //Try optimized 'AddRange' implementation if available
            if (Collection is ConcurrentStack<T>)
            {
                var stack = Collection as ConcurrentStack<T>;
                stack.PushRange(items.ToArray());
            }
            else
            {
                //Use the default approach - add one item at a time
                foreach (var item in items)
                {
                    Collection.TryAdd(item);
                }
            }
        }

        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="IProducerConsumerCollection{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }
    }
}
