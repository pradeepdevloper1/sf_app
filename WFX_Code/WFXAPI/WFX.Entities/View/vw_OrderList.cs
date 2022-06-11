using System;

namespace WFX.Entities
{
    public class vw_OrderList
    {
        public string Module { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Season { get; set; }
        public string Customer { get; set; }
        public DateTime ExFactory { get; set; }
        public string OrderRemark { get; set; }
        public int POQty { get; set; }
        public int OrderStatus { get; set; }        
        public int FactoryID { get; set; }
        public DateTime EntryDate { get; set; }
        public  int CompletedQty { get; set; }
        public int CompletedPer { get; set; }
        public long OrderID { get; set; }
        public DateTime LastSyncedAt { get; set; }
        public string Source { get; set;}
        public string ProcessCode{ get; set; }
        public string ProcessName { get; set; }
        public string FulfillmentType { get; set; }
    }
}
