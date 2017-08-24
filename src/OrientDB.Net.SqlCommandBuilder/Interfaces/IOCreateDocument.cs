namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOCreateDocument
    {
        IOCreateDocument Cluster(string clusterName);
        IOCreateDocument Cluster<T>();
        IOCreateDocument Document(string className);
        IOCreateDocument Document<T>();
        IOCreateDocument Document<T>(T obj);        
        IOCreateDocument Set<T>(string fieldName, T fieldValue);
        IOCreateDocument Set<T>(T obj);
        string ToString();
    }
}
