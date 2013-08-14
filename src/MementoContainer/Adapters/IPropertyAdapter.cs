using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Adapters
{
    /// <summary>
    /// Represents an instance or static property
    /// </summary>
    internal interface IPropertyAdapter
    {
        /// <summary>
        /// Determines whether this property is static.
        /// </summary>
        /// <returns>A boolean stating whether this property is static.</returns>
        bool IsStatic();

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="owner">The object whose property value will be returned.</param>
        /// <returns>The property value of the specified object.</returns>
        object GetValue(object owner);

        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <param name="owner">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        void SetValue(object owner, object value);
    }
}
