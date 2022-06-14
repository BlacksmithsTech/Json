using Blacksmiths.Tests.Json.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacksmiths.Tests.Json
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void DeserialiseNoDiscriminator()
        {
            var json = "{ \"name\": \"Test Name\"}";

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var person = Deserialize<Person>(json);
            });
        }

        [TestMethod]
        public void DeserialiseBadDiscriminator()
        {
            var json = "{ \"name\": \"Test Name\", \"type\": \"Unknown\" }";

            Assert.ThrowsException<JsonException>(() =>
            {
                var person = Deserialize<Person>(json);
            });
        }

        [TestMethod]
        public void DeserialiseEmployee()
        {
            var json = "{ \"name\": \"Test Name\", \"type\": \"Employee\", \"officeNumber\": \"B4\" }";
            this.DeserialiseEmployee(json);
        }

        [TestMethod]
        public void DeserialiseEmployeeUsingInt()
        {
            var json = "{ \"name\": \"Test Name\", \"type\": 1, \"officeNumber\": \"B4\" }";
            this.DeserialiseEmployee(json);
        }

        private void DeserialiseEmployee(string json)
        {
            var person = Deserialize<Person>(json);
            Assert.AreEqual("Blacksmiths.Tests.Json.Models.Employee", person?.GetType().ToString());
            var employee = person as Employee;
            Assert.AreEqual("B4", employee?.OfficeNumber);
        }

        [TestMethod]
        public void DeserialiseCustomer()
        {
            var json = "{ \"name\": \"Test Name\", \"type\": \"Customer\", \"creditLimit\": 123.45 }";
            var person = Deserialize<Person>(json);

            Assert.AreEqual("Blacksmiths.Tests.Json.Models.Customer", person?.GetType().ToString());
            var customer = person as Customer;
            Assert.AreEqual(123.45M, customer?.CreditLimit);
        }

        private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, GetOptions());
        private static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, GetOptions());

        private static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                    {
                        new JsonStringEnumConverter(),
                        new Text.Json.Serialization.InheritanceConverterFactory(),
                    }
            };
        }
    }
}