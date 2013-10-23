using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Domain
{
    internal class PropertyData : IPropertyData
    {
        public IPropertyAdapter PropertyAdapter { get; private set; }
        public object Owner { get; private set; }
        public bool Cascade { get; private set; }

        public PropertyData(PropertyInfo property, IEnumerable<Attribute> attrs, object owner)
            : this(property, owner)
        {
            var propertiesAttrs = attrs.OfType<MementoPropertyAttribute>();

            Cascade = propertiesAttrs.All(a => a.Cascade);
        }

        public PropertyData(PropertyInfo property, MementoClassAttribute attr, object owner)
            : this(property, owner)
        {
            Cascade = attr.Cascade;
        }

        private PropertyData(PropertyInfo property, object owner)
        {
            PropertyAdapter = new PropertyInfoAdapter(property);
            Owner = owner;
        }
    }
}
