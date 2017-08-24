using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.Core.Abstractions;
using System;
using OrientDB.Net.Core.Models;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseCreateOperation : IOrientDBOperation<OrientDBBinaryConnection>
    {
        private readonly string _databaseName;
        private readonly DatabaseType _databaseType;
        private readonly StorageType _storageType;
        private readonly ConnectionMetaData _metaData;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;
        private readonly ServerConnectionOptions _options;
        private readonly IOrientDBLogger _logger;

        public DatabaseCreateOperation(string databaseName, DatabaseType databaseType, StorageType storageType, ConnectionMetaData metaData, ServerConnectionOptions options, IOrientDBRecordSerializer<byte[]> serializer, IOrientDBLogger logger)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException($"{nameof(databaseName)} cannot be zero length or null.");
            _metaData = metaData ?? throw new ArgumentNullException($"{nameof(metaData)} cannot be null.");
            _options = options ?? throw new ArgumentNullException($"{nameof(options)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null");

            _databaseName = databaseName;
            _databaseType = databaseType;
            _storageType = storageType;           
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Synchronous, sessionId);

            request.AddDataItem((byte)OperationType.DB_CREATE);
            request.AddDataItem(request.SessionId);

            // operation specific fields
            request.AddDataItem(_databaseName);
            request.AddDataItem(_databaseType.ToString().ToLower());
            request.AddDataItem(_storageType.ToString().ToLower());
            if (_metaData.ProtocolVersion >= 36) request.AddDataItem(-1); //Send null string for non-incrmental backup option

            return request;
        }

        public OrientDBBinaryConnection Execute(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            return new OrientDBBinaryConnection(new DatabaseConnectionOptions()
            {
                Database = _databaseName,
                HostName = _options.HostName,
                Password = _options.Password,
                PoolSize = _options.PoolSize,
                Port = _options.Port,
                Type = _databaseType,
                UserName = _options.UserName
            }, _serializer, _logger);
        }
    }
}
