using OrientDB.Net.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrientDB.Net.Core.Abstractions
{
    public interface IOrientServerConnection
    {
        IOrientDatabaseConnection CreateDatabase(string database, DatabaseType databaseType, StorageType type);
        IOrientDatabaseConnection DatabaseConnect(string database, DatabaseType storageType, int poolSize = 10);
        void DeleteDatabase(string database, StorageType storageType);
        bool DatabaseExists(string database, StorageType storageType);
        void Shutdown(string username, string password);
        IEnumerable<string> ListDatabases();
        string GetConfigValue(string name);
        void SetConfigValue(string name, string value);
        Task<IOrientDatabaseConnection> CreateDatabaseAsync(string database, DatabaseType databaseType, StorageType storageType);
        Task<IOrientDatabaseConnection> DatabaseConnectAsync(string database, DatabaseType storageType, int poolSize = 10);
        Task DeleteDatabaseAsync(string database, StorageType storageType);
        Task<bool> DatabaseExistsAsync(string database, StorageType storageType);
        Task ShutdownAsync(string username, string password);
        Task<IEnumerable<string>> ListDatabasesAsync();
        Task<string> GetConfigValueAsync(string name);
        Task SetConfigValueAsync(string name, string value);
    }
}
