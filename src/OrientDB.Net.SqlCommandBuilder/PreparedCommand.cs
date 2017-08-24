using System;
using System.Collections.Generic;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class PreparedCommand
    {
        private string _query;
        private Dictionary<string, object> _parameters;

        public PreparedCommand(string query)
        {
            _query = query;
        }

        public override string ToString()
        {
            return _query;
        }

        public PreparedCommand Set(string key, object value)
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
