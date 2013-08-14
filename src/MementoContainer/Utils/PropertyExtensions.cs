using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    /// <summary>
    /// A set of extension methods for <see cref="System.Reflection.PropertyInfo"/>
    /// </summary>
    internal static class PropertyExtensions
    {
        /// <summary>
        /// Determines whether this property declares both get and set accessors.
        /// </summary>
        /// <param name="prop">The property being checked.</param>
        /// <returns>A boolean stating whether this property declares both get and set accessors.</returns>
        internal static bool HasGetAndSet(this PropertyInfo prop)
        {
            return prop.CanRead && prop.CanWrite;
        } 
    }
}
