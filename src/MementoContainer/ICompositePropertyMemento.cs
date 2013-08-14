using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Represents a property component that might be also composed by several other property components.
    /// When an object is registered, ICompositePropertyMemento is used to build a tree of properties.
    /// <para></para>
    /// E.g., if this ICompositePropertyMemento records the state of <code>article.Photo</code> (where article is the owner and Photo is the property),
    /// ICompositePropertyMemento might also record Photo's properties (like Size, Description, Filename).
    /// </summary>
    internal interface ICompositePropertyMemento : IPropertyMemento
    {
        /// <summary>
        /// Collection of properties related to this property's value.
        /// </summary>
        IEnumerable<ICompositePropertyMemento> Children { get; set; }
    }
}
