using System.Collections.Generic;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    internal class CommandResult<T>
    {
        public IEnumerable<T> Results { get; }

        public CommandResult(IEnumerable<T> results)
        {
            Results = results;
        }
    }
}