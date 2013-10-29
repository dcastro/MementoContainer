using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Specifies that all properties of this class declaring get and set accessors
    /// should be recorded within the memento container
    /// whenever an object of the containing type is registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MementoClassAttribute : Attribute
    {
        internal bool Cascade { get; private set; }

        /// <summary>
        /// Specifies that all properties of this class declaring get and set accessors
        /// should be recorded within the memento container.
        /// After <see cref="IMemento.Rollback"/> is called, these properties will be set to their original values.
        /// </summary>
        /// <param name="cascade">Specifies whether these properties' values should be added to the container as well.</param>
        public MementoClassAttribute(bool cascade = true)
        {
            Cascade = cascade;
        }
    }
}
