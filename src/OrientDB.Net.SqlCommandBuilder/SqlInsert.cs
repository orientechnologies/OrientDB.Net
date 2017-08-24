using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// syntax:
// INSERT INTO <Class>|cluster:<cluster>|index:<index> 
// [<cluster>](cluster) 
// [VALUES (<expression>[,]((<field>[,]*))*)]|[<field> = <expression>[,](SET)*]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class SqlInsert : IOInsert
    {
        private SqlQuery _sqlQuery;

        private SqlInsert()
        {
            _sqlQuery = new SqlQuery();
        }

        public static IOInsert CreateSqlInsert(string className = null)
        {
            if (string.IsNullOrWhiteSpace(className))
                return new SqlInsert();
            return new SqlInsert().Into(className);
        }

        #region Insert

        public IOInsert Insert(string className)
        {
            return Into(className);
        }

        public IOInsert Insert<T>()
        {
            return Into<T>();
        }

        public IOInsert Insert<T>(T obj)
        {
            // check for OClassName shouldn't have be here since INTO clause might specify it

            _sqlQuery.Insert(obj);

            return this;
        }

        #endregion

        #region Into

        public IOInsert Into(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public IOInsert Into<T>()
        {
            Into(typeof(T).Name);

            return this;
        }

        #endregion

        #region Cluster

        public IOInsert Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public IOInsert Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public IOInsert Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public IOInsert Set<T>(T obj)
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
