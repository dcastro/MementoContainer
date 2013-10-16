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
    internal class CollectionAnalyzer : BaseAnalyzer, ICollectionAnalyzer
    {
        public IEnumerable<Tuple<object, bool>> GetCollections(object obj)
        {
            Type type = obj.GetType();

            IList<Tuple<object, bool>> collections;

            if (type.IsMementoClass())
            {
                //find all declared collections
                var typeInfo = type.GetTypeInfo();
                var mementoClassAttr = typeInfo.GetCustomAttribute<MementoClassAttribute>();

                collections = typeInfo
                    .DeclaredProperties
                    .Where(p => p.PropertyType.IsCollection())
                    .Select(p => Tuple.Create(p.GetValue(obj), GetCascade(mementoClassAttr)))
                    .Where(pair => pair.Item1 != null)
                    .ToList();
            }
            else
            {
                //find all properties with the MementoCollection attribute
                var attributesMap = type.GetFullAttributesMap();

                collections = attributesMap
                    .Where(kv => kv.Value.Any(attr => attr is MementoCollectionAttribute))
                    .Select(kv => kv.Key)
                    .Select(ValidateCollection)
                    .Select(prop => Tuple.Create(prop.GetValue(obj), GetCascade(attributesMap[prop])))
                    .Where(pair => pair.Item1 != null)
                    .ToList();
            }

            return collections;
        }

        private bool GetCascade(IEnumerable<Attribute> attrs)
        {
            var collectionAttr = ((MementoCollectionAttribute) attrs.First(attr => attr is MementoCollectionAttribute));
            return collectionAttr.Cascade;
        }

        private PropertyInfo ValidateCollection(PropertyInfo property)
        {
            if (!property.PropertyType.IsCollection())
                throw CollectionException.IsNotCollection(property);

            return property;
        }
    }
}
