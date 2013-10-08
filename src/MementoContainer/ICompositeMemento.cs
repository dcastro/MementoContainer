using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Represents a property component that might be also composed by several other components.
    /// When an object is registered, ICompositeMemento is used to build a tree of properties.
    /// <para></para>
    /// E.g., if this ICompositeMemento records the state of <code>article.Photo</code> (where article is the owner and Photo is the property),
    /// ICompositeMemento might also record Photo's properties (like Size, Description, Filename).
    /// </summary>
    internal interface ICompositeMemento : IMementoComponent
    {
        /// <summary>
        /// Collection of properties related to this property's value.
        /// </summary>
        IEnumerable<ICompositeMemento> Children { get; set; }
    }
}
