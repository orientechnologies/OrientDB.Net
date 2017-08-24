
namespace OrientDB.Net.SqlCommandBuilder.Protocol
{
    internal enum QueryType
    {
        CreateClass,
        CreateProperty,
        CreateCluster,
        CreateEdge,
        CreateVertex,
        DeleteVertex,
        DeleteEdge,
        DeleteDocument,
        Insert,
        Select,
        Update
    }
}
