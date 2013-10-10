using System;
using System.Collections;
using System.Collections.Generic;

namespace MementoContainer.Analysis
{
    /// <summary>
    /// Analyzes objects and looks for collections to be registered in the memento.
    /// </summary>
    internal interface ICollectionAnalyzer
    {
        IEnumerable<object> GetCollections(object obj);
    }
}
