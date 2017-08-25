using Microsoft.Extensions.Logging;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using System;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal class ParametersEntity : OrientDBEntity
    {    
        public Dictionary<string, object> parameters { get; set; }
    }

    internal class SelectParameterizedCommandPayload : ICommandPayload
    {
        private readonly string _sqlString;
        private readonly string _fetchPlan;
        private readonly ConnectionMetaData _metaData;
        private readonly ILogger _logger;
        private readonly string[] _parameters;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;

        public SelectParameterizedCommandPayload(string sql, string[] parameters, IOrientDBRecordSerializer<byte[]> serializer, string fetchPlan, ConnectionMetaData metaData, ILogger logger)
        {
            _sqlString = sql;
            _fetchPlan = fetchPlan;
            _metaData = metaData;
            _parameters = parameters ?? throw new ArgumentNullException($"{nameof(parameters)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");
        }

        public Request CreatePayloadRequest(int sessionId, byte[] token)
        {
            //DictionaryOrientDBEntity paramsEntity = new DictionaryOrientDBEntity();
            //paramsEntity.SetField("params", _parameters as Dictionary<string, object>);

            CommandPayloadQuery payload = new CommandPayloadQuery();
            payload.Text = _sqlString;
            payload.NonTextLimit = -1;
            payload.FetchPlan = _fetchPlan;
            var paramsDictionary = new Dictionary<string, object>();
            for(var i = 0; i < _parameters.Length; i++)
            {
                paramsDictionary.Add(i.ToString(), _parameters[i]);
            }
            var pe = new ParametersEntity() { parameters = paramsDictionary };          
            pe.OClassName = string.Empty;
            payload.SerializedParams = _serializer.Serialize(pe);

            Request request = new Request(OperationMode.Synchronous, sessionId);
            //base.Request(request);

            request.AddDataItem((byte)OperationType.COMMAND);
            request.AddDataItem(request.SessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(token);
            }

            // operation specific fields
            request.AddDataItem((byte)request.OperationMode);

            // idempotent command (e.g. select)
            var queryPayload = payload;
            if (queryPayload != null)
            {
                // Write command payload length
                request.AddDataItem(queryPayload.PayLoadLength);
                request.AddDataItem(queryPayload.ClassName);
                //(text:string)(non-text-limit:int)[(fetch-plan:string)](serialized-params:bytes[])
                request.AddDataItem(queryPayload.Text);
                request.AddDataItem(queryPayload.NonTextLimit);
                request.AddDataItem(queryPayload.FetchPlan);

                if (queryPayload.SerializedParams == null || queryPayload.SerializedParams.Length == 0)
                {
                    request.AddDataItem((int)0);
                }
                else
                {
                    request.AddDataItem(queryPayload.SerializedParams);
                }
                return request;
            }
            // @todo Fix this to a better domain exception.
            throw new Exception("Need to fix this");
        }
    }
}
