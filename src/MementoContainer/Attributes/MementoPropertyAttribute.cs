using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Specifies that this property should be recorded within the memento container
    /// whenever an object of the containing type is registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MementoPropertyAttribute : Attribute
    {
        internal bool Cascade { get; private set; }

        /// <summary>
        /// Specifies that this property should be recorded within the memento container
        /// whenever an object of the containing type is registered.
        /// After <see cref="IMemento.Rollback"/> is called, this property will be set to its original value.
        /// </summary>
        /// <param name="cascade">Specifies whether this property' value should be added to the container as well.</param>
        public MementoPropertyAttribute(bool cascade = true)
        {
            Cascade = cascade;
        }
    }
}
