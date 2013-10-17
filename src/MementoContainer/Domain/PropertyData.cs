using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;

namespace MementoContainer.Domain
{
    internal class PropertyData : IPropertyData
    {
        public IPropertyAdapter PropertyAdapter { get; private set; }
        public object Owner { get; private set; }
        public bool Cascade { get; private set; }

        public PropertyData(PropertyInfo property, IEnumerable<Attribute> attrs, object owner) : this(property, owner)
        {
            var propertyAttr = ((MementoPropertyAttribute)attrs.First(attr => attr is MementoPropertyAttribute));
            Cascade = propertyAttr.Cascade;
        }

        public PropertyData(PropertyInfo property, Attribute mementoClassAttr, object owner) : this(property, owner)
        {
            Cascade = ((MementoClassAttribute)mementoClassAttr).Cascade;
        }

        private PropertyData(PropertyInfo property, object owner)
        {
            PropertyAdapter = new PropertyInfoAdapter(property);
            Owner = owner;
        }
    }
}
