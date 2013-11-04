using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Thrown when a property that doesn't declare both get or set accessors is supplied.
    /// </summary>
    public class PropertyException : MementoException
    {
        private PropertyException(string msg)
            : base(msg)
        {
        }

        internal static PropertyException MissingAccessors(PropertyInfo property)
        {
            string message = string.Format("Property '{0}' must declare get and set accessors.", property.Name);
            return new PropertyException(message);
        }

        internal static PropertyException MissingGetAccessor(PropertyInfo property)
        {
            string message = string.Format("Property '{0}' must declare a get accessor.", property.Name);
            return new PropertyException(message);
        }

        internal static PropertyException MissingSetAccessor(PropertyInfo property)
        {
            string message = string.Format("Property '{0}' must declare a set accessor.", property.Name);
            return new PropertyException(message);
        }
    }
}
