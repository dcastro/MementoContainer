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

        private CollectionException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        internal static CollectionException IsNotCollection(Type type)
        {
            string message = string.Format("Property of type '{0}' must implement ICollection<T>. " +
                                           "Alternatively, provide an ICollectionAdapter through the MementoCollectionAttribute overloaded constructor.",
                                           type.Name);
            return new CollectionException(message);
        }

        internal static CollectionException InvalidAdapterType(Type type)
        {
            string message = string.Format("Collection adapter of type '{0}' does not implement ICollectionAdapter<T1, T2>",
                                           type.Name);
            return new CollectionException(message);
        }

        internal static CollectionException FailedAdapterActivation(Type type, Exception innerException)
        {
            string message = string.Format("Failed to instantiate collection adapter of type '{0}'",
                                           type.Name);
            return new CollectionException(message, innerException);
        }
    }
}
