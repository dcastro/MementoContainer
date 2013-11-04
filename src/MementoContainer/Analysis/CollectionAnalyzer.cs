using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Domain;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    internal class CollectionAnalyzer : ICollectionAnalyzer
    {
        public IEnumerable<ICollectionData> GetCollections(object obj)
        {
            Type type = obj.GetType();
            IDictionary<PropertyInfo, IList<Attribute>> attributesMap = type.GetFullAttributesMap();
            IList<ICollectionData> collections;

            if (type.IsMementoClass())
            {
                //find all declared collections + properties with the MementoCollection attribute
                var typeInfo = type.GetTypeInfo();
                var mementoClassAttr = typeInfo.GetCustomAttribute<MementoClassAttribute>();
                
                collections = attributesMap
                    .Where(kv => kv.Value.Any(attr => attr is MementoCollectionAttribute) || kv.Key.PropertyType.IsCollection())
                    .Select(kv => kv.Key)
                    .Select(prop => new CollectionData(prop.GetValue(obj), prop.PropertyType, attributesMap[prop], mementoClassAttr))
                    .Where(data => data.Collection != null)
                    .Cast<ICollectionData>()
                    .ToList();
            }
            else
            {
                //find all properties with the MementoCollection attribute
                collections = attributesMap
                    .Where(kv => kv.Value.Any(attr => attr is MementoCollectionAttribute))
                    .Select(kv => kv.Key)
                    .Select(prop => new CollectionData(prop.GetValue(obj), prop.PropertyType, attributesMap[prop]))
                    .Where(data => data.Collection != null)
                    .Cast<ICollectionData>()
                    .ToList();
            }

            return collections;
        }
    }
}
