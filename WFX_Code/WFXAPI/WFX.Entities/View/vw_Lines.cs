using System;

namespace WFX.Entities
{
    public class vw_Lines
    {
        public int LineID { get; set; }
        public int ModuleID { get; set; }
        public string LineName { get; set; }
        public string InternalLineName { get; set; }
        public int NoOfMachine { get; set; }
        public int LineCapacity { get; set; }
        public string LineloadingPoint { get; set; }
        public string TabletID { get; set; }

        public string ModuleName { get; set; }
        public int FactoryID { get; set; }
        public string FactoryName { get; set; }
    }
}
