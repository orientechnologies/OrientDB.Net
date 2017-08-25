using Microsoft.Extensions.Logging;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal class CommandPayloadConstructorFactory : ICommandPayloadConstructorFactory
    {
        private readonly ILogger _logger;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;

        public CommandPayloadConstructorFactory(IOrientDBRecordSerializer<byte[]> serializer, ILogger logger)
        {
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannt be null.");
        }

        public ICommandPayload CreatePayload(string query, string fetchPlan, ConnectionMetaData metaData, string[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
                return CreateParameterizedPayload(query, fetchPlan, metaData, parameters);
            return CreateNonParameterizedPayload(query, fetchPlan, metaData);
        }

        private ICommandPayload CreateNonParameterizedPayload(string query, string fetchPlan, ConnectionMetaData metaData)
        {
            if (query.ToLower().StartsWith("select"))
                return new SelectCommandPayload(query, fetchPlan, metaData, _logger);
            if (query.ToLower().StartsWith("insert"))
                return new InsertCommandPayload(query, fetchPlan, metaData, _logger);
            if (query.ToLower().StartsWith("create")) // Maybe we really don't need a bunch of different types here.
                return new InsertCommandPayload(query, fetchPlan, metaData, _logger); // This works...
            if (query.ToLower().StartsWith("update"))
                return new InsertCommandPayload(query, fetchPlan, metaData, _logger);

            return null;
        }

        private ICommandPayload CreateParameterizedPayload(string query, string fetchPlan, ConnectionMetaData metaData, params string[] parameters)
        {
            if (query.ToLower().StartsWith("select"))
                return new SelectParameterizedCommandPayload(query, parameters, _serializer, fetchPlan, metaData, _logger);          

            return null;
        }
    }
}
