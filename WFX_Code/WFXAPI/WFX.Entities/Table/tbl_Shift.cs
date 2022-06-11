using System;

namespace WFX.Entities
{
    public class tbl_Shift
    {
        public int ShiftID { get; set; }
        public int ModuleID { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public int FactoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
