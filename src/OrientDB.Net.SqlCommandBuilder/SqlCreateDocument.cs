using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// shorthand for INSERT INTO for documents

namespace OrientDB.Net.SqlCommandBuilder
{
    public class SqlCreateDocument : IOCreateDocument
    {
        private SqlQuery _sqlQuery;

        private SqlCreateDocument()
        {
            _sqlQuery = new SqlQuery();
        }

        public static IOCreateDocument CreateSqlCreate(string className = null)
        {
            if (string.IsNullOrWhiteSpace(className))
                return new SqlCreateDocument();
            return new SqlCreateDocument().Document(className);
        }

        #region Document

        public IOCreateDocument Document(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public IOCreateDocument Document<T>(T obj)
        {
            // check for OClassName shouldn't have be here since INTO clause might specify it

            _sqlQuery.Insert(obj);

            return this;
        }

        public IOCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public IOCreateDocument Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public IOCreateDocument Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public IOCreateDocument Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public IOCreateDocument Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Insert);
        }
    }
}
