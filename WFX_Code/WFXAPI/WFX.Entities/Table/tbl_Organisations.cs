using System;

namespace WFX.Entities
{
    public class tbl_Organisations
    {
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationAddress { get; set; }
        public string OrganisationLogoPath { get; set; }
        public string ERPAPIURL { get; set; }
    }
}
