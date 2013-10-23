using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Analysis;
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

            return propertyMementos.Union(collectionMementos).ToList();
        }

        public ICompositeMemento CreateCollectionMemento(object collection, bool cascade)
        {
            return new CollectionMemento(collection, cascade, this);
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
