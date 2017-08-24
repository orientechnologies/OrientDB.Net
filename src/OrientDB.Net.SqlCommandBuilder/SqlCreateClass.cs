using System;
using System.Linq;
using System.Reflection;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// syntax: 
// CREATE CLASS <class> 
// [EXTENDS <super-class>] 
// [CLUSTER <clusterId>*]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class SqlCreateClass
    {
        private SqlQuery _sqlQuery;
        private string _className;
        private Type _type;
        private bool _autoProperties;
        public SqlCreateClass()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Class

        public SqlCreateClass Class(string className)
        {
            _className = className;
            _sqlQuery.Class(_className);

            return this;
        }

        public SqlCreateClass Class<T>()
        {
            _type = typeof(T);
            _className = typeof(T).Name;
            return Class(_className);
        }

        public SqlCreateClass Class<T>(string className)
        {
            _type = typeof(T);
            _className = className;
            return Class(_className);
        }

        #endregion

        #region Extends

        public SqlCreateClass Extends(string superClass)
        {
            _sqlQuery.Extends(superClass);

            return this;
        }

        public SqlCreateClass CreateProperties()
        {
            if (_type == null)
                throw new InvalidOperationException("Can only create properties automatically when a generic type parameter has been specified");

            _autoProperties = true;
            return this;
        }

        public SqlCreateClass CreateProperties<T>()
        {
            if (_type != null && _type != typeof(T))
                throw new InvalidOperationException("Inconsistent type specified - type for CreateProperties<T> must match type for Class<T>");

            _type = typeof(T);

            _autoProperties = true;
            return this;
        }


        public SqlCreateClass Extends<T>()
        {
            return Extends(typeof(T).Name);
        }

        #endregion

        public SqlCreateClass Cluster(short clusterId)
        {
            _sqlQuery.Cluster(clusterId.ToString());

            return this;
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateClass);
        }
    }
}
