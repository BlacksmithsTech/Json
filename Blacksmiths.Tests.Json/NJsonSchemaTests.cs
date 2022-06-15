using Blacksmiths.Tests.Json.Models;
using Blacksmiths.Text.Json.NJsonSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.Generation;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacksmiths.Tests.Json
{
    [TestClass]
    public class NJsonSchemaTests
    {
        [TestMethod]
        public void ManualTest()
        {
            var schema = JsonSchema.FromType<Business>(GetSettings());
            var json = schema.ToJson();

            Assert.IsNotNull(json);
            //TODO: assertions
        }

        private static JsonSchemaGeneratorSettings GetSettings()
        {
            var settings = new JsonSchemaGeneratorSettings();
            settings.SchemaProcessors.Add(new DiscriminatorProcessor());
            return settings;
        }
    }
}