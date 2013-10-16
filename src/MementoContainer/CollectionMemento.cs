using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Factories;
using MementoContainer.Utils;

namespace MementoContainer
{
    internal class CollectionMemento : ICompositeMemento
    {
        private readonly dynamic _collection;
        private readonly Array _copy;
        private readonly IMementoFactory _factory;

        private readonly Type _collectionType;

        public IEnumerable<ICompositeMemento> Children { get; set; }

        public CollectionMemento(object collection, IMementoFactory factory)
        {
            _factory = factory;
            _collection = new DynamicWrapper(collection);
            _collectionType = collection.GetType();

            //initialize array
            var collectionCount = _collection.Count;
            var genericTypeArgument = _collectionType
                .FindGenericInterface(typeof (ICollection<>))
                .GenericTypeArguments[0];

            _copy = Array.CreateInstance(genericTypeArgument, collectionCount);
            
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
            if (_collectionType.ImplementsGeneric(typeof (List<>)))
            {
                _collection.AddRange(_copy);
            }
            else if (_collectionType.ImplementsGeneric(typeof (HashSet<>)) &&
                     _collectionType.ImplementsGeneric(typeof (SortedSet<>)))
            {
                _collection.UnionWith(_copy);
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
            Children = _copy.Cast<object>()
                            .Where(o => o != null)
                            .SelectMany(obj => _factory.CreateMementos(obj))
                            .ToList();
        }
    }
}
