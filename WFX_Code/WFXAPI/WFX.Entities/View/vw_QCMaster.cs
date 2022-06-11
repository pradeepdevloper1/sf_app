using System;

namespace WFX.Entities
{
    public class vw_QCMaster
    {
        public int FactoryID { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Line { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Color { get; set; }
        public string Customer { get; set; }
        public DateTime QCDate { get; set; }
        public int Qty { get; set; }
        public Int64 QCMasterId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string UserFirstName { get; set; }
        
    }
}
