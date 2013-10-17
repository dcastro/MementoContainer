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

            var collectionAttr = ((MementoCollectionAttribute) attrs.First(attr => attr is MementoCollectionAttribute));
            Cascade = collectionAttr.Cascade;
        }

        public CollectionData(object collection, Attribute mementoClassAttr)
        {
            Collection = collection;
            Cascade = ((MementoClassAttribute) mementoClassAttr).Cascade;
        }
    }
}
