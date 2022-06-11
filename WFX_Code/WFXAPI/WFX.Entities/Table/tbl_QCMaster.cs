using System;
using System.Collections.Generic;

namespace WFX.Entities
{
    public class tbl_QCMaster
    {
        public long QCMasterId { get; set; }
        public DateTime QCDate { get; set; }
        public int TypeOfWork { get; set; }
        public string PONo { get; set; }
        public string Color { get; set; }
        public string Part { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int QCStatus { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string ShiftName { get; set; }
        public string Module { get; set; }
        public string ProcessName {get; set; }
        public string Line { get; set; }
        public string SONo { get; set; }
        public decimal WFXStockGRNID { get; set; }
        public string BatchNumber { get; set; }
        public long QCRequestID { get; set; }
        public string TabletID { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        public string ProcessCode { get; set; }
        public virtual ICollection<tbl_QCDefectDetails> tbl_QCDefectDetails { get; set; }
    }
}
