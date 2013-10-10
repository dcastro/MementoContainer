﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;

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
        public static IDictionary<PropertyInfo, IList<Type>> GetFullAttributesMap(this Type type)
        {
            var properties = type.GetRuntimeProperties().ToList();

            var interfaceProperties = type.GetTypeInfo()
                                          .ImplementedInterfaces
                                          .SelectMany(@interface => @interface.GetRuntimeProperties())
                                          .ToList();

            //merge each property's attributes with the attributes declared on any of the interfaces.
            return properties.Select(property => new KeyValuePair<PropertyInfo, IList<Type>>(
                                                     property,
                                                     property.GetAttributeTypes().Union(
                                                         interfaceProperties
                                                             .Where(ip => ip.Name == property.Name)
                                                             .SelectMany(ip => ip.GetAttributeTypes()))
                                                             .ToList()))
                             .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static bool IsMementoClass(this Type type)
        {
            return type.GetTypeInfo()
                       .IsDefined(typeof (MementoClassAttribute));
        }

        /// <summary>
        /// Returns the types of the attributes declared for a given property.
        /// </summary>
        /// <param name="property">The property whose attributes' types will be returned.</param>
        /// <returns>A set of attribute types.</returns>
        private static IEnumerable<Type> GetAttributeTypes(this PropertyInfo property)
        {
            if (property == null)
                return new List<Type>();

            return property.CustomAttributes
                           .Select(a => a.AttributeType);
        }
    }
}
