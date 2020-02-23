using System.Collections.Generic;
using Newtonsoft.Json;

namespace AzureLearning.Models
{
    public class AppSettings
    {
        public CosmosDbConfiguration CosmosDb { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public string AllowedHosts { get; set; }
        public ActiveDirectory ActiveDirectory { get; set; }
        public IEnumerable<string> ApiKeys { get; set; }
    }

    public class CosmosDbConfiguration
    {
        public string Uri { get; set; }
        public string Key { get; set; }
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string CosmosDb { get; set; }
        public string SqlDb { get; set; }
    }

    public class ActiveDirectory
    {
        public string ApplicationId { get; set; }
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }

        [JsonProperty("Microsoft.Hosting.Lifetime")]
        public string Lifetime { get; set; }
    }
}