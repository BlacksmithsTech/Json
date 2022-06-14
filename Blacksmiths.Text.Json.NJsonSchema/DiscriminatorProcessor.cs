using Blacksmiths.Text.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

                    context.Schema.DiscriminatorObject = new OpenApiDiscriminator();
                    context.Schema.DiscriminatorObject.PropertyName = propertyName;

                    foreach (var mappedType in discriminatorDetails.TypeMappings.Values)
                        if (!context.Resolver.HasSchema(mappedType, false))
                        {
                            context.Generator.Generate(mappedType, context.Resolver);
                        }
                }
            }
        }
    }
}
