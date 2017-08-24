using OrientDB.Net.ConnectionProtocols.Binary.Core;

namespace OrientDB.Net.ConnectionProtocols.Binary.Constants
{
    internal class DriverConstants
    {
        public static string ClientID { get; internal set; }
        public static string DriverName { get; internal set; }
        public static string DriverVersion { get; internal set; }
        public static short ProtocolVersion { get; internal set; }
        public static RecordFormat RecordFormat { get; internal set; }

        static DriverConstants()
        {
            ProtocolVersion = 36;
            DriverName = "OrientDB-NET.binary";
            DriverVersion = "0.2.2";
            ClientID = "null";
            RecordFormat = RecordFormat.ORecordDocument2csv;
        }
    }
}
