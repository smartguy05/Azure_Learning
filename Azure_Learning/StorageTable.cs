using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureLearning.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLearning
{
    public class StorageTable
    {
        private CloudTable _cloudTable;
        private readonly AppSettings _settings;

        public StorageTable(AppSettings settings)
        {
            _settings = settings;
            ConfigureAzure();
        }

        public async Task InsertNewItemAsync<T>(T employee) where T : TableEntity
        {
            var insertOp = TableOperation.Insert(employee);
            await _cloudTable.ExecuteAsync(insertOp);
        }

        public async IAsyncEnumerable<T> QueryItemAsync<T>(IEnumerable<TableQueryParam> filters = null) where T : TableEntity, new()
        {
            var query = new TableQuery<T>();

            foreach (var filter in filters)
            {
                query = query.Where(TableQuery.GenerateFilterCondition(filter.Name, QueryComparisons.Equal, filter.Value));
            }

            var token = new TableContinuationToken();

            while (token != null)
            {
                var result = await _cloudTable.ExecuteQuerySegmentedAsync(query, token);
                token = result.ContinuationToken;

                if (result.Any())
                {
                    foreach (var entity in result)
                    {
                        yield return entity;
                    }
                }
            }
        }

        public async Task DeleteItemAsync<T>(IEnumerable<TableQueryParam> filters) where T : TableEntity, new()
        {
            var filteredItems = QueryItemAsync<T>(filters);
            var items = await filteredItems.ToListAsync();

            if (items.Any())
            {
                var deleteTasks = new List<Task>();

                foreach (var item in items)
                {
                    var operation = TableOperation.Delete(item);
                    deleteTasks.Add(_cloudTable.ExecuteAsync(operation));
                }

                await Task.WhenAll(deleteTasks);
            }
        }

        private void ConfigureAzure()
        {
            var storageAccount = CloudStorageAccount.Parse(_settings.ConnectionStrings.CosmosDb);
            var tableClient = storageAccount.CreateCloudTableClient();
            _cloudTable = tableClient.GetTableReference("Employees");
        }
    }
}