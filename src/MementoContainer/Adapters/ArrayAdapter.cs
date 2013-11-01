using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Adapters
{
    /// <summary>
    /// Wraps an array, allowing the memento container to read/restore its state.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    internal class ArrayAdapter<T> : ICollectionAdapter<T[], T>
    {
        /// <summary>
        /// Gets or sets the collection being adapted.
        /// </summary>
        public T[] Collection { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the array.
        /// </summary>
        public int Count
        {
            get { return Collection.Length; }
        }

        /// <summary>
        /// No-op implementation - the AddRange method will simply replace each element in the array with its original value.
        /// </summary>
        public void Clear()
        {
            //no-op - the AddRange method will simply replace each element in the array with its original value
        }
        
        /// <summary>
        /// Adds a set of items to the array.
        /// </summary>
        /// <param name="items">The items that should be added to the array.</param>
        public void AddRange(IEnumerable<T> items)
        {
            int index = 0;
            foreach (var item in items)
            {
                Collection[index++] = item;
            }
        }

        /// <summary>
        /// Copies the elements of this array to another System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from this array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }
    }
}
