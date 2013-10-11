using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    internal class CollectionMemento<T> : ICompositeMemento
    {
        private readonly ICollection<T> _collection;
        private readonly T[] _copy;

        public IEnumerable<ICompositeMemento> Children { get; set; }

        public CollectionMemento(ICollection<T> collection)
        {
            _collection = collection;
            _copy = new T[_collection.Count];
            SaveState();
        }

        private void SaveState()
        {
            _collection.CopyTo(_copy, 0);
        }

        public void Restore()
        {
            _collection.Clear();

            foreach (var element in _copy)
            {
                _collection.Add(element);
            }
        }
    }
}
