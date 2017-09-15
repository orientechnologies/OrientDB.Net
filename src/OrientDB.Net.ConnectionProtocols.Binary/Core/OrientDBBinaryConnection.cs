using OrientDB.Net.ConnectionProtocols.Binary.Command;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using System;
using OrientDB.Net.Core.Abstractions;
using System.Linq;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using OrientDB.Net.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class OrientDBBinaryConnection : IOrientDatabaseConnection
    {
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;
        private readonly DatabaseConnectionOptions _connectionOptions;
        private OrientDBBinaryConnectionStream _connectionStream;
        private OpenDatabaseResult _openResult; // might not be how I model this here in the end.
        private readonly ICommandPayloadConstructorFactory _payloadFactory;
        private readonly ILogger _logger;

        public OrientDBBinaryConnection(DatabaseConnectionOptions options, IOrientDBRecordSerializer<byte[]> serializer, ILogger logger)
        {
            _connectionOptions = options ?? throw new ArgumentNullException($"{nameof(options)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            _payloadFactory = new CommandPayloadConstructorFactory(serializer, logger);

            Open();          
        }

        public OrientDBBinaryConnection(string hostname, string username, string password, IOrientDBRecordSerializer<byte[]> serializer, ILogger logger, int port = 2424, int poolsize = 10)
        {
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException($"{nameof(hostname)} cannot be null or zero length.");
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"{nameof(username)} cannot be null or zero length.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"{nameof(password)} cannot be null or zero length.");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");

            _connectionOptions = new DatabaseConnectionOptions
            {
                HostName = hostname,
                Password = password,
                PoolSize = poolsize,
                Port = port,
                UserName = username
            };

            Open();
        }

        public void Open()
        {
            _connectionStream = new OrientDBBinaryConnectionStream(_connectionOptions, _logger);
            foreach(var stream in _connectionStream.StreamPool)
            {
                _openResult = _connectionStream.Send(new DatabaseOpenOperation(_connectionOptions, _connectionStream.ConnectionMetaData));
                stream.SessionId = _openResult.SessionId;
                stream.Token = _openResult.Token;
                _logger.LogDebug($"Opened connection with session id {stream.SessionId}");
            }
        }

        public void Close()
        {
            _connectionStream.Send(new DatabaseCloseOperation(_openResult.Token, _connectionStream.ConnectionMetaData));
            _connectionStream.Close();
        }   

        public IOrientDBCommandResult ExecuteCommand(string sql)
        {
            return new OrientDBCommand(_connectionStream, _serializer, _payloadFactory, _logger).Execute(sql);
        }

        public async Task<IOrientDBCommandResult> ExecuteCommandAsync(string sql)
        {
            return await new OrientDBCommand(_connectionStream, _serializer, _payloadFactory, _logger)
                .ExecuteAsync(sql);
        }

        private IOrientDBCommand CreateCommand()
        {
            return new OrientDBCommand(_connectionStream, _serializer, _payloadFactory, _logger);
        }

        public void Dispose()
        {
            Close();
        }

        public IOrientDBTransaction CreateTransaction()
        {
            return new BinaryOrientDBTransaction(_connectionStream, _serializer, _connectionStream.ConnectionMetaData, (clusterName) =>
            {
                var schema = CreateCommand().Execute<ClassSchema>($"select expand(classes) from metadata:schema").First(n => n.Name == clusterName);
                return schema.DefaultClusterId;
            });
        }

        public async Task<IOrientDBTransaction> CreateTransactionAsync()
        {
            var classSchemata = await CreateCommand().ExecuteAsync<ClassSchema>($"select expand(classes) from metadata:schema");

            return new BinaryOrientDBTransaction(_connectionStream, _serializer, _connectionStream.ConnectionMetaData, (clusterName) =>
            {
                var schema = classSchemata.First(n => n.Name == clusterName);
                return schema.DefaultClusterId;
            });
        }

        public async Task<IEnumerable<TResultType>> ExecuteQueryAsync<TResultType>(string sql) where TResultType : OrientDBEntity
        {
            return await CreateCommand().ExecuteAsync<TResultType>(sql);
        }

        public IEnumerable<TResultType> ExecuteQuery<TResultType>(string sql) where TResultType : OrientDBEntity
        {
            return CreateCommand().Execute<TResultType>(sql);
        }

        public IEnumerable<TResultType> ExecutePreparedQuery<TResultType>(string sql, params string[] parameters) where TResultType : OrientDBEntity
        {
            return CreateCommand().ExecutePrepared<TResultType>(sql, parameters);
        }

        public async Task OpenAsync()
        {
            _connectionStream = new OrientDBBinaryConnectionStream(_connectionOptions, _logger);
            foreach (var stream in _connectionStream.StreamPool)
            {
                _openResult = await _connectionStream.SendAsync(new DatabaseOpenOperation(_connectionOptions, _connectionStream.ConnectionMetaData));
                stream.SessionId = _openResult.SessionId;
                stream.Token = _openResult.Token;
                _logger.LogDebug($"Opened connection with session id {stream.SessionId}");
            }
        }
    }
}
