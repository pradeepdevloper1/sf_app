using System;

namespace WFX.Entities
{
    public class vw_Culster
    {
        public int ClusterID { get; set; }
        public int OrganisationID { get; set; }
        public string ClusterName { get; set; }
        public string OrganisationName { get; set; }
        public string ClusterHead { get; set; }
        public string ClusterEmail { get; set; }
        public string ClusterRegion { get; set; }
        public int FactoryCount { get; set; }

    }
}
