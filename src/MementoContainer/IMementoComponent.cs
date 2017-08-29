using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MementoContainer
{
    /// <summary>
    /// Represents a component responsible for recording the state of an object/property
    /// and restoring that state on demand.
    /// </summary>
    public interface IMementoComponent
    {
        /// <summary>
        /// Restores the component's state to a previously saved state.
        /// </summary>
        void Rollback();
    }
}
