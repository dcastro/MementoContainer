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

        public PropertyData(PropertyInfo property, object owner, IEnumerable<Attribute> attrs, MementoClassAttribute mementoClassAttr)
            : this(property, owner)
        {
            var propertiesAttrs = attrs.OfType<MementoPropertyAttribute>().ToList();

            //If there are any method-level attributes, use those to decide whether 'cascading' should be performed.
            if (propertiesAttrs.Any())
            {
                Cascade = propertiesAttrs.All(a => a.Cascade);
            }
            else if (mementoClassAttr != null)//otherwise, use the class-level attribute
            {
                Cascade = mementoClassAttr.Cascade;
            }
        }

        public PropertyData(PropertyInfo property, object owner, IEnumerable<Attribute> attrs)
            : this(property, owner, attrs, null)
        {

        }


        private PropertyData(PropertyInfo property, object owner)
        {
            PropertyAdapter = new PropertyInfoAdapter(property);
            Owner = owner;
        }
    }
}
