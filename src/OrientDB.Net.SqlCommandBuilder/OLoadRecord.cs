using OrientDB.Net.Core.Models;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class OLoadRecord
    {
        private ORID _orid;
        private string _fetchPlan = string.Empty;

        internal OLoadRecord()
        {
            
        }

        public OLoadRecord ORID(ORID orid)
        {
            _orid = orid;
            return this;
        }

        public OLoadRecord FetchPlan(string plan)
        {
            _fetchPlan = plan;
            return this;
        }
    }
}
