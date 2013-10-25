using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Analysis;
using MementoContainer.Domain;
using MementoContainer.Utils;

namespace MementoContainer.Factories
{
    internal class MementoFactory : IMementoFactory
    {
        //dependencies
        private readonly IPropertyAnalyzer _propertyAnalyzer;
        private readonly ICollectionAnalyzer _collectionAnalyzer;

        public MementoFactory(IPropertyAnalyzer propertyAnalyzer, ICollectionAnalyzer collectionAnalyzer)
        {
            _propertyAnalyzer = propertyAnalyzer;
            _collectionAnalyzer = collectionAnalyzer;
        }

        public IEnumerable<ICompositeMemento> CreateMementos(object owner)
        {
            var propertyMementos = _propertyAnalyzer.GetProperties(owner)
                                                    .Select(data => new PropertyMemento(data, this))
                                                    .ToList();

            var collectionMementos =
                _collectionAnalyzer.GetCollections(owner)
                                   .Select(data => new CollectionMemento(data, this))
                                   .Cast<ICompositeMemento>()
                                   .ToList();

            return propertyMementos.Concat(collectionMementos).ToList();
        }

        public ICompositeMemento CreateCollectionMemento<TCollection, TElement>(
            ICollectionAdapter<TCollection, TElement> adapter,
            bool cascade)
        {
            var data = new CollectionData(adapter, cascade, typeof(TElement));
            return new CollectionMemento(data, this);
        }

        public ICompositeMemento CreateCollectionMemento(object collection, bool cascade)
        {
            var data = new CollectionData(collection, cascade);
            return new CollectionMemento(data, this);
        }

        public IMementoComponent CreateMemento<TOwner, TProp>(TOwner owner, Expression<Func<TOwner, TProp>> propertyExpression)
        {
            var props = _propertyAnalyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(owner, false, props.First(), this);
            return new DeepPropertyMemento(owner, props);
        }

        public IMementoComponent CreateMemento<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            var props = _propertyAnalyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(null, false, props.First(), this);
            return new DeepPropertyMemento(null, props);
        }
    }
}
