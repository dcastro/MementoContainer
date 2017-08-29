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
    /// <summary>
    /// Default implementation of IMementoFactory, responsible for creating Memento components.
    /// </summary>
    public class MementoFactory : IMementoFactory
    {
        //dependencies
        private readonly IPropertyAnalyzer _propertyAnalyzer;
        private readonly ICollectionAnalyzer _collectionAnalyzer;

        /// <summary>
        /// Creates a new MementoFactory which uses given property analyzer and collection analyzer
        /// </summary>
        /// <param name="propertyAnalyzer">Property analyzer to use</param>
        /// <param name="collectionAnalyzer">Collection analyzer to use</param>
        public MementoFactory(IPropertyAnalyzer propertyAnalyzer, ICollectionAnalyzer collectionAnalyzer)
        {
            _propertyAnalyzer = propertyAnalyzer;
            _collectionAnalyzer = collectionAnalyzer;
        }

        /// <summary>
        /// Creates mementos for a given object
        /// </summary>
        /// <param name="owner">Object to create memento</param>
        /// <returns>Mementos associated to owner</returns>
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

        /// <summary>
        /// Creates mementos for a given collection adapter
        /// </summary>
        /// <typeparam name="TCollection">Collection type</typeparam>
        /// <typeparam name="TElement">Element type</typeparam>
        /// <param name="adapter">Collection adapter to create mementos</param>
        /// <param name="cascade">Indicates if memento should cascade</param>
        /// <returns></returns>
        public ICompositeMemento CreateCollectionMemento<TCollection, TElement>(
            ICollectionAdapter<TCollection, TElement> adapter,
            bool cascade)
        {
            var data = new CollectionData(adapter, cascade, typeof(TElement));
            return new CollectionMemento(data, this);
        }

        /// <summary>
        /// Creates mementos for a given collection
        /// </summary>
        /// <param name="collection">Collection to create mementos</param>
        /// <param name="cascade">Indicates if memento should cascade</param>
        public ICompositeMemento CreateCollectionMemento(object collection, bool cascade)
        {
            var data = new CollectionData(collection, cascade);
            return new CollectionMemento(data, this);
        }

        /// <summary>
        /// Creates mementos for property subset a given object indicated by an experession
        /// </summary>
        /// <typeparam name="TOwner">Owner type</typeparam>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="owner">Object to extract properties</param>
        /// <param name="propertyExpression">Expression to indicate properties</param>
        /// <returns>Memento for the properties</returns>
        public IMementoComponent CreateMemento<TOwner, TProp>(TOwner owner, Expression<Func<TOwner, TProp>> propertyExpression)
        {
            var props = _propertyAnalyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(owner, false, props.First(), this);
            return new PropertyChainMemento(owner, props);
        }

        /// <summary>
        /// Creates mementos for property set indicated by an experession
        /// </summary>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="propertyExpression">Expression to indicate properties</param>
        /// <returns>Memento for the properties</returns>
        public IMementoComponent CreateMemento<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            var props = _propertyAnalyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(null, false, props.First(), this);
            return new PropertyChainMemento(null, props);
        }
    }
}
