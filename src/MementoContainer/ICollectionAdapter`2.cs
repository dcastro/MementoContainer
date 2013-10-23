using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Wraps a custom collection, allowing the memento container to read/restore the state of a collection that does not implement <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TItem">The type of the elements in the collection.</typeparam>
    public interface ICollectionAdapter<TCollection, in TItem>
    {
        /// <summary>
        /// Gets or sets the collection being adapted.
        /// </summary>
        TCollection Collection { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the <typeparamref name="TCollection"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Removes all items from the <typeparamref name="TCollection"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a set of items to the <typeparamref name="TCollection"/>.
        /// </summary>
        /// <param name="items">The items that should be added to the <typeparamref name="TCollection"/></param>
        void AddRange(IEnumerable<TItem> items);

        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <typeparamref name="TCollection"/>.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void CopyTo(TItem[] array, int arrayIndex);
    }
}
