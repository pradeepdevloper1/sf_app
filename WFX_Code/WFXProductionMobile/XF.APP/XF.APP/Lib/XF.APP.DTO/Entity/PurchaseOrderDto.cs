using System;
namespace XF.APP.DTO
{
    public class PurchaseOrderDto
    {
        public long Id { get; set; }
        public int OrderID { get; set; }
        public string SoNo { get; set; }
        public string PoNo { get; set; }
        public string Module { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Season { get; set; }
        public string Customer { get; set; }
        public int PrimaryPart { get; set; }
        public string Part { get; set; }
        public string Color { get; set; }
        public string Hexcode { get; set; }
        public string Fabric { get; set; }
        public string OrderRemark { get; set; }
        public int IsSizeRun { get; set; }
        public int PoQty { get; set; }
        public string SizeList { get; set; }
        public int OrderStatus { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public int PassQty { get; set; }
        public int DefectQty { get; set; }
        public int RejectQty { get; set; }
        public string Shift { get; set; }
        public string MarkPoints { get; set; }
        public DateTime PlanStDt { get; set; }
        public DateTime ExFactory { get; set; }
        public DateTime EntryDate { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        //public string ProcessCode { get; set; }
        //public string ProcessName { get; set; }
        //public string FulfillmentType { get; set; }
    }
}
