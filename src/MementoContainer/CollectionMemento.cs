using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Factories;

namespace MementoContainer
{
    internal class CollectionMemento<T> : ICompositeMemento
    {
        private readonly ICollection<T> _collection;
        private readonly T[] _copy;
        private readonly IMementoFactory _factory;

        public IEnumerable<ICompositeMemento> Children { get; set; }

        public CollectionMemento(ICollection<T> collection, IMementoFactory factory)
        {
            _collection = collection;
            _copy = new T[_collection.Count];
            _factory = factory;

            SaveState();
            GenerateChildren();
        }

        private void SaveState()
        {
            _collection.CopyTo(_copy, 0);
        }

        public void Restore()
        {
            _collection.Clear();

            //use optimized "bulk insertion" if available
            if (_collection is List<T>)
            {
                var list = _collection as List<T>;
                list.AddRange(_copy);
            }
            else if (_collection is HashSet<T>)
            {
                var hashSet = _collection as HashSet<T>;
                hashSet.UnionWith(_copy);
            }
            else if (_collection is SortedSet<T>)
            {
                var sortedSet = _collection as SortedSet<T>;
                sortedSet.UnionWith(_copy);
            }
            else
            {
                foreach (var element in _copy)
                {
                    _collection.Add(element);
                }
            }

            //Restore children
            foreach (var child in Children)
            {
                child.Restore();
            }
        }

        /// <summary>
        /// Generates instances of ICompositeMemento for each property that belongs to this property's value.
        /// </summary>
        protected void GenerateChildren()
        {
            Children = _copy.SelectMany(obj => _factory.CreateMementos(obj))
                .ToList();
        }
    }
}
