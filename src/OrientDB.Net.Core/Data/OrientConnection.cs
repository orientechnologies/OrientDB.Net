﻿using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrientDB.Net.Core.Data
{
    public class OrientConnection<TDataType> : IOrientDatabaseConnection
    {
        private readonly ILogger _logger;

        private readonly IOrientDatabaseConnection _databaseConnection;

        internal OrientConnection(
            IOrientDBRecordSerializer<TDataType> serializer, 
            IOrientDBConnectionProtocol<TDataType> connectionProtocol, 
            ILogger logger, 
            string database, 
            DatabaseType databaseType, 
            int poolSize = 10)
        {
            if (serializer == null) throw new ArgumentNullException($"{nameof(serializer)}");
            if (connectionProtocol == null) throw new ArgumentNullException($"{nameof(connectionProtocol)}");
            if (string.IsNullOrWhiteSpace(database)) throw new ArgumentException($"{nameof(database)}");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)}");

            var serverConnection = connectionProtocol.CreateServerConnection(serializer, logger);
            _databaseConnection = serverConnection.DatabaseConnect(database, databaseType, poolSize);
        }

        public async Task<IEnumerable<TResultType>> ExecuteQueryAsync<TResultType>(string sql) where TResultType : OrientDBEntity
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException($"{nameof(sql)} cannot be zero length or null");
            _logger.LogDebug($"Executing SQL Query: {sql}");
            return await _databaseConnection.ExecuteQueryAsync<TResultType>(sql);
        }

        public IEnumerable<TResultType> ExecuteQuery<TResultType>(string sql) where TResultType : OrientDBEntity
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException($"{nameof(sql)} cannot be zero length or null");
            _logger.LogDebug($"Executing SQL Query: {sql}");
            var data = _databaseConnection.ExecuteQuery<TResultType>(sql);
            return data;
        }

        public IOrientDBCommandResult ExecuteCommand(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException($"{nameof(sql)} cannot be zero length or null");
            _logger.LogDebug($"Executing SQL Command: {sql}");
            var data = _databaseConnection.ExecuteCommand(sql);
            return data;
        }

        public async Task<IOrientDBCommandResult> ExecuteCommandAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException($"{nameof(sql)} cannot be zero length or null");
            _logger.LogDebug($"Executing SQL Command: {sql}");
            var data = await _databaseConnection.ExecuteCommandAsync(sql);
            return data;
        }

        public IEnumerable<TResultType> ExecutePreparedQuery<TResultType>(string sql, params string[] parameters) where TResultType : OrientDBEntity
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException($"{nameof(sql)} cannot be zero length or null");
            if (parameters == null)
                throw new ArgumentNullException($"{nameof(parameters)} cannot be null");
            _logger.LogDebug($"Executing SQL Query: {sql}");
            var data = _databaseConnection.ExecutePreparedQuery<TResultType>(sql, parameters);
            return data;
        }

        public IOrientDBTransaction CreateTransaction()
        {
            return _databaseConnection.CreateTransaction();
        }

        public async Task<IOrientDBTransaction> CreateTransactionAsync()
        {
            return await _databaseConnection.CreateTransactionAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _databaseConnection?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
