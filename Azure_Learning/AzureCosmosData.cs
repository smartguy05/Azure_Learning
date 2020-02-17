using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureLearning.DbContexts;
using AzureLearning.Interfaces;
using AzureLearning.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLearning
{
    public class AzureCosmosData : IAzureContext
    {
        private readonly AppSettings _settings;

        public AzureCosmosData(AppSettings settings)
        {
            _settings = settings;
            using (var context = new CosmosDbContext(settings))
            {
                context.Database.EnsureCreated();
            }
        }

        public virtual async Task<T> AddAsync<T>(T item, CancellationToken token = default) where T : class
        {
            using (var context = new CosmosDbContext(_settings))
            {
                context.Add(item);
                if (await context.SaveChangesAsync(token) == 1)
                {
                    return item;
                };

                throw new DbAddFailedException();
            }
        }

        public virtual IEnumerable<T> Get<T>(Func<T, bool> filter = null) where T : class
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var dbSet = context.Set<T>().AsEnumerable();

                return filter != null
                    ? dbSet?.Where(filter).ToList()
                    : dbSet.ToList();
            }
        }

        public virtual async Task<T> GetById<T>(Guid id) where T : class, ISqlEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                return await AsyncEnumerable.FirstOrDefaultAsync(context.Set<T>(), w => w.Id == id);
            }
        }

        public virtual async Task<bool> RemoveAsync<T>(T item, CancellationToken token = default) where T : class, ISqlEntry
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var dbSet = context.Set<T>();
                var entity = dbSet.AsEnumerable().FirstOrDefault(w => w.Id == item.Id);
                context.Remove(entity);
                return await context.SaveChangesAsync(token) == 1;
            }
        }

        public virtual async Task<bool> RemoveManyAsync<T>(IEnumerable<T> items, CancellationToken token = default) where T : class
        {
            using (var context = new CosmosDbContext(_settings))
            {
                context.AttachRange(items);
                context.RemoveRange(items);
                return await context.SaveChangesAsync(token) == items.Count();
            }
        }

        public virtual async Task<bool> UpdateAsync<T>(T item, CancellationToken token = default) where T : class
        {
            using (var context = new CosmosDbContext(_settings))
            {
                var db = context.Set<T>();

                db.Update(item);
                return await context.SaveChangesAsync(token) == 1;
            }
        }
    }
}