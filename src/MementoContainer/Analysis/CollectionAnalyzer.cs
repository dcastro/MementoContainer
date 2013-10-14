using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Exceptions;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    internal class CollectionAnalyzer : ICollectionAnalyzer
    {
        public IEnumerable<object> GetCollections(object obj)
        {
            Type type = obj.GetType();

            IList<object> collections;

            if (type.IsMementoClass())
            {
                //find all declared collections
                collections = type
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(p => p.PropertyType.ImplementsGeneric(typeof (ICollection<>)))
                    .Select(p => p.GetValue(obj))
                    .Where(o => o != null)
                    .ToList();
            }
            else
            {
                //find all properties with the MementoCollection attribute
                collections = type
                    .GetFullAttributesMap()
                    .Where(kv => kv.Value.Contains(typeof (MementoCollectionAttribute)))
                    .Select(kv => kv.Key)
                    .Select(ValidateCollection)
                    .Select(property => property.GetValue(obj))
                    .Where(o => o != null)
                    .ToList();
            }

            //return the object itself, if it is a collection
            if (type.ImplementsGeneric(typeof (ICollection<>)))
                collections.Add(obj);

            return collections;
        }

        private PropertyInfo ValidateCollection(PropertyInfo property)
        {
            if (!property.PropertyType.ImplementsGeneric(typeof(ICollection<>)))
                throw CollectionException.IsNotCollection(property);
            return property;
        }
    }
}
