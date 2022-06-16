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

    [DiscriminatorTypeMapping(PersonType.Customer, typeof(DTO_Customer))]
    [DiscriminatorTypeMapping(PersonType.Employee, typeof(DTO_Employee))]
    public class Person
    {
        [Discriminator]
        public PersonType? Type { get; set; }
        public string? Name { get; set; }
    }

    public class DTO_Customer : Person
    {
        [System.ComponentModel.DataAnnotations.Required]
        public decimal? CreditLimit { get; set; }
    }

    public class DTO_Employee : Person
    {
        public string? OfficeNumber { get; set; }
    }

    public class Business
    {
        public Person? Customer { get; set; }
        public Person? Employee { get; set; }
        public Person[] People { get; set; }
    }
}
