﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Domain;
using MementoContainer.Utils;

namespace MementoContainer
{
    /// <summary>
    /// Represents a chain of properties.
    /// E.g., magazine => magazine.FrontPage.Photo.Description
    /// In this example, 'magazine' is the owner, 'FrontPage' and 'Photo' are the links and 'Description' is the target - the property being recorded.
    /// </summary>
    internal class PropertyChainMemento : IPropertyMemento
    {
        public object Owner { get; set; }
        public IPropertyAdapter Property { get; set; }
        public object SavedValue { get; set; }

        protected IList<IPropertyAdapter> Links { get; set; }

        public PropertyChainMemento(object owner, IList<IPropertyAdapter> props)
        {
            Property = props.Last();
            Links = props.Take(props.Count - 1).ToList();

            Owner = Property.IsStatic ? null : owner;

            SavedValue = GetValue();
        }

        /// <summary>
        /// Returns the property's value.
        /// </summary>
        /// <returns></returns>
        internal object GetValue()
        {
            object lastOwner = ResolveLinks();
            return Property.GetValue(lastOwner);
        }

        /// <summary>
        /// Loops through the links, feeding each link's value to the next link as its owner,
        /// and returns the last link's value (i.e., Property's current owner)
        /// </summary>
        /// <returns></returns>
        protected object ResolveLinks()
        {
            return Links.Aggregate(Owner, (currentOwner, prop) => prop.GetValue(currentOwner));
        }

        public void Rollback()
        {
            object lastOwner = ResolveLinks();
            Property.SetValue(lastOwner, SavedValue);
        }
    }
}
