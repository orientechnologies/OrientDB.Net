using OrientDB.Net.Core.Models;

namespace OrientDB.Net.Serializers.RecordCSVSerializer.Models
{
    internal class OLinkCollection
    {
        internal int PageSize { get; set; }
        internal ORID Root { get; set; }
        internal int KeySize { get; set; }
    }
}
