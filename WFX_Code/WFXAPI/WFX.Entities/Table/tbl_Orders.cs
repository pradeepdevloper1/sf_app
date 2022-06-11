using System;

namespace WFX.Entities
{
    public class tbl_Orders
    {
        public long OrderID { get; set; }
        public string Module { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Season { get; set; }
        public string Customer { get; set; }
        public DateTime PlanStDt { get; set; }
        public DateTime ExFactory { get; set; }
        public int PrimaryPart { get; set; }
        public string Part { get; set; }
        public string Color { get; set; }
        public string Hexcode { get; set; }
        public string Fabric { get; set; }
        public string OrderRemark { get; set; }
        public int IsSizeRun { get; set; }
        public int POQty { get; set; }
        public string SizeList { get; set; }
        public int OrderStatus { get; set; }        
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public DateTime EntryDate { get; set; }
        public string Source { get; set;}
        public DateTime LastSyncedAt { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string FulfillmentType { get; set; }
    }
}
