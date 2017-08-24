using OrientDB.Net.Core.Models;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// syntax: 
// UPDATE <class>|cluster:<cluster>> 
// SET|INCREMENT [= <field-value>](<field-name>)[<field-name> = <field-value>](,)* 
// [<conditions>] (WHERE) 
// [<max-records>](LIMIT)

// collections: 
// UPDATE <class>|cluster:<cluster>> 
// [[<field-name> = <field-value>](ADD|REMOVE])[<field-name> = <field-value>](,)* 
// [<conditions>](WHERE)

// maps:
// UPDATE <class>|cluster:<cluster>> 
// [[<field-name> = <map-key> [,<map-value>]](PUT|REMOVE])[<field-name> = <map-key> [,<map-value>]](,)* 
// [<conditions>](WHERE)

namespace OrientDB.Net.SqlCommandBuilder
{
    public class SqlUpdate
    {
        private SqlQuery _sqlQuery;

        public SqlUpdate()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Update

        public SqlUpdate Update(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public SqlUpdate Update<T>(T obj)
        {
            _sqlQuery.Update(obj);

            return this;
        }

        #endregion

        #region Class

        public SqlUpdate Class(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public SqlUpdate Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public SqlUpdate Cluster(string clusterName)
        {
            _sqlQuery.Cluster("cluster:" + clusterName);

            return this;
        }

        public SqlUpdate Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Record

        public SqlUpdate Record(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public SqlUpdate Record(OrientDBEntity document)
        {
            return Record(document.ORID);
        }

        #endregion

        #region Set

        public SqlUpdate Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public SqlUpdate Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        public SqlUpdate Add<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Add(fieldName, fieldValue);

            return this;
        }

        #region Remove

        public SqlUpdate Remove(string fieldName)
        {
            _sqlQuery.Remove(fieldName);

            return this;
        }

        public SqlUpdate Remove<T>(string fieldName, T collectionValue)
        {
            _sqlQuery.Remove(fieldName, collectionValue);

            return this;
        }

        #endregion

        #region Where with conditions

        public SqlUpdate Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public SqlUpdate And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public SqlUpdate Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public SqlUpdate Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public SqlUpdate NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public SqlUpdate Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public SqlUpdate LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public SqlUpdate Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public SqlUpdate GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public SqlUpdate Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public SqlUpdate IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public SqlUpdate Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public SqlUpdate Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        public SqlUpdate Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }

        #region Upsert

        public SqlUpdate Upsert()
        {
            _sqlQuery.Upsert();

            return this;
        }

        #endregion

      

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Update);
        }
    }
}
