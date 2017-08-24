using OrientDB.Net.Core.Models;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// syntax:
// DELETE VERTEX <rid>|<[<class>] 
// [WHERE <conditions>] 
// [LIMIT <MaxRecords>>]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class OSqlDeleteVertex
    {
        private SqlQuery _sqlQuery;

        public OSqlDeleteVertex()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Delete

        public OSqlDeleteVertex Delete(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public OSqlDeleteVertex Delete<T>(T obj)
        {
            _sqlQuery.DeleteVertex(obj);

            return this;
        }

        #endregion

        #region Class

        public OSqlDeleteVertex Class(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlDeleteVertex Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Where with conditions

        public OSqlDeleteVertex Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlDeleteVertex And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlDeleteVertex Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlDeleteVertex Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlDeleteVertex NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlDeleteVertex Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlDeleteVertex LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlDeleteVertex Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlDeleteVertex GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlDeleteVertex Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlDeleteVertex IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlDeleteVertex Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlDeleteVertex Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        public OSqlDeleteVertex Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.DeleteVertex);
        }
    }
}
