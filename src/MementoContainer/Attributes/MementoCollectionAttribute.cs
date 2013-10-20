using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Specifies that this collection should be recorded within the memento container.
    /// After <see cref="IMemento.Restore"/> is called, the collection will contain the same elements as it originally did.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MementoCollectionAttribute : Attribute
    {
        internal bool Cascade { get; private set; }
        internal Type CollectionAdapterType { get; private set; }

        /// <summary>
        /// Specifies that this collection should be recorded within the memento container.
        /// After <see cref="IMemento.Restore"/> is called, the collection will contain the same elements as it originally did.
        /// </summary>
        /// <param name="cascade">Specifies whether items in this collection should be added to the container as well.</param>
        public MementoCollectionAttribute(bool cascade = true)
        {
            Cascade = cascade;
        }

        /// <summary>
        /// Specifies that this collection should be recorded within the memento container.
        /// After <see cref="IMemento.Restore"/> is called, the collection will contain the same elements as it originally did.
        /// </summary>
        /// <param name="collectionAdapterType">The Type of a converter to be used for custom collections that don't implement <see cref="ICollection{T}"/>.
        /// This adapter must implement <see cref="ICollectionAdapter{TCollection,TItem}"/></param>
        /// <param name="cascade">Specifies whether items in this collection should be added to the container as well.</param>
        public MementoCollectionAttribute(Type collectionAdapterType, bool cascade = true)
        {
            CollectionAdapterType = collectionAdapterType;
            Cascade = cascade;
        }
    }
}
