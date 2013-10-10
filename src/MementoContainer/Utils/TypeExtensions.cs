using System;
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
        public static IDictionary<PropertyInfo, IList<Type>> GetAttributesMap(this Type type)
        {
            var properties = type.GetRuntimeProperties().ToList();
            var interfaceProperties = type.GetTypeInfo()
                                          .ImplementedInterfaces
                                          .SelectMany(@interface => @interface.GetRuntimeProperties())
                                          .ToList();

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

        private static IEnumerable<Type> GetAttributeTypes(this PropertyInfo property)
        {
            if (property == null)
                return new List<Type>();

            return property.CustomAttributes
                           .Select(a => a.AttributeType);
        }
    }
}
