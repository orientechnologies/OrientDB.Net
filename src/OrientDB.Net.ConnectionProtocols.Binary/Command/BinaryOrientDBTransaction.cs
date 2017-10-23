using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.Collections.Generic;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using OrientDB.Net.Core.Exceptions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using System.Linq;
using System;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    public class BinaryOrientDBTransaction : IOrientDBTransaction
    {
        private readonly OrientDBBinaryConnectionStream _stream;
        private readonly Dictionary<ORID, DatabaseTransactionRequest> _records = new Dictionary<ORID, DatabaseTransactionRequest>();
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;
        private readonly ConnectionMetaData _metaData;
        private readonly Func<string, short> _clusterIdResolver;

        public BinaryOrientDBTransaction(OrientDBBinaryConnectionStream stream, IOrientDBRecordSerializer<byte[]> serializer, 
            ConnectionMetaData metaData, Func<string, short> clusterIdResolver)
        {
            _stream = stream;
            _serializer = serializer;
            _metaData = metaData;
            _clusterIdResolver = clusterIdResolver;
        }

        public void AddEntity<T>(T entity) where T : OrientDBEntity
        {
            var record = new DatabaseTransactionRequest(TransactionRecordType.Create, entity, _serializer);
            AddToRecords(record);   
        }

        private void AddToRecords(DatabaseTransactionRequest record)
        {
            bool hasOrid = record.RecordORID != null;
            bool needsOrid = record.RecordType != TransactionRecordType.Create;

            if(!hasOrid)
            {
                record.RecordORID = ORID.NewORID();
                string className = string.IsNullOrEmpty(record.EntityClassName) ? record.EntityName : record.EntityClassName;
                record.RecordORID.ClusterId = _clusterIdResolver(className);
            }

            if (_records.ContainsKey(record.RecordORID))
            {
                if (record.RecordType != _records[record.RecordORID].RecordType)
                    throw new OrientDBException(OrientDBExceptionType.Query, "Record has already been added as part of another operation within this transaction."); // Fix the Exception Type.
                _records[record.RecordORID] = record;
            }
            else
                _records.Add(record.RecordORID, record);
        }

        public void Commit()
        {
            TransactionResult tranResult = _stream.Send(new DatabaseCommitTransactionOperation(_records.Values, _metaData, true));

            var survivingRecords = _records.Values.Where(r => r.RecordType != TransactionRecordType.Delete).ToList();

            foreach (var kvp in tranResult.CreatedRecordMapping)
            {
                var record = _records.First(n => n.Key.ToString() == kvp.Key.ToString()).Value;
                record.RecordORID = kvp.Value;
                _records.Add(record.RecordORID, record);                
            }

            var versions = tranResult.UpdatedRecordVersions;
            foreach (var kvp in versions)
            {
                var record = _records[kvp.Key];
                record.Version = kvp.Value;
            }

            Reset();
        }

        public void Reset()
        {
            _records.Clear();
        }

        public void Remove<T>(T entity) where T : OrientDBEntity
        {
            var record = new DatabaseTransactionRequest(TransactionRecordType.Delete, entity, _serializer);
            AddToRecords(record);
        }

        public void Update<T>(T entity) where T : OrientDBEntity
        {
            var record = new DatabaseTransactionRequest(TransactionRecordType.Update, entity, _serializer);
            AddToRecords(record);
        }

        public void AddEdge(Edge edge, Vertex from, Vertex to)
        {
            AddEntity(edge);
            edge.SetField("out", from.ORID);
            edge.SetField("in", to.ORID);

            AppendORIDToField(from, $"out_{edge.OClassName}", edge.ORID);
            AppendORIDToField(to, $"in_{edge.OClassName}", edge.ORID);

            if (!_records.ContainsKey(from.ORID))
                Update(from);
            if (!_records.ContainsKey(to.ORID))
                Update(to);
        }        

        private void AppendORIDToField(DictionaryOrientDBEntity entity, string field, ORID orid)
        {
            if(entity.Fields.Keys.Contains(field))
            {
                entity.SetField(field, entity.GetField<HashSet<ORID>>(field).Add(orid));
            }
            else
            {
                var oridHashSet = new HashSet<ORID>();
                oridHashSet.Add(orid);
                entity.SetField(field, oridHashSet);
            }
        }
    }
}
