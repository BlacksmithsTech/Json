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
        protected DiscriminatorDetails DiscriminatorDetails { get; }

        public InheritanceConverter()
        {
            this.DiscriminatorDetails = new DiscriminatorDetails(typeof(T));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (null == this.DiscriminatorDetails.DiscriminatorProperty)
                throw new ArgumentNullException($"An inheritance converter is running against type '{typeof(T)}' which doesn't declare a property with DiscriminatorAttribute");

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Start object token type expected");
            using (JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                string discriminatorPropertyName = options.PropertyNamingPolicy?.ConvertName(this.DiscriminatorDetails.DiscriminatorProperty.Name);
                var typeofDiscriminator = Nullable.GetUnderlyingType(this.DiscriminatorDetails.DiscriminatorProperty.PropertyType) ?? this.DiscriminatorDetails.DiscriminatorProperty.PropertyType;
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

                if (!this.DiscriminatorDetails.TypeMappings.TryGetValue(discriminatorValue, out Type derivedType))
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
