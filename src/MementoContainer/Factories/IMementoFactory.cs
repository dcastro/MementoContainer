using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Factories
{
    /// <summary>
    /// IMementoFactory is responsible for creating Memento components.
    /// </summary>
    public interface IMementoFactory
    {
        /// <summary>
        /// Creates memento components for each of the given object "owner"'s properties.
        /// </summary>
        /// <param name="owner">The object whose properties are being registered.</param>
        /// <returns>A set of memento components.</returns>
        IEnumerable<ICompositeMemento> CreateMementos(object owner);

        /// <summary>
        /// Creates a collection memento for the given collection object.
        /// </summary>
        /// <param name="collection">The collection for which the memento will be created</param>
        /// <param name="cascade">Specifies whether items in this collection should be added to the container as well.</param>
        /// <returns>A collection memento.</returns>
        ICompositeMemento CreateCollectionMemento(object collection, bool cascade);

        /// <summary>
        /// Creates a collection memento for the given collection object.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection being registered.</typeparam>
        /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
        /// <param name="adapter">An adapter for a custom collection. Its <see cref="ICollectionAdapter{TCollection,TItem}.Collection"/> property should be set.</param>
        /// <param name="cascade">Specifies whether items in this adapter's collection should be added to the container as well.</param>
        /// <returns>A collection memento.</returns>
        ICompositeMemento CreateCollectionMemento<TCollection, TElement>(
            ICollectionAdapter<TCollection, TElement> adapter,
            bool cascade);

        /// <summary>
        /// Creates a memento component from a given object 'owner' and an expression that maps an instance
        /// of that object to one of its properties.
        /// <para/>
        /// E.g., CreateMemento(article, a => a.Photo.Size);
        /// </summary>
        /// <typeparam name="TOwner">The type of the object whose property is being registered.</typeparam>
        /// <typeparam name="TProp">The type of the property being registered.</typeparam>
        /// <param name="owner">The object whose property is being registered.</param>
        /// <param name="propertyExpression">An expression that maps an instance of the object 'owner' to one of its properties.</param>
        /// <returns>A memento component.</returns>
        IMementoComponent CreateMemento<TOwner, TProp>(TOwner owner, Expression<Func<TOwner, TProp>> propertyExpression);

        /// <summary>
        /// Creates a memento component from a given expression that returns either <para/>
        /// (1) a static property or <para/>
        /// (2) a property of a static property.
        /// <para/>
        /// E.g., CreateMemento(() => ArticleContainer.Name );<para/>
        /// CreateMemento(() => ArticleContainer.Editor.Name);
        /// </summary>
        /// <typeparam name="TProp">The type of the property being registered.</typeparam>
        /// <param name="propertyExpression">An expression that maps to a static property or a property of a static property.</param>
        /// <returns>A memento component.</returns>
        IMementoComponent CreateMemento<TProp>(Expression<Func<TProp>> propertyExpression);
    }
}
