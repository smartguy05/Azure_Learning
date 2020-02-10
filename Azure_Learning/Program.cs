using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using StorageAccountTables.Models;

namespace StorageAccountTables
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            // Setup
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true, true)
                .Build();

            var settings = new AppSettings();
            config.Bind(settings);

            // await TestAzureStorageTables(settings);
        }

        private static async Task TestAzureStorageTables(AppSettings settings)
        {
            var testingName = "Aiden";
            var testingLastName = "Heusler";
            var storageTables = new StorageTable(settings);

            // Create employees
            Console.WriteLine("Adding new employees");
            await CreateNewEmployees(storageTables);

            // Query employees
            var tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = testingName
                }
            };
            var employees = storageTables.QueryItem<EmployeeEntity>(tableParams);
            Console.WriteLine($"All staff with name {testingName}:");
            Console.WriteLine("=====================================");
            await foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            //Delete employee
            Console.WriteLine($"Deleting employee with first name {testingName} and last name James");
            tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = testingName
                },
                new TableQueryParam
                {
                    Name =  "LastName",
                    Value = testingLastName
                }
            };
            await storageTables.DeleteItem<EmployeeEntity>(tableParams);
            Console.WriteLine("Employee deleted");

            Console.WriteLine();

            // Query again
            tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = testingName
                }
            };
            employees = storageTables.QueryItem<EmployeeEntity>(tableParams);
            Console.WriteLine($"All staff with name {testingName}:");
            Console.WriteLine("=====================================");
            await foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
        }

        public static async Task CreateNewEmployees(StorageTable table)
        {
            var firstNames = new[] { "Anthony", "Jennifer", "Aiden", "Alex", "Aaron", "Archer" };
            var lastNames = new[] { "James", "Smith", "Jones", "Lowderman", "Heusler", "Bonaparte" };
            var random = new Random();

            for (var i = 0; i < 3; i++)
            {
                var randomFirstName = firstNames[random.Next(firstNames.Length)];
                var randomLastName = lastNames[random.Next(lastNames.Length)];

                var employee1 = new EmployeeEntity(randomFirstName, randomLastName);

                await table.InsertNewItem(employee1);
            }
        }
    }
}