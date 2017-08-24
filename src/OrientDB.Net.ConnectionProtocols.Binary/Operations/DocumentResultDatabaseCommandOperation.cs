using Operations;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Command;
using System.Reflection;
using OrientDB.Net.Core.Abstractions;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using System.Collections.Generic;
using OrientDB.Net.Core.Models;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DocumentResultDatabaseCommandOperation : IOrientDBOperation<DocumentResult>
    {
        private readonly string _fetchPlan;
        private ConnectionMetaData _connectionMetaData;
        private string _query;
        private ICommandPayloadConstructorFactory _payloadFactory;
        private IOrientDBRecordSerializer<byte[]> _serializer;

        public DocumentResultDatabaseCommandOperation(ICommandPayloadConstructorFactory payloadFactory, ConnectionMetaData metaData, IOrientDBRecordSerializer<byte[]> serializer, string query, string fetchPlan = "*:0")
        {
            if (payloadFactory == null)
                throw new ArgumentNullException($"{nameof(payloadFactory)} cannot be null.");
            if (metaData == null)
                throw new ArgumentNullException($"{nameof(metaData)} cannot be null.");
            if (serializer == null)
                throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException($"{nameof(query)} cannot be zero length or null.");

            _payloadFactory = payloadFactory;
            _connectionMetaData = metaData;
            _serializer = serializer;
            _query = query;
            _fetchPlan = fetchPlan;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            return _payloadFactory.CreatePayload(_query, _fetchPlan, _connectionMetaData).CreatePayloadRequest(sessionId, token);
        }

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

        public DocumentResult Execute(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            if (_connectionMetaData.ProtocolVersion > 26 && _connectionMetaData.UseTokenBasedSession)
                ReadToken(reader);

            PayloadStatus payloadStatus = (PayloadStatus)reader.ReadByte();

            List<DictionaryOrientDBEntity> documents = new List<DictionaryOrientDBEntity>();

            int contentLength;

            switch (payloadStatus)
            {
                case PayloadStatus.NullResult: // 'n'
                                               // nothing to do
                    break;
                case PayloadStatus.SingleRecord: // 'r'
                    DictionaryOrientDBEntity document = ParseDocument(reader);
                    documents.Add(document);
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
                    DictionaryOrientDBEntity sDocument = ParseDocument(reader);
                    documents.Add(sDocument);
                    break;
                default:
                    break;
            }

            if (_connectionMetaData.ProtocolVersion >= 17)
            {
                //Load the fetched records in cache
                while ((payloadStatus = (PayloadStatus)reader.ReadByte()) != PayloadStatus.NoRemainingRecords)
                {
                    DictionaryOrientDBEntity document = ParseDocument(reader);

                    if (document != null && payloadStatus == PayloadStatus.PreFetched)
                    {
                        documents.Add(document);
                        //Put in the client local cache
                        //response.Connection.Database.ClientCache[document.ORID] = document;
                    }
                }
            }

            return new DocumentResult(documents);
        }

        protected bool EndOfStream(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            return !(bool)reader.BaseStream.GetType().GetProperty("DataAvailable").GetValue(reader.BaseStream);
        }

        private DictionaryOrientDBEntity ParseDocument(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");

            DictionaryOrientDBEntity document;

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

                document = Activator.CreateInstance<DictionaryOrientDBEntity>();
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

                document = _serializer.Deserialize<DictionaryOrientDBEntity>(rawRecord);
                document.ORID = orid;
                document.OVersion = version;
                document.OClassId = classId;
            }

            return document;
        }
    }
}
