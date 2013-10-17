using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Domain
{
    internal class CollectionData : ICollectionData
    {
        public object Collection { get; private set; }
        public bool Cascade { get; private set; }

        public CollectionData(object collection, IEnumerable<Attribute> attrs)
        {
            Collection = collection;

            var collectionAttrs = attrs.OfType<MementoCollectionAttribute>();

            Cascade = collectionAttrs.All(a => a.Cascade);
        }

        public CollectionData(object collection, MementoClassAttribute attr)
        {
            Collection = collection;
            Cascade = attr.Cascade;
        }
    }
}
