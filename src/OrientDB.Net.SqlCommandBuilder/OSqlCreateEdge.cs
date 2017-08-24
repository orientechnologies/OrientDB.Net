using OrientDB.Net.Core.Exceptions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.SqlCommandBuilder.Extensions;
using OrientDB.Net.SqlCommandBuilder.Interfaces;
using OrientDB.Net.SqlCommandBuilder.Protocol;

// syntax: 
// CREATE EDGE [<class>] 
// [CLUSTER <cluster>] 
// FROM <rid>|(<query>)|[<rid>]* 
// TO <rid>|(<query>)|[<rid>]* 
// [SET <field> = <expression>[,]*]

namespace OrientDB.Net.SqlCommandBuilder
{
    public class OSqlCreateEdge : IOCreateEdge
    {
        private SqlQuery _sqlQuery;

        public OSqlCreateEdge()
        {
            _sqlQuery = new SqlQuery();
        }

        #region Edge

        public IOCreateEdge Edge(string className)
        {
            _sqlQuery.Edge(className);

            return this;
        }

        public IOCreateEdge Edge<T>(T obj)
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

            string className = document.OClassName;

            if (typeof(T) == typeof(Edge))
            {
                className = "E";
            }
            else if (string.IsNullOrEmpty(document.OClassName))
            {
                throw new OrientDBException(OrientDBExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            _sqlQuery.Edge(className);
            _sqlQuery.Set(document);

            return this;
        }

        public IOCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public IOCreateEdge Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public IOCreateEdge Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region From

        public IOCreateEdge From(ORID orid)
        {
            _sqlQuery.From(orid);

            return this;
        }

        public IOCreateEdge From<T>(T obj)
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

        public IOCreateEdge To(ORID orid)
        {
            _sqlQuery.To(orid);

            return this;
        }

        public IOCreateEdge To<T>(T obj)
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

        #region Set

        public IOCreateEdge Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public IOCreateEdge Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion
        
        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateEdge);
        }
    }
}
