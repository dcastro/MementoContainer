using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Thrown when a property with the MementoCollection attribute does not implement <see cref="ICollection{T}"/>
    /// </summary>
    public class CollectionException : Exception
    {
        private CollectionException(string msg)
            : base(msg)
        {
        }

        internal static CollectionException IsNotCollection(PropertyInfo property)
        {
            string message = string.Format("Property '{0}' of type '{1}' does not implement ICollection<T>",
                property.Name,
                property.PropertyType.Name);
            return new CollectionException(message);
        }
    }
}
