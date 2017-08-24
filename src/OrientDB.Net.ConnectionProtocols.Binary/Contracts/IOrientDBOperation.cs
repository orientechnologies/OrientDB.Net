using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;

namespace OrientDB.Net.ConnectionProtocols.Binary.Contracts
{
    internal interface IOrientDBRequest
    {
        Request CreateRequest(int sessionId, byte[] token);
    }

    internal interface IOrientDBOperation<T> : IOrientDBRequest
    {        
        T Execute(BinaryReader reader);
    }
}
