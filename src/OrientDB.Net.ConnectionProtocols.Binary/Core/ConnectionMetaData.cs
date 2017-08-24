namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class ConnectionMetaData
    {
        public int ProtocolVersion { get; internal set; }
        public int OrientRelease { get; internal set; }
        public int ClusterCount { get; internal set; }
        public string ClusterConfig { get; internal set; }
        public bool UseTokenBasedSession { get; internal set; }
    }
}
