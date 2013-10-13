using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using MementoContainer.Exceptions;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    internal class CollectionAnalyzer : ICollectionAnalyzer
    {
        public IEnumerable<object> GetCollections(object obj)
        {
            Type type = obj.GetType();

            if (type.IsMementoClass())
            {
                //find all declared collections
                return type
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(p => p.PropertyType.ImplementsGeneric(typeof(ICollection<>)))
                    .Select(p => p.GetValue(obj))
                    .Where(o => o != null)
                    .ToList();
            }

            //find all properties with the MementoCollection attribute
            return type
                .GetFullAttributesMap()
                .Where(kv => kv.Value.Contains(typeof (MementoCollectionAttribute)))
                .Select(kv => kv.Key)
                .Select(ValidateCollection)
                .Select(property => property.GetValue(obj))
                .Where(o => o != null)
                .ToList();
        }

        private PropertyInfo ValidateCollection(PropertyInfo property)
        {
            if (!property.PropertyType.ImplementsGeneric(typeof(ICollection<>)))
                throw CollectionException.IsNotCollection(property);
            return property;
        }
    }
}
