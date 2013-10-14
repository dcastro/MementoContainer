using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MementoContainer.Adapters;

namespace MementoContainer.Analysis
{
    /// <summary>
    /// Analyzes certain inputs (e.g., expressions and objects) and looks for properties to be registered in the memento.
    /// </summary>
    internal interface IPropertyAnalyzer
    {
        /// <summary>
        /// Parses all properties contained within an expression that maps an instance of an object to one of its properties.
        /// <para></para>
        /// E.g., the expression <code>article => article.Photo.Size</code> would be broken down into two properties: Photo and Size.
        /// </summary>
        /// 
        /// <exception cref="InvalidExpressionException">
        /// The expression must not contain method calls, variables, fields, closures or any other operation.
        /// </exception>
        /// 
        /// <exception cref="PropertyException">
        /// All properties must declare get and set accessors.
        /// </exception>
        /// 
        /// <typeparam name="TOwner">The type of the function parameter.</typeparam>
        /// <typeparam name="TProperty">The type of the last property in the expression.</typeparam>
        /// <param name="expression">The expression that maps an instance of an object to one of its properties.</param>
        /// <returns>The properties contained within the expression.</returns>
        IList<IPropertyAdapter> GetProperties<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> expression);

        /// <summary>
        /// Parses all properties contained within an expression that returns either <para></para>
        /// (1) a static property or <para></para>
        /// (2) a property of a static property.<para></para>
        /// E.g., the expression <code>() => ArticleContainer.Editor.Name</code> would be broken down into two properties: Editor (static) and Name.
        /// </summary>
        /// 
        /// <exception cref="InvalidExpressionException">
        /// The expression must not contain method calls, variables, fields, closures or any other operation.
        /// </exception>
        /// 
        /// <exception cref="PropertyException">
        /// All properties must declare get and set accessors.
        /// </exception>
        /// 
        /// <typeparam name="TProperty">The type of the last property in the expression.</typeparam>
        /// <param name="expression">The expression that returns either a static property or a property of a static property.</param>
        /// <returns>The properties contained within the expression.</returns>
        IList<IPropertyAdapter> GetProperties<TProperty>(Expression<Func<TProperty>> expression);

        /// <summary>
        /// Retrieve an object's properties.
        /// If the object's type has the MementoClass attribute defined, all properties declaring get and set accessors are retrieved.
        /// Otherwise, only properties with the MementoProperty attribute will be returned.
        /// </summary>
        /// 
        /// <exception cref="PropertyException">
        /// All properties that have the <see cref="MementoPropertyAttribute"/> defined must declare get and set accessors.
        /// </exception>
        /// 
        /// <param name="obj">The object whose properties will be retrieved.</param>
        /// <returns>The object's properties.</returns>
        IList<IPropertyAdapter> GetProperties(object obj);
    }
}
