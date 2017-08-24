using OrientDB.Net.Core.Models;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class DatabaseConnectionOptions : ServerConnectionOptions
    {
        public string Database { get; set; }
        public DatabaseType Type { get; set; }
    }
}
