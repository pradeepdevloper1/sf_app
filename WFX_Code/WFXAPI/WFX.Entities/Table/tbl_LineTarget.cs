using System;

namespace WFX.Entities
{
    public class tbl_LineTarget
    {
        public long LineTargetID { get; set; }
        public string Module { get; set; }
        public string Section { get; set; }
        public string Line { get; set; }
        public string Style { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Part { get; set; }
        public string Color { get; set; }
        public double SMV { get; set; }
        public double Operators { get; set; }
        public double Helpers { get; set; }
        public string ShiftName { get; set; }
        public double ShiftHours { get; set; }
        public DateTime Date { get; set; }
        public double PlannedEffeciency { get; set; }
        public int PlannedTarget { get; set; }
        public int UserID { get; set; }
        public DateTime EntryDate { get; set; }
        public string SizeList { get; set; }
        public int FactoryID { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }

    }
}
