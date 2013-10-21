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
            string message = string.Format("Property of type '{0}' must implement '{1}'. " +
                                           "Alternatively, provide an ICollectionAdapter through the MementoCollectionAttribute overloaded constructor.",
                                           type,
                                           typeof(ICollection<>));
            return new CollectionException(message);
        }

        internal static CollectionException InvalidAdapterType(Type type)
        {
            string message = string.Format("Collection adapter of type '{0}' does not implement '{1}'",
                                           type,
                                           typeof(ICollectionAdapter<,>));
            return new CollectionException(message);
        }

        internal static CollectionException FailedAdapterActivation(Type type, Exception innerException)
        {
            string message = string.Format("Failed to instantiate collection adapter of type '{0}'",
                                           type);
            return new CollectionException(message, innerException);
        }

        internal static CollectionException AdapterTypeMismatch(Type adapterType, /* e.g., StackAdapter */
                                                                Type boundGenericAdapterType, /* e.g, ICollectionAdapter<Stack<string>, string> */
                                                                Type collectionType) /* e.g., Stack<int> */
        {
            string message = string.Format("Collection adapter of type '{0}' (which implements '{1}') cannot be used with collections of type '{2}'.",
                                           adapterType,
                                           boundGenericAdapterType,
                                           collectionType);
            return new CollectionException(message);
        }
    }
}
