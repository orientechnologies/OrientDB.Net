using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.Core.Abstractions;

namespace OrientDB.Net.ConnectionProtocols.Binary
{
    public class BinaryProtocol : IOrientDBConnectionProtocol<byte[]>
    {
        private readonly ServerConnectionOptions _options;
        private static OrientDBBinaryServerConnection _serverConnection;
        private IOrientDBLogger _logger;

        public BinaryProtocol(string hostName, string userName, string password, int port = 2424) : this(new ServerConnectionOptions
        {
            HostName = hostName,
            Password = password,
            Port = port,
            UserName = userName
        })
        { }

        public BinaryProtocol(ServerConnectionOptions options)
        {
            _options = options;
        }

        public IOrientServerConnection CreateServerConnection(IOrientDBRecordSerializer<byte[]> serializer, IOrientDBLogger logger)
        {
            _logger = logger;
            if (_serverConnection == null)
                _serverConnection = new OrientDBBinaryServerConnection(_options, serializer, _logger);
            _logger.Information("OrientDB.Net.ConnectionProtocols.Binary Initialized.");
            return _serverConnection;
        }

        public void Dispose()
        {
            _serverConnection.Dispose();
            _logger.Information("OrientDB.Net.ConnectionProtocols.Binary Disposed.");
        }
    }
}
