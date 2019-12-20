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
        public delegate bool CollectionFilter(MementoClassAttribute mementoClassAttr, PropertyInfo pi, IList<Attribute> propertyAttrs);
        private static readonly CollectionFilter DefaultFilter = (mementoClassAttr, pi, propertyAttrs) =>
                mementoClassAttr != null ?
                        propertyAttrs.Any(attr => attr is MementoCollectionAttribute) && pi.PropertyType.IsCollection()
                        : propertyAttrs.Any(attr => attr is MementoCollectionAttribute);

        CollectionFilter Filter { get; }

        /// <summary>
        /// Initializes CollectionAnalyzer with given filter
        /// </summary>
        /// <param name="filter">Property selection filter</param>
        public CollectionAnalyzer(CollectionFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Initializes CollectionAnalyzer with default filter
        /// </summary>
        public CollectionAnalyzer() : this(DefaultFilter)
        {

        }

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
            MementoClassAttribute mementoClassAttr = type.GetTypeInfo().GetCustomAttribute<MementoClassAttribute>();
            IDictionary<PropertyInfo, IList<Attribute>> attributesMap = type.GetFullAttributesMap();

            return attributesMap
                .Where(kv => Filter(mementoClassAttr, kv.Key, kv.Value))
                .Select(kv => new CollectionData(kv.Key.GetValue(obj), kv.Key.PropertyType, attributesMap[kv.Key], mementoClassAttr))
                .Where(data => data.Collection != null)
                .Cast<ICollectionData>()
                .ToList();
        }
    }
}
