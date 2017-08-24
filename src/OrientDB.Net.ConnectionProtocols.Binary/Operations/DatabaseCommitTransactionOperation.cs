using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using System.Collections.Generic;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.Core.Models;
using System.Reflection;
using System.Net.Sockets;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseCommitTransactionOperation : IOrientDBOperation<TransactionResult>
    {
        private readonly ConnectionMetaData _metaData;
        private readonly bool _useTransactionLog;
        private readonly IEnumerable<DatabaseTransactionRequest> _records;

        public DatabaseCommitTransactionOperation(IEnumerable<DatabaseTransactionRequest> records, ConnectionMetaData metaData, bool useTransactionLog)
        {
            _metaData = metaData;
            _useTransactionLog = useTransactionLog;
            _records = records;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            var request = new Request(OperationMode.Synchronous, sessionId);

            request.AddDataItem((byte)OperationType.TX_COMMIT);
            request.AddDataItem(sessionId);

            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {
                request.AddDataItem(token);
            }

            int transactionId = 1;
            request.AddDataItem(transactionId);
            request.AddDataItem((byte)(_useTransactionLog ? 1 : 0));

            foreach(var item in _records)
            {
                item.AddToRequest(request);
            }

            request.AddDataItem(byte.MinValue);
            request.AddDataItem("");

            return request;
        }        

        public TransactionResult Execute(BinaryReader reader)
        {
            if (_metaData.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
                ReadToken(reader);

            var createdRecordMapping = new Dictionary<ORID, ORID>();
            int recordCount = reader.ReadInt32EndianAware();
            for (int i = 0; i < recordCount; i++)
            {
                var tempORID = ReadORID(reader);
                var realORID = ReadORID(reader);
                createdRecordMapping.Add(tempORID, realORID);
            }

            TransactionResult result = new TransactionResult();
            result.CreatedRecordMapping = createdRecordMapping;

            int updatedCount = reader.ReadInt32EndianAware();
            var updateRecordVersions = new Dictionary<ORID, int>();
            for (int i = 0; i < updatedCount; i++)
            {
                var orid = ReadORID(reader);
                var newVersion = reader.ReadInt32EndianAware();
                updateRecordVersions.Add(orid, newVersion);
            }

            result.UpdatedRecordVersions = updateRecordVersions;

            // Work around differents in storage type < version 2.0
            if (_metaData.ProtocolVersion >= 28 || (_metaData.ProtocolVersion >= 20 && _metaData.ProtocolVersion <= 27 && !EndOfStream(reader)))
            {
                int collectionChanges = reader.ReadInt32EndianAware();
                if (collectionChanges > 0)
                    throw new NotSupportedException("Processing of collection changes is not implemented. Failing rather than ignoring potentially significant data");
            }

            return result;
        }

        private bool EndOfStream(BinaryReader reader)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var length = (Int32)typeof(NetworkStream).GetTypeInfo().GetField("_readLen", flags).GetValue(reader.BaseStream);
            var pos = (Int32)typeof(NetworkStream).GetTypeInfo().GetField("_readPos", flags).GetValue(reader.BaseStream);
            return length == pos;
        }

        private ORID ReadORID(BinaryReader reader)
        {
            ORID result = new ORID();
            result.ClusterId = reader.ReadInt16EndianAware();
            result.ClusterPosition = reader.ReadInt64EndianAware();
            return result;
        }

        internal byte[] ReadToken(BinaryReader reader)
        {
            var size = reader.ReadInt32EndianAware();
            var token = reader.ReadBytesRequired(size);

            // if token renewed
           // if (token.Length > 0)
               // _database.GetConnection().Token = token;

            return token;
        }
    }
}
