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
    public class PropertyException : Exception
    {
        internal PropertyException(PropertyInfo prop)
            : base(PrepareMessage(prop))
        {
        }

        private static string PrepareMessage(PropertyInfo prop)
        {
            return string.Format("Property {0} must declare get and set accessors.", prop.Name);
        }
    }
}
