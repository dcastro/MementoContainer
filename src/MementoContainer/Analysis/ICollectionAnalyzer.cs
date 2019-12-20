﻿using System;
using System.Collections;
using System.Collections.Generic;
using MementoContainer.Domain;

namespace MementoContainer.Analysis
{
    /// <summary>
    /// Analyzes objects and looks for collections to be registered in the memento.
    /// </summary>
    public interface ICollectionAnalyzer
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
        /// <returns>A set of collections.</returns>
        IEnumerable<ICollectionData> GetCollections(object obj);
    }
}
