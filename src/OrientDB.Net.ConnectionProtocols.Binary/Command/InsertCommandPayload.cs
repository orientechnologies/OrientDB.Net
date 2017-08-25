using Microsoft.Extensions.Logging;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using OrientDB.Net.Core.Abstractions;
using System;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal class InsertCommandPayload : ICommandPayload
    {
        private readonly string _sqlString;
        private readonly string _fetchPlan;
        private readonly ConnectionMetaData _metaData;
        private readonly ILogger _logger;

        public InsertCommandPayload(string sql, string fetchPlan, ConnectionMetaData metaData, ILogger logger)
        {
            _sqlString = sql;
            _fetchPlan = fetchPlan;
            _metaData = metaData;
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
        }

        public Request CreatePayloadRequest(int sessionId, byte[] token)
        {
            CommandPayloadScript payload = new CommandPayloadScript();
            payload.Text = _sqlString;
            payload.Language = "sql";

            Request request = new Request(OperationMode.Synchronous, sessionId);

            request.AddDataItem((byte)OperationType.COMMAND);
            request.AddDataItem(request.SessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(token);
            }

            // operation specific fields
            request.AddDataItem((byte)request.OperationMode);

            var scriptPayload = payload;
            if (scriptPayload != null)
            {
                request.AddDataItem(scriptPayload.PayLoadLength);
                request.AddDataItem(scriptPayload.ClassName);
                if (scriptPayload.Language != "gremlin")
                    request.AddDataItem(scriptPayload.Language);
                request.AddDataItem(scriptPayload.Text);
                if (scriptPayload.SimpleParams == null)
                    request.AddDataItem((byte)0); // 0 - false, 1 - true
                else
                {
                    request.AddDataItem((byte)1);
                    request.AddDataItem(scriptPayload.SimpleParams);
                }
                request.AddDataItem((byte)0);
                return request;
            }
            // @todo Fix this to a better domain exception.
            throw new Exception("Need to fix this");
        }
    }
}
