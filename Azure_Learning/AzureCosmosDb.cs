using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureLearning.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLearning
{
    public class AzureCosmosDb
    {
        private readonly AppSettings _settings;

        public AzureCosmosDb(AppSettings settings)
        {
            _settings = settings;
            using (var context = new CosmosDbContext(settings))
            {
                context.Database.EnsureCreated();
            }
        }

        public async Task InsertAsync<T>(T item) where T: TableEntity, ITableEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                context.Add(item);
                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<T> QueryItem<T>(Func<T, bool> filter = null) where T: TableEntity, ITableEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var dbSet = context.Set<T>().AsEnumerable();

                return filter != null
                    ? dbSet?.Where(filter).ToList()
                    : dbSet.ToList();
            }
        }

        public async Task<bool> DeleteAsync<T>(string id, CancellationToken token) where T : TableEntity, ITableEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var dbSet = context.Set<T>();
                var entry = await dbSet.FirstOrDefaultAsync(w => w.Id == id, token);
                context.Remove(entry);
                return await context.SaveChangesAsync(token) == 1;
            }
        }

        public async Task<bool> DeleteAsync<T>(T item, CancellationToken token) where T : TableEntity, ITableEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var dbSet = context.Set<T>();
                var entry = await dbSet.FirstOrDefaultAsync(w => w.Id == item.Id, token);
                context.Remove(entry);
                return await context.SaveChangesAsync(token) == 1;
            }
        }

        public async Task<bool> DeleteManyAsync<T>(IEnumerable<T> items, CancellationToken token) where T: TableEntity, ITableEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                context.AttachRange(items);
                context.RemoveRange(items);
                return await context.SaveChangesAsync(token) == items.Count();
            }
        }
    }
}