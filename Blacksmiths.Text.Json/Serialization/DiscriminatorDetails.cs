using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Blacksmiths.Text.Json.Serialization
{
    public class DiscriminatorDetails
    {
        public Type Type { get; }

        /// <summary>
        /// Gets the discriminator <see cref="PropertyInfo"/> of the base type to convert
        /// </summary>
        public PropertyInfo DiscriminatorProperty { get; }

        /// <summary>
        /// Mapping of discriminator keys to types
        /// </summary>
        public Dictionary<object, Type> TypeMappings { get; }

        public DiscriminatorDetails(Type type)
        {
            if (null == type)
                throw new ArgumentNullException(nameof(type));

            this.Type = type;

            var discriminatorProperties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttributes<DiscriminatorAttribute>().Any()).ToArray();
            if (discriminatorProperties.Length > 1)
                throw new InvalidOperationException($"Multiple discriminators were specified against type '{this.Type}'. Only one property should be specified as a discriminator");

            if (discriminatorProperties.Length == 1)
            {
                this.DiscriminatorProperty = discriminatorProperties[0];
                this.TypeMappings = this.Type.GetCustomAttributes<DiscriminatorTypeMappingAttribute>().ToDictionary(dtma => dtma.Key, dtma => dtma.ChildType);
                if (this.TypeMappings.Any(kvp => kvp.Value.Equals(this.Type)))
                    throw new JsonException("The target of a discriminator type mapping may not be the base class as this will cause a recursion in deserialization");
            }
        }
    }
}
