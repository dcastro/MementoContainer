using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    /// <summary>
    /// Provides a set of methods to validate method parameters.
    /// </summary>
    internal static class Method
    {
        /// <summary>
        /// Throws an exception of the specified type if the condition is not met.
        /// </summary>
        /// <typeparam name="T">The type of the exception to be thrown.</typeparam>
        /// <param name="condition">The condition that should be met.</param>
        public static void Requires<T>(bool condition) where T : Exception, new()
        {
            if (!condition)
                throw new T();
        }
    }
}
