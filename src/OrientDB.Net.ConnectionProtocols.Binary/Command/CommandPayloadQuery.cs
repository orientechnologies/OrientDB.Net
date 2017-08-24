using OrientDB.Net.ConnectionProtocols.Binary.Core;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    internal class CommandPayloadQuery
    {
        public CommandPayloadQuery()
        {
            ClassName = "q";
        }
        internal int NonTextLimit { get; set; }
        internal string FetchPlan { get; set; }
        internal byte[] SerializedParams { get; set; }
        internal int PayLoadLength
        {
            get
            {
                return sizeof(int) + BinarySerializer.Length(ClassName) +
                       sizeof(int) + BinarySerializer.Length(Text) +
                       sizeof(int) + //NonTextLimit +
                       sizeof(int) + BinarySerializer.Length(FetchPlan) +
                       sizeof(int) + (SerializedParams != null ? SerializedParams.Length : 0);
            }
        }
        public virtual string ClassName { get; protected set; }
        internal string Text { get; set; }
    }
}
