using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Domain
{
    /// <summary>
    /// Represents data related to a collection to be registered in the memento container.
    /// </summary>
    internal interface ICollectionData
    {
        /// <summary>
        /// Represents a collection.
        /// </summary>
        object Collection { get; }

        /// <summary>
        /// Gets the type of the elements stored in the Collection.
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Specifies whether 'cascading' should be performed.
        /// </summary>
        bool Cascade { get; }
    }
}
