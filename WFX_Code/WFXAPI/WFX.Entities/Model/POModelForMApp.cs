using System;
using System.Collections.Generic;
using WFX.Entities;
namespace WFX.Entities
{
    public class POModelForMApp
    {
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
        //public string Part { get; set; }
        //public string Color { get; set; }
        public string Hexcode { get; set; }
        public string Fabric { get; set; }
        //public string OrderRemark { get; set; }
        //public int IsSizeRun { get; set; }
        public int POQty { get; set; }
        //public string SizeList { get; set; }
        public int OrderStatus { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public DateTime EntryDate { get; set; }

        public string Line { get; set; }

        public List<tbl_POImages> imagelist { get; set; }
        public List<tbl_POShiftImages> shiftimagelist { get; set; }
        //public List<POColorList> colorlist { get; set; }
        public List<string> colorlist { get; set; }
    }


    public class POColorList
    {
        //public string PONo { get; set; }
        public string Color { get; set; }
    }
}
