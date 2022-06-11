using System;

namespace WFX.Entities
{
    public class tbl_Clusters
    {
        public int ClusterID { get; set; }
        public int OrganisationID { get; set; }
        public string ClusterName { get; set; }
        public string ClusterHead { get; set; }
        public string ClusterEmail { get; set; }
        public string ClusterRegion { get; set; }
    }
}
