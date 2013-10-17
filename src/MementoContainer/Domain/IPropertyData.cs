using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;

namespace MementoContainer.Domain
{
    /// <summary>
    /// Represents data related to a property to be registered in the memento container.
    /// </summary>
    internal interface IPropertyData
    {
        /// <summary>
        /// Represents the property.
        /// </summary>
        IPropertyAdapter PropertyAdapter { get; }

        /// <summary>
        /// Represents the object this property belongs to.
        /// </summary>
        object Owner { get; }

        /// <summary>
        /// Specifies whether 'cascading' should be performed.
        /// </summary>
        bool Cascade { get; }
    }
}
