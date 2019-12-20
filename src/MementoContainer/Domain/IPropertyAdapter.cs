﻿namespace MementoContainer.Domain
{
    /// <summary>
    /// Represents an instance or static property
    /// </summary>
    public interface IPropertyAdapter
    {
        /// <summary>
        /// Returns true if this property is static.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="owner">The object whose property value will be returned.</param>
        /// <returns>The property value of the specified object.</returns>
        object GetValue(object owner);

        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <param name="owner">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        void SetValue(object owner, object value);
    }
}
