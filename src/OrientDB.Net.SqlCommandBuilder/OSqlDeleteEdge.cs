using OrientDB.Net.SqlCommandBuilder.Protocol;
using OrientDB.Net.SqlCommandBuilder.Extensions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Exceptions;

// syntax:
// DELETE EDGE <rid>|FROM <rid>|TO <rid>|<[<class>] 
// [WHERE <conditions>]> 
// [LIMIT <MaxRecords>]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class OSqlDeleteEdge
    {
        private SqlQuery _sqlQuery;

        public OSqlDeleteEdge()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Delete

        public OSqlDeleteEdge Delete(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public OSqlDeleteEdge Delete<T>(T obj)
        {
            _sqlQuery.DeleteEdge(obj);

            return this;
        }

        #endregion

        #region From

        public OSqlDeleteEdge From(ORID orid)
        {
            _sqlQuery.From(orid);

            return this;
        }

        public OSqlDeleteEdge From<T>(T obj)
        {
            DictionaryOrientDBEntity document;

            if (obj is OrientDBEntity)
            {
                document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }

            if (document.ORID == null)
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery.From(document.ORID);

            return this;
        }

        #endregion

        #region To

        public OSqlDeleteEdge To(ORID orid)
        {
            _sqlQuery.To(orid);

            return this;
        }

        public OSqlDeleteEdge To<T>(T obj)
        {
            DictionaryOrientDBEntity document;

            if (obj is OrientDBEntity)
            {
                document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }

            if (document.ORID == null)
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery.To(document.ORID);

            return this;
        }

        #endregion

        #region Class

        public OSqlDeleteEdge Class(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlDeleteEdge Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Where with conditions

        public OSqlDeleteEdge Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlDeleteEdge And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlDeleteEdge Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlDeleteEdge Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlDeleteEdge NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlDeleteEdge Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlDeleteEdge LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlDeleteEdge Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlDeleteEdge GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlDeleteEdge Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlDeleteEdge IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlDeleteEdge Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlDeleteEdge Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        public OSqlDeleteEdge Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }
        
        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.DeleteEdge);
        }
    }
}
