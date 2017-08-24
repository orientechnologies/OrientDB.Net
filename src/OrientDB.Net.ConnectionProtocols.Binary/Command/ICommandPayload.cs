using OrientDB.Net.ConnectionProtocols.Binary.Core;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal interface ICommandPayload
    {
        Request CreatePayloadRequest(int sessionId, byte[] token);
    }
}
