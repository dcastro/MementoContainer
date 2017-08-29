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
    /// <summary>
    /// Default implementation of the ICollectionAnalyzer interface to analyze objects and look for collections to be registered in the memento.
    /// </summary>
    public class CollectionAnalyzer : ICollectionAnalyzer
    {
        /// <summary>
        /// Retrieve an object's collections.
        /// If the object's type has the MementoClass attribute defined, all properties whose type implements <see cref="ICollection{T}"/> will be retrieved.
        /// Otherwise, only collections with the MementoCollection attribute will be returned.
        /// </summary>
        /// 
        /// <exception cref="CollectionException">
        /// All properties that have the <see cref="MementoCollectionAttribute"/> defined must implement <see cref="ICollection{T}"/>.
        /// </exception>
        /// 
        /// <param name="obj">The object whose collections will be returned.</param>
        /// <returns>A set of collections.</returns>
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
                    .Where(PropertySelectorForMementoClass)
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
                    .Where(PropertySelectorForNonMementoClass)
                    .Select(kv => kv.Key)
                    .Select(prop => new CollectionData(prop.GetValue(obj), prop.PropertyType, attributesMap[prop]))
                    .Where(data => data.Collection != null)
                    .Cast<ICollectionData>()
                    .ToList();
            }

            return collections;
        }

        /// <summary>
        /// Tells if a given property has to be registered in the memento in case the class is decorated with MementoClassAttribute
        /// </summary>
        /// <param name="kv">Property info and its attributes</param>
        /// <returns>True if the property must be registered in the memento</returns>
        protected virtual bool PropertySelectorForMementoClass(KeyValuePair<PropertyInfo, IList<Attribute>> kv)
        {
            return kv.Value.Any(attr => attr is MementoCollectionAttribute) || kv.Key.PropertyType.IsCollection();
        }

        /// <summary>
        /// Tells if a given property has to be registered in the memento in case the class is not decorated with MementoClassAttribute
        /// </summary>
        /// <param name="kv">Property info and its attributes</param>
        /// <returns>True if the property must be registered in the memento</returns>
        protected virtual bool PropertySelectorForNonMementoClass(KeyValuePair<PropertyInfo, IList<Attribute>> kv)
        {
            return kv.Value.Any(attr => attr is MementoCollectionAttribute);
        }
    }
}
