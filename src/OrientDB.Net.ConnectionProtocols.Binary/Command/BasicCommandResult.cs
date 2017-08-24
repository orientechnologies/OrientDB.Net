using OrientDB.Net.Core.Abstractions;
using System;
using OrientDB.Net.Core.Models;
using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Command
{
    public class BasicCommandResult : IOrientDBCommandResult
    {
        public int RecordsAffected { get; set; }

        public IEnumerable<DictionaryOrientDBEntity> UpdatedRecords { get; set; }
    }
}
