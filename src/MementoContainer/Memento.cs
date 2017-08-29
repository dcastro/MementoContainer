using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MementoContainer.Analysis;
using MementoContainer.Factories;
using MementoContainer.Utils;

namespace MementoContainer
{
    /// <summary>
    /// Represents a Memento container, which saves the state of objects' properties
    /// so that they can be later restored to their initial state.
    /// </summary>
    public class Memento : IMemento
    {
        internal IList<IMementoComponent> Components { get; set; }

        //dependencies
        internal IMementoFactory Factory { private get; set; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Memento()
        {
            Components = new List<IMementoComponent>();
            CreateMementoFactory();
        }

        /// <summary>
        /// Creates a new instance of IMemento.
        /// </summary>
        /// <returns>A new instance of IMemento.</returns>
        public static IMemento Create()
        {
            return new Memento();
        }

        /// <summary>
        /// Creates memento factory via DependencyContainer
        /// </summary>
        protected virtual void CreateMementoFactory()
        {
            Factory = DependencyContainer.GetInstanceFor<IMementoFactory>() ?? new MementoFactory(
                DependencyContainer.GetInstanceFor<IPropertyAnalyzer>() ?? new PropertyAnalyzer(),
                DependencyContainer.GetInstanceFor<ICollectionAnalyzer>() ?? new CollectionAnalyzer()
            );
        }

        /// <summary>
        /// Registers an object's property.
        /// Usage: <para/>
        /// <c>RegisterProperty(article, article => article.Photo);</c> <para/>
        /// <c>RegisterProperty(magazine, m => m.FrontPage.Photo.Description);</c>
        /// </summary>
        /// 
        /// <exception cref="InvalidExpressionException">
        /// The expression must not contain method calls, variables, fields, closures or any other operation.
        /// </exception>
        /// 
        /// <exception cref="PropertyException">
        /// All properties in the expression must declare get and set accessors.
        /// </exception>
        /// 
        /// <typeparam name="TOwner">The type of the object whose property is being registered.</typeparam>
        /// <typeparam name="TProp">The type of the property being registered.</typeparam>
        /// <param name="owner">The object whose property is being registered.</param>
        /// <param name="propertyExpression">The expression that maps an instance of an object to one of its properties.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento RegisterProperty<TOwner, TProp>(TOwner owner, Expression<Func<TOwner, TProp>> propertyExpression)
        {
            owner.ThrowIfNull("owner");
            propertyExpression.ThrowIfNull("propertyExpression");

            var memento = Factory.CreateMemento(owner, propertyExpression);
            Components.Add(memento);
            return this;
        }

        /// <summary>
        /// Registers either (1) a static property or (2) a property of a static property.
        /// Usage: <para/>
        /// <c>RegisterProperty(() => ArticleContainer.Editor.Name);</c>
        /// </summary>
        /// 
        /// <exception cref="InvalidExpressionException">
        /// The expression must not contain method calls, variables, fields, closures or any other operation.
        /// </exception>
        /// 
        /// <exception cref="PropertyException">
        /// All properties in the expression must declare get and set accessors.
        /// </exception>
        /// 
        /// <typeparam name="TProp">The type of the property being registered.</typeparam>
        /// <param name="propertyExpression">The expression that returns either a static property or a property of a static property.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento RegisterProperty<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            propertyExpression.ThrowIfNull("propertyExpression");

            var memento = Factory.CreateMemento(propertyExpression);
            Components.Add(memento);
            return this;
        }

        /// <summary>
        /// Registers properties/collections of a given object.
        /// If the object's type has the MementoClass attribute defined, all properties declaring get and set accessors and collections are registered.
        /// Otherwise, only properties/collections with the MementoProperty and/or MementoCollection attributes will be registered.
        /// </summary>
        /// 
        /// <exception cref="PropertyException">
        /// All properties that have the <see cref="MementoPropertyAttribute"/> defined must declare get and set accessors.
        /// </exception>
        /// 
        /// <exception cref="CollectionException">
        /// All properties that have the <see cref="MementoCollectionAttribute"/> defined must either implement <see cref="ICollection{T}"/> 
        /// or provide an <see cref="ICollectionAdapter{TCollection,TItem}"/> through the attribute constructor.
        /// The adapter must be an instantiable type and have a public parameterless constructor.
        /// </exception>
        /// 
        /// <param name="obj">The object whose properties are being registered.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento Register(object obj)
        {
            obj.ThrowIfNull("obj");

            var mementos = Factory.CreateMementos(obj);

            foreach (var memento in mementos)
            {
                Components.Add(memento);
            }

            return this;
        }

        /// <summary>
        /// Registers a collection.
        /// After <see cref="IMemento.Rollback"/> is called, the collection will contain the same elements as at the time of registration.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The collection being registered.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento RegisterCollection<T>(ICollection<T> collection)
        {
            collection.ThrowIfNull("collection");

            var memento = Factory.CreateCollectionMemento(collection, false);

            Components.Add(memento);
            return this;
        }

        /// <summary>
        /// Registers a custom collection though an adapter.
        /// The adapter's 'Collection' property must be set.
        /// After <see cref="IMemento.Rollback"/> is called, the collection will contain the same elements as at the time of registration.
        /// </summary>
        /// 
        /// <typeparam name="TCollection">The type of the collection being registered.</typeparam>
        /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
        /// <param name="adapter">An adapter for a custom collection. Its 'Collection' property should be set.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento RegisterCollection<TCollection, TElement>(ICollectionAdapter<TCollection, TElement> adapter)
        {
            adapter.ThrowIfNull("adapter");
            if(adapter.Collection == null)
                throw new ArgumentException("The adapter's Collection property must not be null.", "adapter");

            var memento = Factory.CreateCollectionMemento(adapter, false);

            Components.Add(memento);
            return this;
        }

        /// <summary>
        /// Restores every registered property to their initially recorded value.
        /// </summary>
        public void Rollback()
        {
            foreach (var memento in Components)
            {
                memento.Rollback();
            }
        }
    }
}
