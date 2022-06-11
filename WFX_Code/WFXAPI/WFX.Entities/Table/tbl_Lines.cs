using System;

namespace WFX.Entities
{
    public class tbl_Lines
    {
        public int LineID { get; set; }
        public int ModuleID { get; set; }
        public string LineName { get; set; }
        public string InternalLineName { get; set; }
        public int NoOfMachine { get; set; }
        public int LineCapacity { get; set; }
        public string LineloadingPoint { get; set; }
        public string TabletID { get; set; }
        public int FactoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ModuleName { get; set; }
        public string ProcessType { get; set; }
        public string DeviceSerialNo { get; set; }

    }
}
