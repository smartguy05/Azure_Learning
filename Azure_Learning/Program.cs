using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureLearning.Models;
using Microsoft.Extensions.Configuration;

namespace AzureLearning
{
    public class Program
    {
        private const string TestingName = "Anthony";
        private const string TestingLastName = "Smith";

        private static async Task Main(string[] args)
        {
            // Setup
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", true, true)
                .AddUserSecrets(Assembly.GetAssembly(typeof(Program)))
                .Build();

            var settings = new AppSettings();
            config.Bind(settings);

            // await TestAzureStorageTables(settings);
            await TestAzureCosmosDb(settings);
        }

        private static async Task TestAzureStorageTables(AppSettings settings)
        {
            var storageTables = new StorageTable(settings);

            // Create employees
            Console.WriteLine("Adding new employees");
            var employeesToAdd = CreateNewEmployees(3);
            var insertTasks = new List<Task>();

            await foreach (var emp in employeesToAdd)
            {
                insertTasks.Add(storageTables.InsertNewItemAsync(emp));
            }

            await Task.WhenAll(insertTasks);

            // Query employees
            var tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = TestingName
                }
            };
            var employees = storageTables.QueryItemAsync<EmployeeEntity>(tableParams);
            Console.WriteLine($"All staff with name {TestingName}:");
            Console.WriteLine("=====================================");
            await foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            //Delete employee
            Console.WriteLine($"Deleting employee with first name {TestingName} and last name {TestingLastName}");
            tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = TestingName
                },
                new TableQueryParam
                {
                    Name =  "LastName",
                    Value = TestingLastName
                }
            };
            await storageTables.DeleteItemAsync<EmployeeEntity>(tableParams);
            Console.WriteLine("Employee deleted");

            Console.WriteLine();

            // Query again
            tableParams = new List<TableQueryParam>
            {
                new TableQueryParam
                {
                    Name = "FirstName",
                    Value = TestingName
                }
            };
            employees = storageTables.QueryItemAsync<EmployeeEntity>(tableParams);
            Console.WriteLine($"All staff with name {TestingName}:");
            Console.WriteLine("=====================================");
            await foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
        }

        private static async IAsyncEnumerable<EmployeeEntity> CreateNewEmployees(int numberToCreate)
        {
            var firstNames = new[] { "Anthony", "Jennifer", "Aiden", "Alex", "Aaron", "Archer" };
            var lastNames = new[] { "James", "Smith", "Jones", "Lowderman", "Heusler", "Bonaparte" };
            var random = new Random();

            for (var i = 0; i < numberToCreate; i++)
            {
                var randomFirstName = firstNames[random.Next(firstNames.Length)];
                var randomLastName = lastNames[random.Next(lastNames.Length)];

                yield return new EmployeeEntity(randomFirstName, randomLastName);
            }
        }

        private static async Task TestAzureCosmosDb(AppSettings settings)
        {
            var cosmosDb = new AzureCosmosDb(settings);

            // create objects to insert
            Console.WriteLine("Adding new employees");
            
            // insert objects
            var insertTasks = new List<Task>();
            var employeeToInsert = new EmployeeEntity(TestingName, TestingLastName);
            var employeesToInsert = CreateNewEmployees(3);

            insertTasks.Add(cosmosDb.InsertAsync(employeeToInsert));
            await foreach (var emp in employeesToInsert)
            {
                insertTasks.Add(cosmosDb.InsertAsync(emp));
            }
            
            await Task.WhenAll(insertTasks);
            
            Console.WriteLine();

            // all employees
            var employees = cosmosDb.QueryItem<EmployeeEntity>();

            Console.WriteLine("All staff");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            // all employees with name {TestingName}
            employees = cosmosDb.QueryItem<EmployeeEntity>(w => w.FirstName == TestingName);

            Console.WriteLine($"All staff with the first name {TestingName}");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            //Delete employee
            Console.WriteLine($"Deleting employee with first name {TestingName} and last name {TestingLastName}");
            var deleteEmployee =
                cosmosDb.QueryItem<EmployeeEntity>(w => w.FirstName == TestingName && w.LastName == TestingLastName)
                    .FirstOrDefault();

            if (deleteEmployee != null)
            {
                await cosmosDb.DeleteAsync(deleteEmployee, CancellationToken.None);
            }

            Console.WriteLine($"Employee {TestingName} {TestingLastName} deleted");
            
            Console.WriteLine();
            
            // all employees
            employees = cosmosDb.QueryItem<EmployeeEntity>();
            
            Console.WriteLine("All staff");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
            
            Console.WriteLine();
        }
    }
}