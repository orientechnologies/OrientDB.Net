using System.Collections.Generic;
using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Extensions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Exceptions;

namespace OrientDB.Net.SqlCommandBuilder
{
    class ORecordCreateEdge : IOCreateEdge
    {
        private DictionaryOrientDBEntity _document;
        private ORID _source;
        private ORID _dest;
        private string _edgeName;

        public ORecordCreateEdge()
        {

        }

        #region Edge

        public IOCreateEdge Edge(string className)
        {
            _edgeName = className;

            return this;
        }

        public IOCreateEdge Edge<T>(T obj)
        {

            if (obj is OrientDBEntity)
            {
                _document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                _document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }

            if (string.IsNullOrEmpty(_document.OClassName))
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            return this;
        }

        public IOCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        #endregion       

        #region Set

        public IOCreateEdge Set<T>(string fieldName, T fieldValue)
        {
            if (_document == null)
                _document = new DictionaryOrientDBEntity();
            _document.SetField(fieldName, fieldValue);

            return this;
        }

        public IOCreateEdge Set<T>(T obj)
        {
            var document = obj is OrientDBEntity ? obj as DictionaryOrientDBEntity : OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);

            // TODO: go also through embedded fields
            foreach (KeyValuePair<string, object> field in document.Fields)
            {
                // set only fields which doesn't start with @ character
                if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                {
                    Set(field.Key, field.Value);
                }
            }

            return this;
        }

        #endregion

        public IOCreateEdge From(ORID orid)
        {
            _source = orid;
            return this;
        }

        public IOCreateEdge From<T>(T obj)
        {
            _source = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj).ORID;
            return this;

        }

        public IOCreateEdge To(ORID orid)
        {
            _dest = orid;
            return this;
        }

        public IOCreateEdge To<T>(T obj)
        {
            _dest = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj).ORID;
            return this;
        }

        private static OrientDBEntity ToODocument<T>(T obj)
        {
            DictionaryOrientDBEntity document;

            if (obj is IDictionary<string, object>)
            {
                document = obj as DictionaryOrientDBEntity;
            }
            else
            {
                document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }

            if (document.ORID == null)
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain ORID value.");
            }
            return document;
        }
    }
}
