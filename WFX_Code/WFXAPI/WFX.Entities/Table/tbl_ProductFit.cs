using System;

namespace WFX.Entities
{
    public class tbl_ProductFit
    {
        public int ProductFitID { get; set; }
        public int FactoryID { get; set; }
        public string FitType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
