using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Adapters
{
    /// <summary>
    /// Wraps System.Reflection.PropertyInfo to make the system more testable.
    /// </summary>
    internal class PropertyInfoAdapter : IPropertyAdapter
    {
        private readonly PropertyInfo _property;

        public PropertyInfoAdapter(PropertyInfo property)
        {
            _property = property;
        }

        public bool IsStatic
        {
            get
            {
                return (_property.CanRead && _property.GetMethod.IsStatic) ||
                       (_property.CanWrite && _property.SetMethod.IsStatic);
            }
        }

        public object GetValue(object owner)
        {
            return _property.GetValue(owner);
        }

        public void SetValue(object owner, object value)
        {
            _property.SetValue(owner, value);
        }

        public override string ToString()
        {
            return _property.ToString();
        }


        #region Equality Operators
        public static bool operator ==(PropertyInfoAdapter a, PropertyInfoAdapter b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a._property == b._property;
        }

        public static bool operator !=(PropertyInfoAdapter a, PropertyInfoAdapter b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var adapter = obj as PropertyInfoAdapter;
            if (adapter == null)
                return false;

            return _property == adapter._property;
        }

        protected bool Equals(PropertyInfoAdapter other)
        {
            return Equals(_property, other._property);
        }

        public override int GetHashCode()
        {
            return (_property != null ? _property.GetHashCode() : 0);
        }
        #endregion
    }
}
