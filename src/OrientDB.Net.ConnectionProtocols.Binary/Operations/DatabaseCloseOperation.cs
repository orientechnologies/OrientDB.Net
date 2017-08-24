using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using System.IO;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseCloseOperation : IOrientDBOperation<CloseDatabaseResult>
    {
        private readonly ConnectionMetaData _metaData;
        private readonly byte[] _connectionToken;

        public DatabaseCloseOperation(byte[] token, ConnectionMetaData metaData)
        {
            _connectionToken = token;
            _metaData = metaData;
        }

        internal byte[] ReadToken(BinaryReader reader)
        {
            var size = reader.ReadInt32EndianAware();
            var token = reader.ReadBytesRequired(size);            

            return token;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Asynchronous, sessionId); // This may not be async actually.

            request.AddDataItem((byte)OperationType.DB_CLOSE);
            request.AddDataItem(request.SessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(_connectionToken);
            }

            return request;
        }

        public CloseDatabaseResult Execute(BinaryReader reader)
        {
            return new CloseDatabaseResult(true);
        }
    }
}