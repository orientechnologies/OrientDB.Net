namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOCreateVertex
    {
        IOCreateVertex Vertex(string className);
        IOCreateVertex Vertex<T>(T obj);
        IOCreateVertex Vertex<T>();        
        IOCreateVertex Set<T>(string fieldName, T fieldValue);
        IOCreateVertex Set<T>(T obj);      
        string ToString();
    }
}
