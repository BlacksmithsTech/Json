using System;
using System.Collections.Generic;
using System.Text;

namespace Blacksmiths.Text.Json.Serialization
{
    /// <summary>
    /// Defines a type mapping between a base class and a child class. Apply this attribute to the base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DiscriminatorTypeMappingAttribute : Attribute
    {
        /// <summary>
        /// The discriminator key
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// The child type to serialize/deserialize
        /// </summary>
        public Type ChildType { get; }

        /// <summary>
        /// Defines a type mapping between a base class and a child class. Apply this attribute to the base class.
        /// </summary>
        /// <param name="key">The discriminator key (the value within the discriminator property) which maps to a type</param>
        /// <param name="childType">The child type to deserialize when the discrimator is this key</param>
        public DiscriminatorTypeMappingAttribute(object key, Type childType)
        {
            this.Key = key;
            this.ChildType = childType;
        }
    }
}
