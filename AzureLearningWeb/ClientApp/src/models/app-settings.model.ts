export class AppSettings {
  public cosmosDb: CosmosDbConfiguration;
  public connectionStrings: ConnectionStrings;
  public allowedHosts: string;
  public activeDirectory: ActiveDirectory;
  public apiKeys: string[];
}

export class CosmosDbConfiguration {
  public uri: string;
  public key: string;
  public collectionName: string;
  public databaseName: string;
}

export class ConnectionStrings {
  public defaultConnection: string;
  public cosmosDb: string;
  public sqlDb: string;
}

export class ActiveDirectory {
  public applicationId: string;
  public instance: string;
  public domain: string;
  public tenantId: string;
  public clientId: string;
}

export class Logging {
  public logLevel: LogLevel;
}

export class LogLevel {
  public default: string;
  public microsoft: string;
  public lifetime: string;
}
