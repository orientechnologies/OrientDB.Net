using System.Collections.Concurrent;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class ResultRow
    {
        private readonly ConcurrentDictionary<string, object> _properties = new ConcurrentDictionary<string, object>();

        public ResultRow()
        {

        }

        public T Get<T>(string field)
        {
            return (T)_properties[field];
        }

        internal void Set<T>(string field, T value)
        {
            _properties.AddOrUpdate(field, value, (key, val) => _properties[key] = val);
        }
    }
}
