using OrientDB.Net.Core.Models;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    internal class DocumentResult
    {
        public IEnumerable<DictionaryOrientDBEntity> Results { get; }

        public DocumentResult(IEnumerable<DictionaryOrientDBEntity> results)
        {
            Results = results;
        }
    }
}
