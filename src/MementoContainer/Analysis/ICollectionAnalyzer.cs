using System;
using System.Collections;
using System.Collections.Generic;
using MementoContainer.Exceptions;

namespace MementoContainer.Analysis
{
    /// <summary>
    /// Analyzes objects and looks for collections to be registered in the memento.
    /// </summary>
    internal interface ICollectionAnalyzer
    {
        /// <summary>
        /// Retrieve an object's collections.
        /// If the object's type has the MementoClass attribute defined, all properties whose type implements <see cref="ICollection{T}"/> will be retrieved.
        /// Otherwise, only collections with the MementoCollection attribute will be returned.
        /// </summary>
        /// 
        /// <exception cref="CollectionException">
        /// All properties that have the <see cref="MementoCollectionAttribute"/> defined must implement <see cref="ICollection{T}"/>.
        /// </exception>
        /// 
        /// <param name="obj">The object whose collections will be returned.</param>
        /// <returns>A set of collections as objects.</returns>
        IEnumerable<Tuple<object, bool>> GetCollections(object obj);
    }
}
