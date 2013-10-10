using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    internal class CollectionMemento<T> : ICompositeMemento
    {
        private ICollection<T> _collection;

        public IEnumerable<ICompositeMemento> Children { get; set; }

        public CollectionMemento(ICollection<T> collection)
        {
            _collection = collection;
        }

        public void Restore()
        {

        }
    }
}
