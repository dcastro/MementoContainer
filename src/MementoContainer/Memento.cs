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
            InjectDependencies();
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
        /// Injects the system's dependencies.
        /// </summary>
        internal void InjectDependencies()
        {
            Factory = new MementoFactory(
                new PropertyAnalyzer(),
                new CollectionAnalyzer()
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
            Method.Requires<ArgumentNullException>(owner != null);
            Method.Requires<ArgumentNullException>(propertyExpression != null);

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
            Method.Requires<ArgumentNullException>(propertyExpression != null);

            var memento = Factory.CreateMemento(propertyExpression);
            Components.Add(memento);
            return this;
        }

        /// <summary>
        /// Registers properties of a given object.
        /// If the object's type has the MementoClass attribute defined, all properties declaring get and set accessors are registered.
        /// Otherwise, only properties with the MementoProperty attribute will be registered.
        /// </summary>
        /// 
        /// <exception cref="PropertyException">
        /// All properties being registered must declare get and set accessors.
        /// </exception>
        /// 
        /// <param name="obj">The object whose properties are being registered.</param>
        /// <returns>This IMemento instance.</returns>
        public IMemento Register(object obj)
        {
            Method.Requires<ArgumentNullException>(obj != null);

            var mementos = Factory.CreateMementos(obj);

            foreach (var memento in mementos)
            {
                Components.Add(memento);
            }

            return this;
        }

        /// <summary>
        /// Restores every registered property to their initially recorded value.
        /// </summary>
        public void Restore()
        {
            foreach (var memento in Components)
            {
                memento.Restore();
            }
        }
    }
}
