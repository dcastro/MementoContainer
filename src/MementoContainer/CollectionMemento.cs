using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Domain;
using MementoContainer.Factories;
using MementoContainer.Utils;

namespace MementoContainer
{
    internal class CollectionMemento : ICompositeMemento
    {
        private readonly dynamic _collection;
        private readonly Array _copy;
        private readonly IMementoFactory _factory;
        private readonly bool _cascade;

        private readonly Type _collectionType;
        private readonly Type _collectionItemsType;

        public IEnumerable<ICompositeMemento> Children { get; set; }

        public CollectionMemento(ICollectionData data, IMementoFactory factory)
            : this(data.Collection, data.Cascade, factory)
        {
            
        }

        public CollectionMemento(object collection, bool cascade, IMementoFactory factory)
        {
            _factory = factory;
            _collection = new DynamicInvoker(collection);
            _collectionType = collection.GetType();
            _cascade = cascade;

            //initialize array
            var collectionCount = _collection.Count;
            _collectionItemsType = _collectionType
                .FindGenericInterface(typeof (ICollection<>))
                .GenericTypeArguments[0];

            _copy = Array.CreateInstance(_collectionItemsType, collectionCount);
            SaveState();

            Children = new List<ICompositeMemento>();

            if (_cascade)
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
            //if this is a collection of collections
            if (_collectionItemsType.IsCollection())
            {
                //create collection mementos for each item
                Children = _copy.Cast<object>()
                                .Where(o => o != null)
                                .Select(obj => _factory.CreateCollectionMemento(obj, _cascade))
                                .ToList();
            }
            else //otherwise, create mementos for each item
            {
                Children = _copy.Cast<object>()
                                .Where(o => o != null)
                                .SelectMany(obj => _factory.CreateMementos(obj))
                                .ToList();
            }
        }
    }
}
