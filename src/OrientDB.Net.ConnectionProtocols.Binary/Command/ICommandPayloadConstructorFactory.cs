using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal interface ICommandPayloadConstructorFactory
    {
        ICommandPayload CreatePayload(string query, string fetchPlan, ConnectionMetaData metaData, params string[] parameters);
    }
}
