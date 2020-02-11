using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureLearning.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLearning
{
    public class AzureCosmosDb
    {
        private readonly AppSettings _settings;
        private DocumentClient _documentClient;
        private readonly Uri _collectionUri;
        private readonly FeedOptions _queryOptions;

        public AzureCosmosDb(AppSettings settings)
        {
            _settings = settings;
            _documentClient = new DocumentClient(new Uri(settings.CosmosDb.Uri), settings.CosmosDb.Key);
            _collectionUri =
                UriFactory.CreateDocumentCollectionUri(_settings.CosmosDb.DatabaseName,
                    _settings.CosmosDb.CollectionName);

            _queryOptions = new FeedOptions
            {
                MaxItemCount = -1,
                EnableCrossPartitionQuery = true
            };
        }

        public async Task InsertItemAsync<T>(T item) where T : TableEntity, ITableEntry
        {
            // using (var context = new CosmosDbContext(_settings))
            // {
            //     await context.Database.EnsureCreatedAsync();
            //     context.Add(item);
            //     await context.SaveChangesAsync();
            // }

            var result = await _documentClient.CreateDocumentAsync(_collectionUri, item);
        }

        public IEnumerable<T> QueryItem<T>(Func<T, bool> filter = null)
        {
            var entries = _documentClient.CreateDocumentQuery<T>(_collectionUri, _queryOptions);

            return filter != null
                ? entries.Where(filter)
                : entries;
        }

        public async Task DeleteItemAsync<T>(Func<T, bool> filter, string partitionFilter) where T : TableEntity, ITableEntry
        {
            var entry = QueryItem(filter).FirstOrDefault();

            if (entry == null)
            {
                return;
            }
            var partitionKey = new PartitionKey(partitionFilter);
            var requestOptions = new RequestOptions
            {
                PartitionKey = partitionKey
            };

            var document = GetDocument(entry.Id);

            var delete = await _documentClient.DeleteDocumentAsync(entry.DocumentLink);
        }

        private Uri GetDocumentLink(string id)
        {
            return UriFactory.CreateDocumentUri(_settings.CosmosDb.DatabaseName, _settings.CosmosDb.CollectionName, id);
        }

        private Document GetDocument(string id)
        {
            return _documentClient.CreateDocumentQuery(_collectionUri, _queryOptions)
                .AsEnumerable()
                .FirstOrDefault(w => w.Id == id);
        }
    }
}