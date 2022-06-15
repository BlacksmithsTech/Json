using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Blacksmiths.Text.Json.Serialization
{
    /// <summary>
    /// Provides a concrete implementation from a base class using a discriminator
    /// </summary>
    /// <typeparam name="T">The type of the base class</typeparam>
    public class InheritanceConverter<T> : JsonConverter<T>
    {
        /// <summary>
        /// Gets the discriminator <see cref="PropertyInfo"/> of the base type to convert
        /// </summary>
        protected PropertyInfo DiscriminatorProperty { get; }

        /// <summary>
        /// Mapping of discriminator keys to types
        /// </summary>
        protected Dictionary<object, Type> TypeMappings { get; }

        public InheritanceConverter()
        {
            var typeofT = typeof(T);
            var discriminatorProperties = typeofT.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttributes<DiscriminatorAttribute>().Any()).ToArray();
            if (discriminatorProperties.Length > 1)
                throw new InvalidOperationException($"Multiple discriminators were specified against type '{typeofT}'. Only one property should be specified as a discriminator");

            if(discriminatorProperties.Length == 1)
            {
                this.DiscriminatorProperty = discriminatorProperties[0];
                this.TypeMappings = typeofT.GetCustomAttributes<DiscriminatorTypeMappingAttribute>().ToDictionary(dtma => dtma.Key, dtma => dtma.ChildType);
                if (this.TypeMappings.Any(kvp => kvp.Value.Equals(typeofT)))
                    throw new JsonException("The target of a discriminator type mapping may not be the base class as this will cause a recursion in deserialization");
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Start object token type expected");
            using (JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                string discriminatorPropertyName = options.PropertyNamingPolicy?.ConvertName(this.DiscriminatorProperty.Name);
                var typeofDiscriminator = Nullable.GetUnderlyingType(this.DiscriminatorProperty.PropertyType) ?? this.DiscriminatorProperty.PropertyType;
                object discriminatorValue = null;
                jsonDocument.RootElement.TryGetProperty(discriminatorPropertyName, out JsonElement discriminatorProperty);

                switch (discriminatorProperty.ValueKind)
                {
                    case JsonValueKind.Number:
                        discriminatorValue = discriminatorProperty.GetInt32();
                        break;

                    case JsonValueKind.String:
                        discriminatorValue = discriminatorProperty.GetString();
                        break;
                }

                if(null == discriminatorValue)
                    throw new ArgumentNullException($"A discriminator key was not provided within an instance of type '{typeof(T)}'");

                if (typeofDiscriminator.IsEnum)
                {
                    try
                    {
                        if (discriminatorValue is int dint)
                            discriminatorValue = Enum.ToObject(typeofDiscriminator, dint);
                        else if (discriminatorValue is string dstring)
                            discriminatorValue = Enum.Parse(typeofDiscriminator, dstring);
                    }
                    catch
                    {
                        throw new JsonException($"The discriminator key '{discriminatorValue}' was invalid");
                    }
                }

                if (!this.TypeMappings.TryGetValue(discriminatorValue, out Type derivedType))
                    throw new JsonException($"Failed to find the derived type with the specified discriminator key '{discriminatorValue}'");

                string json = jsonDocument.RootElement.GetRawText();
                return (T)JsonSerializer.Deserialize(json, derivedType, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}
