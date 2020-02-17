using System;
using AzureLearning.Interfaces;

namespace AzureLearning.Models
{
    public class SqlEmployee : ISqlEntry, IEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Id { get; set; }

        public SqlEmployee()
        {
            Id = Guid.NewGuid();
        }

        public SqlEmployee(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}