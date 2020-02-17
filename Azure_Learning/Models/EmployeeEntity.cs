using System;
using AzureLearning.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureLearning.Models
{
    public class EmployeeEntity : TableEntity, ITableEntry, IEmployee, ISqlEntry
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("_self")]
        public string DocumentLink { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public EmployeeEntity()
        {
            PartitionKey = "/LastName";
            Id = Guid.NewGuid();
        }

        public EmployeeEntity(string firstName, string lastName)
        {
            PartitionKey = "/LastName";
            FirstName = firstName;
            LastName = lastName;
            Id = Guid.NewGuid();
        }
    }
}