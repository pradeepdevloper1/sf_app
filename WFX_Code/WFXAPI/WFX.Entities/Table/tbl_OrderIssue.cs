using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class tbl_OrderIssue
    {
        public long OrderIssueId { get; set; }
        public DateTime IssueDate { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Color { get; set; }
        public string Part { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string BatchNumber { get; set; }
        public long QCRequestID { get; set; }
        public string TabletID { get; set; }
        public decimal WFXStockGRNID { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        public string Line { get; set; }
        public string ToPONo {get; set;}
        public string ToLine { get; set; }
    }
}
