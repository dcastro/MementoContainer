using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
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
                                                    .Select(p => new PropertyMemento(owner, true, p, this));

            var collectionMementos =
                _collectionAnalyzer.GetCollections(owner)
                                   .Select(o => CreateCollectionMemento((dynamic) o))
                                   .Cast<ICompositeMemento>();

            return propertyMementos.Union(collectionMementos).ToList();
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

        private ICompositeMemento CreateCollectionMemento<T>(ICollection<T> collection)
        {
            return new CollectionMemento<T>(collection);
        }
    }
}
