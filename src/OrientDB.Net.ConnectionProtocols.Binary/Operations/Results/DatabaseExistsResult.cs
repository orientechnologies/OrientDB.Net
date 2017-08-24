namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    class DatabaseExistsResult
    {
        public bool Exists { get; }
        public DatabaseExistsResult(bool databaseExists)
        {
            Exists = databaseExists;
        }
    }
}
