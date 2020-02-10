using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace StorageAccountTables.Models
{
    public class EmployeeEntity : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public EmployeeEntity()
        {
        }

        public EmployeeEntity(string firstName, string lastName)
        {
            PartitionKey = "staff";
            RowKey = Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
        }
    }
}