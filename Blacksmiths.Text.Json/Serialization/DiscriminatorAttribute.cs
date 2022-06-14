using System;
using System.Collections.Generic;
using System.Text;

namespace Blacksmiths.Text.Json.Serialization
{
    /// <summary>
    /// Marks a property as being a discriminator used for polymorphic serialization. A class should only define one such property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DiscriminatorAttribute : Attribute
    {
    }
}
