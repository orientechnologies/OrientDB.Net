using System;
namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOInsert
    {
        IOInsert Cluster(string clusterName);
        IOInsert Cluster<T>();
        IOInsert Insert(string className);
        IOInsert Insert<T>();
        IOInsert Insert<T>(T obj);
        IOInsert Into(string className);
        IOInsert Into<T>();     
        IOInsert Set<T>(string fieldName, T fieldValue);
        IOInsert Set<T>(T obj);
        string ToString();
    }
}
