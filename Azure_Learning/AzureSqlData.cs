using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureLearning.DbContexts;
using AzureLearning.Interfaces;
using AzureLearning.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureLearning
{
    public class AzureSqlData : IAzureContext, IDisposable
    {
        private readonly AppSettings _settings;
        private readonly SqlDbContext _context;
        private readonly SemaphoreSlim _semaphore;
        private readonly Mutex _mutex;

        public AzureSqlData(AppSettings settings)
        {
            _settings = settings;
            _context = new SqlDbContext(settings);
            _semaphore = new SemaphoreSlim(1, 1);
            _mutex = new Mutex(false);
        }

        public virtual async Task<T> AddAsync<T>(T item, CancellationToken token = default) where T : class
        {
            await _semaphore.WaitAsync(token);
            try
            {
                _context.Add(item);
                if (await _context.SaveChangesAsync(token) == 1)
                {
                    return item;
                };

                throw new DbAddFailedException();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual IEnumerable<T> Get<T>(Func<T, bool> filter = null) where T : class
        {
            _semaphore.Wait();
            try
            {
                var db = _context.Set<T>();

                return filter == null
                    ? db
                    : db.AsEnumerable().Where(filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task<T> GetById<T>(Guid id, CancellationToken token = default) where T : class, ISqlEntry
        {
            await _semaphore.WaitAsync(token);
            try
            {
                var db = _context.Set<T>();

                return await AsyncEnumerable.FirstOrDefaultAsync(db, w => w.Id == id, token);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task<bool> RemoveAsync<T>(T item, CancellationToken token = default) where T : class, ISqlEntry
        {
            await _semaphore.WaitAsync(token);
            try
            {
                _context.Attach(item).State = EntityState.Deleted;

                return await _context.SaveChangesAsync(token) == 1;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task<bool> RemoveManyAsync<T>(IEnumerable<T> items, CancellationToken token = default) where T : class
        {
            await _semaphore.WaitAsync(token);
            try
            {
                var db = _context.Set<T>();

                db.RemoveRange(items);
                return await _context.SaveChangesAsync(token) == items.Count();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task<bool> UpdateAsync<T>(T item, CancellationToken token = default) where T : class
        {
            await _semaphore.WaitAsync(token);
            try
            {
                var db = _context.Set<T>();

                db.Update(item);
                return await _context.SaveChangesAsync(token) == 1;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            try
            {
                _context.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}