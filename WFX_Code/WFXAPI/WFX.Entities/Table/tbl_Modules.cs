using System;

namespace WFX.Entities
{
    public class tbl_Modules
    {
        public int ModuleID { get; set; }
        public int DepartmentID { get; set; }
        public string ModuleName { get; set; }
        public int FactoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
