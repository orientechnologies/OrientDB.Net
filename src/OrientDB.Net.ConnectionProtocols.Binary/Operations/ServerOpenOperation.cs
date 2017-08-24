using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class ServerOpenOperation : IOrientDBOperation<OpenServerResult>
    {
        private readonly ConnectionMetaData _connectionMetaData;
        private readonly ServerConnectionOptions _options;

        public ServerOpenOperation(ServerConnectionOptions _options, ConnectionMetaData connectionMetaData)
        {
            this._options = _options;
            this._connectionMetaData = connectionMetaData;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Synchronous);

            // standard request fields
            request.AddDataItem((byte)OperationType.CONNECT);
            request.AddDataItem(request.SessionId);

            // operation specific fields
            if (DriverConstants.ProtocolVersion > 7)
            {
                request.AddDataItem(DriverConstants.DriverName);
                request.AddDataItem(DriverConstants.DriverVersion);
                request.AddDataItem(DriverConstants.ProtocolVersion);
                request.AddDataItem(DriverConstants.ClientID);
            }
            if (DriverConstants.ProtocolVersion > 21)
            {
                request.AddDataItem(DriverConstants.RecordFormat.ToString());
            }

            if (DriverConstants.ProtocolVersion > 26)
            {
                request.AddDataItem((byte)(_connectionMetaData.UseTokenBasedSession ? 1 : 0)); // Use Token Session 0 - false, 1 - true
            }
            if (DriverConstants.ProtocolVersion >= 34)
            {
                request.AddDataItem((byte)0);// Support Push
                request.AddDataItem((byte)1);//Support collect-stats
            }

            request.AddDataItem(_options.UserName);
            request.AddDataItem(_options.Password);

            return request;
        }

        public OpenServerResult Execute(BinaryReader reader)
        {
            OpenServerResult result = new OpenServerResult();

            var sessionId = reader.ReadInt32EndianAware();
            result.SessionId = sessionId;

            if (_connectionMetaData.ProtocolVersion > 26)
            {
                var size = reader.ReadInt32EndianAware();
                var token = reader.ReadBytesRequired(size);
                
                result.Token = token;
            }

            return result;
        }
    }
}
