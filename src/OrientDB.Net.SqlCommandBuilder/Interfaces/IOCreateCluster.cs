using OrientDB.Net.Core.Models;

namespace OrientDB.Net.SqlCommandBuilder.Interfaces
{
    public interface IOCreateCluster
    {
        IOCreateCluster Cluster(string clusterName, ClusterType clusterType);
        IOCreateCluster Cluster<T>(ClusterType clusterType);   
        string ToString();
    }
}
