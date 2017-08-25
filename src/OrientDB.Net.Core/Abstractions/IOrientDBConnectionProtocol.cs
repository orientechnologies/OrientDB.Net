using Microsoft.Extensions.Logging;
using System;

namespace OrientDB.Net.Core.Abstractions
{
    public interface IOrientDBConnectionProtocol<TDataType> : IDisposable
    {
        IOrientServerConnection CreateServerConnection(IOrientDBRecordSerializer<TDataType> serializer, ILogger logger);
    }
}
