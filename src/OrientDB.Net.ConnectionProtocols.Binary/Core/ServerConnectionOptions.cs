using System.Net;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class ServerConnectionOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int PoolSize { get; set; } = 10;    
    }
}