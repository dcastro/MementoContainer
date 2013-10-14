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
    }
}
