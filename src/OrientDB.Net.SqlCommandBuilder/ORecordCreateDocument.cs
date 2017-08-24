using System;
using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Extensions;
using OrientDB.Net.Core.Models;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class ORecordCreateDocument : IOCreateDocument
    {
        private DictionaryOrientDBEntity _document;

        public ORecordCreateDocument()
        {
            _document = new DictionaryOrientDBEntity();
        }
        
        public IOCreateDocument Cluster(string clusterName)
        {
            throw new NotImplementedException();
        }

        public IOCreateDocument Cluster<T>()
        {
            throw new NotImplementedException();
        }

        public IOCreateDocument Document(string className)
        {
            _document.OClassName = className;
            return this;
        }

        public IOCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        public IOCreateDocument Document<T>(T obj)
        {
            if (obj is OrientDBEntity)
            {
                _document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                _document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }
            return this;
        }

        public IOCreateDocument Set<T>(string fieldName, T fieldValue)
        {
            _document.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public IOCreateDocument Set<T>(T obj)
        {
            if (obj is OrientDBEntity)
            {
                _document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                _document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }

            return this;
        }
    }
}
