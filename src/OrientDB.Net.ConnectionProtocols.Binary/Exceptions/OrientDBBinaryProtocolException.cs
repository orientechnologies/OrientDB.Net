using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrientDB.Net.ConnectionProtocols.Binary.Exceptions
{
    public class OrientDBBinaryProtocolException : Exception
    {
        public OrientDBBinaryProtocolExceptionType Type { get; }

        public OrientDBBinaryProtocolException()
        {

        }

        public OrientDBBinaryProtocolException(OrientDBBinaryProtocolExceptionType type, string message) : base(message)
        {
            Type = type;
        }

        public OrientDBBinaryProtocolException(OrientDBBinaryProtocolExceptionType type, string message, Exception innerException) : base(message, innerException)
        {
            Type = type;
        }
    }
}
