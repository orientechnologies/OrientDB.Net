using System.Collections.Generic;
using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Extensions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Exceptions;

// syntax: 
// CREATE VERTEX [<class>] 
// [CLUSTER <cluster>] 
// [SET <field> = <expression>[,]*]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class ORecordCreateVertex : IOCreateVertex
    {
        private DictionaryOrientDBEntity _document;

        public ORecordCreateVertex()
        {
        }

        #region Vertex

        public IOCreateVertex Vertex(string className)
        {
            if (_document == null)
                _document = new DictionaryOrientDBEntity();

            _document.OClassName = className;

            return this;
        }

        public IOCreateVertex Vertex<T>(T obj)
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

        public IOCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        #endregion      

        #region Set

        public IOCreateVertex Set<T>(string fieldName, T fieldValue)
        {
            _document.SetField(fieldName, fieldValue);

            return this;
        }

        public IOCreateVertex Set<T>(T obj)
        {
            var document = obj is OrientDBEntity ? (obj as OrientDBEntity).ToDictionaryOrientDBEntity() : OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);

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
    }
}
