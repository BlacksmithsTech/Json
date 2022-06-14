using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacksmiths.Text.Json.Serialization
{
    public class InheritanceConverterFactory : JsonConverterFactory
    {
        protected static Dictionary<Type, JsonConverter> Converters = new Dictionary<Type, JsonConverter>();
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsClass && typeToConvert.IsDefined(typeof(DiscriminatorTypeMappingAttribute));
        }

        public InheritanceConverterFactory()
        {
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!Converters.TryGetValue(typeToConvert, out JsonConverter converter))
            {
                Type converterType = typeof(InheritanceConverter<>).MakeGenericType(typeToConvert);
                converter = (JsonConverter)Activator.CreateInstance(converterType, options.PropertyNamingPolicy);
                Converters.Add(typeToConvert, converter);
            }
            return converter;
        }

    }
}
