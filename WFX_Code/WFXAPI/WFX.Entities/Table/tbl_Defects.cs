using System;

namespace WFX.Entities
{
    public class tbl_Defects
    {
        public int DefectID { get; set; }
        public int DepartmentID { get; set; }
        public string DefectCode { get; set; }
        public string DefectName { get; set; }
        public string DefectType { get; set; }
        public string DefectLevel { get; set; }
        public int FactoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
