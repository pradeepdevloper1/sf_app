using System;

namespace WFX.Entities
{
    public class vw_Organisation
    {
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationAddress { get; set; }
        public string OrganisationLogoPath { get; set; }
   
        public int  ClusterCount { get; set; }
        public int FactoryCount { get; set; }

    }
}
