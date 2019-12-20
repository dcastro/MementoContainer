using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Domain;
using MementoContainer.Utils;

namespace MementoContainer
{
    /// <summary>
    /// Represents a component responsible for recording the state of a static/instance property
    /// and restoring that state on demand.
    /// </summary>
    public interface IPropertyMemento : IMementoComponent
    {
        /// <summary>
        /// Represents the object to which the property belongs.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Represents the property being registered.
        /// </summary>
        IPropertyAdapter Property { get; set; }

        /// <summary>
        /// Represents the recorded state of the property.
        /// </summary>
        object SavedValue { get; set; }
    }
}
