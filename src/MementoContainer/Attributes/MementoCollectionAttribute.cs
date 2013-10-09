using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Attributes
{
    /// <summary>
    /// Specifies that this collection should be recorded within the memento container.
    /// After <see cref="IMemento.Restore"/> is called, the collection will contain the same elements as it originally did.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MementoCollectionAttribute : Attribute
    {
    }
}
