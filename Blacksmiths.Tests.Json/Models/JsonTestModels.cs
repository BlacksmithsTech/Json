using Blacksmiths.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blacksmiths.Tests.Json.Models
{
    public enum PersonType
    {
        Customer,
        Employee,
    }

    [DiscriminatorTypeMapping(PersonType.Customer, typeof(Customer))]
    [DiscriminatorTypeMapping(PersonType.Employee, typeof(Employee))]
    public class Person
    {
        [Discriminator]
        public PersonType? Type { get; set; }
        public string? Name { get; set; }
    }

    public class Customer : Person
    {
        public decimal CreditLimit { get; set; }
    }

    public class Employee : Person
    {
        public string? OfficeNumber { get; set; }
    }

    public class Business
    {
        public Person? Customer { get; set; }
        public Person? Employee { get; set; }
    }
}
