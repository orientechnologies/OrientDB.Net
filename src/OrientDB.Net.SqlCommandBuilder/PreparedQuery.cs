using System;
using System.Collections.Generic;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class PreparedQuery
    {
        private string _query;
        private string _fetchPlan;
        private Dictionary<string, object> _parameters;

        public PreparedQuery(string query, string fetchPlan = "*:0")
        {
            _query = query;
            _fetchPlan = fetchPlan;
        }

        public override string ToString()
        {
            return _query;
        }

        public PreparedQuery Set(string key, object value)
        {
            if (_parameters == null)
                _parameters = new Dictionary<string, object>();

            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            _parameters.Add(key, value);

            return this;
        }
    }
}
