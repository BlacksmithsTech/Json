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
            this.ProcessDiscriminator(context);
            this.ProcessOneOf(context);
        }

        private void ProcessOneOf(SchemaProcessorContext context)
        {
            if (context.Schema.Properties.Count > 0)
            {
                foreach (var property in context.ContextualType.Properties)
                {
                    var matchedProperties = context.Schema.Properties.Where(p => p.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));
                    if (1 == matchedProperties.Count())
                    {
                        var propertySchema = matchedProperties.First().Value;
                        var propertyType = property.PropertyType.ElementType ?? property.PropertyType;

                        var typeMappings = propertyType.GetAttributes<DiscriminatorTypeMappingAttribute>();
                        if (typeMappings.Any())
                        {
                            if (null != property.PropertyType.ElementType)
                            {
                                // ** Array.
                                propertySchema.Item = null;

                                var subSchema = new JsonSchema();
                                foreach (var typeMapping in typeMappings)
                                {
                                    subSchema.OneOf.Add(new JsonSchema { Reference = context.Resolver.GetSchema(typeMapping.ChildType, false) });
                                }

                                propertySchema.Item = subSchema;
                            }
                            else
                            {
                                // ** Object
                                propertySchema.OneOf.Remove(propertySchema.OneOf.Last());//remove the base class

                                foreach (var typeMapping in typeMappings)
                                {
                                    propertySchema.OneOf.Add(new JsonSchema { Reference = context.Resolver.GetSchema(typeMapping.ChildType, false) });
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds classes which wish to use a discriminator and applies it to the schema
        /// </summary>
        private void ProcessDiscriminator(SchemaProcessorContext context)
        {
            var discriminatorDetails = new DiscriminatorDetails(context.ContextualType.Type);
            if (null != discriminatorDetails.DiscriminatorProperty)
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
