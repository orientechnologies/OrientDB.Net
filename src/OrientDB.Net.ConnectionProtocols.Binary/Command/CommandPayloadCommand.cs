using OrientDB.Net.ConnectionProtocols.Binary.Core;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    public class CommandPayloadCommand
    {
        public CommandPayloadCommand()
        {
            ClassName = "c";
        }
        internal byte[] SimpleParams { get; set; }
        internal byte[] ComplexParams { get; set; }
        public virtual string ClassName { get; protected set; }
        internal string Text { get; set; }
        internal int PayLoadLength
        {
            get
            {
                // TODO: Implement Simple Complex params               
                return
                    sizeof(int) + BinarySerializer.Length(ClassName)
                    + sizeof(int) + BinarySerializer.Length(Text)
                    + sizeof(byte) // Has SimpleParams 0 - false , 1 - true
                    + (SimpleParams != null ? sizeof(int) + SimpleParams.Length : 0)
                    + sizeof(byte) // Has ComplexParams 0 - false , 1 - true
                    + (ComplexParams != null ? sizeof(int) + ComplexParams.Length : 0);
            }
        }
    }
}
