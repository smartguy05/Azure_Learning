using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureLearning.Interfaces
{
    public interface IAzureContext
    {
        Task<T> AddAsync<T>(T item, CancellationToken token = default) where T : class;

        IEnumerable<T> Get<T>(Func<T, bool> filter = null) where T : class;

        Task<bool> RemoveAsync<T>(T item, CancellationToken token = default) where T : class, ISqlEntry;

        Task<bool> RemoveManyAsync<T>(IEnumerable<T> items, CancellationToken token = default) where T : class;

        Task<bool> UpdateAsync<T>(T item, CancellationToken token = default) where T : class;
    }
}