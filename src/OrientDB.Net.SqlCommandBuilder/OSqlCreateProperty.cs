using OrientDB.Net.Core.Models;
using OrientDB.Net.SqlCommandBuilder.Protocol;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class OSqlCreateProperty
    {
        private SqlQuery _sqlQuery;
        private string _propertyName;
        private string _class;
        private OrientType _type;

        internal OSqlCreateProperty()
        {
            _sqlQuery = new SqlQuery();
        }
        public OSqlCreateProperty Property(string propertyName, OrientType type)
        {
            _propertyName = propertyName;
            _type = type;
            _sqlQuery.Property(_propertyName, _type);
            return this;
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateProperty);
        }

        public OSqlCreateProperty Class(string @class)
        {
            _class = @class;
            _sqlQuery.Class(_class);
            return this;
        }

        public OSqlCreateProperty LinkedType(OrientType type)
        {
            _sqlQuery.LinkedType(type);
            return this;
        }

        public OSqlCreateProperty LinkedClass(string @class)
        {
            _sqlQuery.LinkedClass(@class);
            return this;
        }
    }
}
