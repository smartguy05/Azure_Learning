using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureLearning.Interfaces;
using AzureLearning.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;

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

            // Cosmos Db
            // var cosmosDb = new AzureCosmosData(settings);
            // await TestAzureDb<EmployeeEntity>(cosmosDb);

            // Sql Server
            using (var sqlServer = new AzureSqlData(settings))
            {
                await TestAzureDb<SqlEmployee>(sqlServer);
            }
        }

        private static async Task TestAzureDb<T>(IAzureContext context) where T : class, IEmployee, ISqlEntry, new()
        {
            // insert objects
            var totalEmployees = context.Get<T>().Count();

            if (totalEmployees < 7)
            {
                // create objects to insert
                Console.WriteLine("Adding new employees");

                var insertTasks = new List<Task>();
                var employeeToInsert = new T()
                {
                    FirstName = TestingName,
                    LastName = TestingLastName
                };
                var employeesToInsert = CreateNewEmployees<T>(3);

                insertTasks.Add(context.AddAsync(employeeToInsert));
                await foreach (var emp in employeesToInsert)
                {
                    insertTasks.Add(context.AddAsync(emp));
                }

                await Task.WhenAll(insertTasks);

                Console.WriteLine();
            }

            // all employees
            var employees = context.Get<T>();

            Console.WriteLine("All staff");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            // all employees with name {TestingName}
            employees = context.Get<T>(w => w.FirstName == TestingName);

            Console.WriteLine($"All staff with the first name {TestingName}");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            //Delete employee
            var deleteEmployee =
                context.Get<T>(w => w.FirstName == TestingName && w.LastName == TestingLastName)
                    .FirstOrDefault();

            if (deleteEmployee != null)
            {
                Console.WriteLine($"Deleting employee with first name {TestingName} and last name {TestingLastName}");

                await context.RemoveAsync(deleteEmployee, CancellationToken.None);

                Console.WriteLine($"Employee {TestingName} {TestingLastName} deleted");
            }

            Console.WriteLine();

            // all employees
            employees = context.Get<T>();

            Console.WriteLine("All staff");
            Console.WriteLine("=====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine();

            Console.ReadKey();
        }

        private static async Task TestAzureStorageTables(AppSettings settings)
        {
            var storageTables = new StorageTable(settings);

            // Create employees
            var totalEmployees = await storageTables.QueryItemAsync<EmployeeEntity>().CountAsync();

            if (totalEmployees < 7)
            {
                Console.WriteLine("Adding new employees");
                var employeesToAdd = CreateNewEmployees<EmployeeEntity>(3);
                var insertTasks = new List<Task>();

                await foreach (var emp in employeesToAdd)
                {
                    insertTasks.Add(storageTables.InsertNewItemAsync(emp));
                }

                await Task.WhenAll(insertTasks);

                Console.WriteLine();
            }

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

        private static async IAsyncEnumerable<T> CreateNewEmployees<T>(int numberToCreate) where T : class, IEmployee, new()
        {
            var firstNames = new[] { "Anthony", "Jennifer", "Aiden", "Alex", "Aaron", "Archer" };
            var lastNames = new[] { "James", "Smith", "Jones", "Lowderman", "Heusler", "Bonaparte" };
            var random = new Random();

            for (var i = 0; i < numberToCreate; i++)
            {
                var randomFirstName = firstNames[random.Next(firstNames.Length)];
                var randomLastName = lastNames[random.Next(lastNames.Length)];

                yield return new T
                {
                    FirstName = randomFirstName,
                    LastName = randomLastName
                };
            }
        }
    }
}