using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XF.APP
{
    [Table("PurchaseOrder")]
    public class PurchaseOrder
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public string SoNo { get; set; }

        [Required]
        public string PoNo { get; set; }

        [Required]
        public string Module { get; set; }

        [Required]
        public string Style { get; set; }

        [Required]
        public string Fit { get; set; }

        [Required]
        public string Product { get; set; }

        [Required]
        public string Season { get; set; }

        [Required]
        public string Customer { get; set; }

        [Required]
        public int PrimaryPart { get; set; }

        [Required]
        public string Part { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Hexcode { get; set; }

        [Required]
        public string Fabric { get; set; }

        [Required]
        public string OrderRemark { get; set; }

        [Required]
        public int IsSizeRun { get; set; }

        [Required]
        public int PoQty { get; set; }

        [Required]
        public string SizeList { get; set; }

        [Required]
        public int OrderStatus { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int FactoryID { get; set; }

        [Required]
        public int PassQty { get; set; }

        [Required]
        public int DefectQty { get; set; }

        [Required]
        public int RejectQty { get; set; }

        [Required]
        public string Shift { get; set; }

        [Required]
        public string MarkPoints { get; set; }

        [Required]
        public DateTime PlanStDt { get; set; }

        [Required]
        public DateTime ExFactory { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }
        [Required]
        public string WFXColorCode { get; set; }
        [Required]
        public string WFXColorName { get; set; }
        //[Required]
        //public string ProcessCode { get; set; }
        //[Required]
        //public string ProcessName { get; set; }
        //[Required]
        //public string FulfillmentType { get; set; }
    }
}
