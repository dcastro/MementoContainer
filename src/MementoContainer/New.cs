using System;
using System.Linq.Expressions;

namespace MementoContainer
{
    /// <summary>
    /// Helper class to initialize objects with empty constructors with better performance than Activator.CreateInstance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class New<T> where T : new()
    {
        /// <summary>
        /// Creates new instance of type T
        /// </summary>
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>> (Expression.New(typeof(T))).Compile();
    }
}
