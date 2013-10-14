using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Returns a dictionary that maps a type's properties to the declared attributes.
        /// This method returns attributes declared not only on the concrete type, but also on any of the implemented interfaces.
        /// </summary>
        /// <param name="type">The type used to build the map.</param>
        /// <returns>A dictionary that maps the type's properties to the corresponding attributes.</returns>
        public static IDictionary<PropertyInfo, IList<Attribute>> GetFullAttributesMap(this Type type)
        {
            var properties = type.GetRuntimeProperties().ToList();

            var interfaceProperties = type.GetTypeInfo()
                                          .ImplementedInterfaces
                                          .SelectMany(@interface => @interface.GetRuntimeProperties())
                                          .ToList();

            //merge each property's attributes with the attributes declared on any of the interfaces.
            return properties.Select(property => new KeyValuePair<PropertyInfo, IList<Attribute>>(
                                                     property,
                                                     property.GetCustomAttributes().Union(
                                                         interfaceProperties
                                                             .Where(ip => ip.Name == property.Name)
                                                             .SelectMany(ip => ip.GetCustomAttributes()))
                                                             .ToList()))
                             .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static bool IsMementoClass(this Type type)
        {
            return type.GetTypeInfo()
                       .IsDefined(typeof (MementoClassAttribute));
        }

        /// <summary>
        /// Returns whether this type, any of its base types or any of the implemented interfaces matches
        /// a given generic type (e.g., <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool ImplementsGeneric(this Type type, Type genericType)
        {
            return type.IsGeneric(genericType) ||
                   type.GetTypeInfo()
                       .ImplementedInterfaces
                       .Any(@interface => @interface.IsGeneric(genericType));
        }

        /// <summary>
        /// Returns whether this type or any of its base types matches a given
        /// generic type (e.g., <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        private static bool IsGeneric(this Type type, Type genericType)
        {
            if (type == null)
                return false;

            if (type.GetTypeInfo().IsGenericType &&
                type.GetGenericTypeDefinition() == genericType)
                return true;

            return type.GetTypeInfo().BaseType.IsGeneric(genericType);
        }

        /// <summary>
        /// Returns the interface implemented by this type that matches the given generic type definition.
        /// For example, typeof(List{int}).FindGenericInterface(typeof(ICollection{})) would return ICollection{int}
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static Type FindGenericInterface(this Type type, Type genericType)
        {
            return type.GetTypeInfo()
                       .ImplementedInterfaces
                       .First(@interface => @interface.IsGeneric(genericType));
        }
    }
}
