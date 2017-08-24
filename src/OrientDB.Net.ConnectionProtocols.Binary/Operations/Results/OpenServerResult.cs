namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    public class OpenServerResult
    {
        public int SessionId { get; set; }
        public byte[] Token { get; set; }
    }
}
