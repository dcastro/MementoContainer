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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MementoClassAttribute : Attribute
    {
    }
}
