using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using MementoContainer.Factories;
using MementoContainer.Utils;

namespace MementoContainer
{
    /// <summary>
    /// Records the state of a property and (optionally) the state of its children properties.
    /// <para></para>
    /// E.g., if this PropertyMemento records the state of <code>article.Photo</code> (where article is the owner and Photo is the property),
    /// PropertyMemento might also record Photo's properties (like Size, Description, Filename).
    /// </summary>
    internal class PropertyMemento : ICompositeMemento, IPropertyMemento
    {
        public IEnumerable<ICompositeMemento> Children { get; set; }
        
        public object Owner { get; set; }
        public IPropertyAdapter Property { get; set; }
        public object SavedValue { get; set; }

        //dependencies
        private IMementoFactory Factory { get; set; }

        internal PropertyMemento(object owner, bool generateChildren, IPropertyAdapter prop, IMementoFactory factory)
        {
            Property = prop;
            Owner = Property.IsStatic ? null : owner;

            SavedValue = Property.GetValue(owner);

            Factory = factory;
            Children = new List<ICompositeMemento>();

            if(generateChildren)
                GenerateChildren();
        }

        public void Restore()
        {
            Property.SetValue(Owner, SavedValue);
            foreach (var child in Children)
            {
                child.Restore();
            }
        }

        /// <summary>
        /// Generates instances of ICompositeMemento for each property that belongs to this property's value.
        /// </summary>
        protected void GenerateChildren()
        {
            //if this property has a value, generate mementos for the value's properties
            if (SavedValue != null)
            {
                Children = Factory.CreateMementos(SavedValue);
            }
        }
    }
}
