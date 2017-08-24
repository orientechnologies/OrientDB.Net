using OrientDB.Net.Core.Models;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class ORecordMetadata
    {
        private ORID _orid;

        internal ORecordMetadata()
        {
            
        }

        public ORecordMetadata ORID(ORID orid)
        {
            _orid = orid;
            return this;
        }
    }
}
