using OrientDB.Net.Core.Models;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    public class TransactionResult
    {
        public IDictionary<ORID, ORID> CreatedRecordMapping { get; set; } = new Dictionary<ORID, ORID>();
        public IDictionary<ORID, int> UpdatedRecordVersions { get; set; } = new Dictionary<ORID, int>();
    }
}
