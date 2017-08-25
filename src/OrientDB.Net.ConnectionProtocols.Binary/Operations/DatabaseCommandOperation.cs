using OrientDB.Net.ConnectionProtocols.Binary.Command;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using Microsoft.Extensions.Logging;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseCommandOperation<T> : IOrientDBOperation<CommandResult<T>> where T : OrientDBEntity
    {
        private readonly string _query;
        private readonly string _fetchPlan;
        private readonly ICommandPayloadConstructorFactory _payloadFactory;
        private readonly ConnectionMetaData _metaData;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;
        private readonly ILogger _logger;

        public DatabaseCommandOperation(ICommandPayloadConstructorFactory payloadFactory, ConnectionMetaData metaData, IOrientDBRecordSerializer<byte[]> serializer, ILogger logger, string query, string fetchPlan = "*:0")
        {
            _fetchPlan = fetchPlan;
            _payloadFactory = payloadFactory ?? throw new ArgumentNullException($"{nameof(payloadFactory)} cannot be null.");
            _metaData = metaData ?? throw new ArgumentNullException($"{nameof(metaData)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException($"{nameof(query)} cannot be zero length or null.");
            _query = query;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            return _payloadFactory.CreatePayload(_query, _fetchPlan, _metaData).CreatePayloadRequest(sessionId, token);
        }

        // Will need to create base class for this.
        internal byte[] ReadToken(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            var size = reader.ReadInt32EndianAware();
            var token = reader.ReadBytesRequired(size);

            // if token renewed
            if (token.Length > 0) { } // Temp, not ready yet.
                //_database.GetConnection().Token = token;

            return token;
        }

        public CommandResult<T> Execute(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            if (_metaData.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
                ReadToken(reader);

            PayloadStatus payloadStatus = (PayloadStatus)reader.ReadByte();

            List<T> documents = new List<T>();

            int contentLength;

            switch (payloadStatus)
            {
                case PayloadStatus.NullResult: // 'n'
                                               // nothing to do
                    break;
                case PayloadStatus.SingleRecord: // 'r'
                    T document = ParseDocument(reader);
                    break;
                case PayloadStatus.SerializedResult: // 'a'
                    contentLength = reader.ReadInt32EndianAware();
                    byte[] serializedBytes = reader.ReadBytes(contentLength);
                    string serialized = System.Text.Encoding.UTF8.GetString(serializedBytes, 0, serializedBytes.Length);
                    
                    break;
                case PayloadStatus.RecordCollection: // 'l'                   

                    int recordsCount = reader.ReadInt32EndianAware();

                    for (int i = 0; i < recordsCount; i++)
                    {
                        documents.Add(ParseDocument(reader));
                    }
                    break;
                case PayloadStatus.SimpleResult: //'w'
                    T sDocument = ParseDocument(reader);
                    documents.Add(sDocument);
                    break;
                default:
                    break;
            }

            if (_metaData.ProtocolVersion >= 17)
            {
                //Load the fetched records in cache
                while ((payloadStatus = (PayloadStatus)reader.ReadByte()) != PayloadStatus.NoRemainingRecords)
                {
                    T document = ParseDocument(reader);
                    
                    if (document != null && payloadStatus == PayloadStatus.PreFetched)
                    {
                        documents.Add(document);
                        //Put in the client local cache
                        //response.Connection.Database.ClientCache[document.ORID] = document;
                    }
                }
            }

            return new CommandResult<T>(documents);
        }

        private T ParseDocument(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            T document;

            short classId = reader.ReadInt16EndianAware();

            if (classId == -2) // NULL
            {
                document = null;
            }
            else if (classId == -3) // record id
            {
                ORID orid = new ORID();
                orid.ClusterId = reader.ReadInt16EndianAware();
                orid.ClusterPosition = reader.ReadInt64EndianAware();

                document = Activator.CreateInstance<T>();
                document.ORID = orid;
                document.OClassId = classId;
            }
            else
            {
                ORecordType type = (ORecordType)reader.ReadByte();

                ORID orid = new ORID();
                orid.ClusterId = reader.ReadInt16EndianAware();
                orid.ClusterPosition = reader.ReadInt64EndianAware();
                int version = reader.ReadInt32EndianAware();
                int recordLength = reader.ReadInt32EndianAware();
                byte[] rawRecord = reader.ReadBytes(recordLength);

                document = _serializer.Deserialize<T>(rawRecord);
                document.ORID = orid;
                document.OVersion = version;
                document.OClassId = classId;
            }

            return document;
        }
    }
}
