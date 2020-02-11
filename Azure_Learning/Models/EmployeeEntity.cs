using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureLearning.Models
{
    public class EmployeeEntity : TableEntity, ITableEntry
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("_self")]
        public string DocumentLink { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public EmployeeEntity()
        {
        }

        public EmployeeEntity(string firstName, string lastName)
        {
            PartitionKey = "/LastName";
            FirstName = firstName;
            LastName = lastName;
            Id = Guid.NewGuid().ToString();
        }
    }

    public interface ITableEntry
    {
        public string Id { get; set; }
        public string DocumentLink { get; set; }
    }
}