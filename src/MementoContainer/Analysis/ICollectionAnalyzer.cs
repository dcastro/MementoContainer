using System;
using System.Collections;
using System.Collections.Generic;

namespace MementoContainer.Analysis
{
    internal delegate T CollectionTransformer<out T>(object collection);

    /// <summary>
    /// Analyzes objects and looks for collections to be registered in the memento.
    /// </summary>
    internal interface ICollectionAnalyzer
    {
        IEnumerable<T> GetCollections<T>(object obj, CollectionTransformer<T> transformer);
    }
}
