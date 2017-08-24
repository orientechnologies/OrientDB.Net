using OrientDB.Net.Core.Models;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class ClassSchema : OrientDBEntity
    {
        public string Name { get; private set; }
        public short DefaultClusterId { get; private set; }

        public override void Hydrate(IDictionary<string, object> data)
        {
            if (data.ContainsKey("defaultClusterId"))
                DefaultClusterId = short.Parse(data["defaultClusterId"].ToString());
            if (data.ContainsKey("name"))
                Name = data["name"].ToString();
        }
    }
}
