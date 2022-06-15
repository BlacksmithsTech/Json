using Blacksmiths.Text.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Blacksmiths.Text.Json.NJsonSchema
{
    public class DiscriminatorProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            var discriminatorDetails = new DiscriminatorDetails(context.ContextualType.Type);
            if(null != discriminatorDetails.DiscriminatorProperty)
            {
                if (context.Schema.Properties.Keys.Contains(discriminatorDetails.DiscriminatorProperty.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    // Object wishes to use a discriminator.
                    var propertyName = context.Schema.Properties.First(p => p.Key.Equals(discriminatorDetails.DiscriminatorProperty.Name, StringComparison.CurrentCultureIgnoreCase)).Key;

                    var discriminator = new OpenApiDiscriminator();
                    context.Schema.DiscriminatorObject = discriminator;
                    context.Schema.DiscriminatorObject.PropertyName = propertyName;
                    
                    foreach (var mappedKvp in discriminatorDetails.TypeMappings)
                        if (!context.Resolver.HasSchema(mappedKvp.Value, false))
                        {
                            var schema = context.Generator.Generate(mappedKvp.Value, context.Resolver);
                            context.Schema.DiscriminatorObject.Mapping.Remove(context.Schema.DiscriminatorObject.Mapping.Keys.Last());//remove NJsonSchema mapping with bad name
                            context.Schema.DiscriminatorObject.Mapping.Add(mappedKvp.Key.ToString(), new JsonSchema { Reference = schema.ActualSchema });
                        }
                }
            }
        }
    }
}
