using OrientDB.Net.Core.Models;

namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOCreateEdge
    {
        IOCreateEdge Edge(string className);
        IOCreateEdge Edge<T>(T obj);
        IOCreateEdge Edge<T>();       
        IOCreateEdge From(ORID orid);
        IOCreateEdge From<T>(T obj);
        IOCreateEdge To(ORID orid);
        IOCreateEdge To<T>(T obj);
        IOCreateEdge Set<T>(string fieldName, T fieldValue);
        IOCreateEdge Set<T>(T obj);        
        string ToString();
    }
}
