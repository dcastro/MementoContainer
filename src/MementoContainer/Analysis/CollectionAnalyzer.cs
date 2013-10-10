using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    internal class CollectionAnalyzer : ICollectionAnalyzer
    {
        public IEnumerable<object> GetCollections(object obj)
        {
            //TODO: validate object type

            Type type = obj.GetType();

            if (type.IsMementoClass())
            {
                // TODO:
            }

            return obj.GetType()
                      .GetFullAttributesMap()
                      .Where(kv => kv.Value.Contains(typeof (MementoCollectionAttribute)))
                      .Select(kv => kv.Key)
                      .Select(property => property.GetValue(obj))
                      .ToList();
        }
    }
}
