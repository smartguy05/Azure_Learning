namespace StorageAccountTables.Models
{
    public class AppSettings
    {
        public CosmosDbConfiguration CosmosDb { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class CosmosDbConfiguration
    {
        public string Uri { get; set; }
        public string Key { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string StorageTables { get; set; }
    }
}