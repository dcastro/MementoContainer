using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Common base class for all MementoContainer exceptions
    /// </summary>
    public abstract class MementoException : Exception
    {
        internal MementoException()
        {
        }

        internal MementoException(string msg)
            : base(msg)
        {

        }

        internal MementoException(string msg, Exception innerException)
            : base(msg, innerException)
        {

        }
    }
}
