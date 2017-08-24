using OrientDB.Net.Core.Models;
using OrientDB.Net.SqlCommandBuilder.Interfaces;

namespace OrientDB.Net.SqlCommandBuilder
{
    public class ODataClasterAdd : IOCreateCluster
    {
        public string ClusterName { get; set; }
        public ClusterType ClusterType { get; set; }

        public ODataClasterAdd()
        {

        }

        public IOCreateCluster Cluster(string clusterName, ClusterType clusterType)
        {
            ClusterName = clusterName;
            ClusterType = clusterType;
            return this;
        }

        public IOCreateCluster Cluster<T>(ClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }
    }
}
