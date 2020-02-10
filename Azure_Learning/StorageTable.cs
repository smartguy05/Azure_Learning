using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StorageAccountTables.Models;

namespace StorageAccountTables
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

        public async Task InsertNewItem<T>(T employee) where T : TableEntity
        {
            var insertOp = TableOperation.Insert(employee);
            await _cloudTable.ExecuteAsync(insertOp);
        }

        public async IAsyncEnumerable<T> QueryItem<T>(IEnumerable<TableQueryParam> filters) where T : TableEntity, new()
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

        public async Task DeleteItem<T>(IEnumerable<TableQueryParam> filters) where T : TableEntity, new()
        {
            var filteredItems = QueryItem<T>(filters);
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
            var storageAccount = CloudStorageAccount.Parse(_settings.ConnectionStrings.StorageTables);
            var tableClient = storageAccount.CreateCloudTableClient();
            _cloudTable = tableClient.GetTableReference("Employees");
        }
    }
}