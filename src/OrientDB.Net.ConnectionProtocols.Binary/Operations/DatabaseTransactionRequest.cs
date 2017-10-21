using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseTransactionRequest
    {
        private readonly OrientDBEntity _entity;
        private readonly TransactionRecordType _recordType;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;

        public DatabaseTransactionRequest(TransactionRecordType recordType, OrientDBEntity entity, IOrientDBRecordSerializer<byte[]> serializer)
        {
            _entity = entity;
            _recordType = recordType;
            _serializer = serializer;
        }

        public string EntityName
        {
            get { return _entity.GetType().Name; }
        }

        public string EntityClassName {
            get { return _entity.OClassName; }
        }

        public ORID RecordORID
        {
            get { return _entity.ORID; }
            set { _entity.ORID = value; }
        }

        public int Version
        {
            get { return _entity.OVersion; }
            set { _entity.OVersion = value; }
        }

        public TransactionRecordType RecordType
        {
            get { return _recordType; }
        }

        public Request AddToRequest(Request request)
        {
            request.AddDataItem((byte)1);
            request.AddDataItem((byte)RecordType);
            request.AddDataItem(RecordORID.ClusterId);
            request.AddDataItem(RecordORID.ClusterPosition);
            request.AddDataItem((byte)ORecordType.Document);

            var serializedDocument = _serializer.Serialize(_entity);
            switch(RecordType)
            {
                case TransactionRecordType.Create:
                    request.AddDataItem(serializedDocument);
                    break;
                case TransactionRecordType.Delete:
                    request.AddDataItem(Version);
                    break;
                case TransactionRecordType.Update:
                    request.AddDataItem(Version);
                    request.AddDataItem(serializedDocument);
                    if(DriverConstants.ProtocolVersion >= 23)
                    {
                        request.AddDataItem((byte)1);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return request;
        }
    }
}
