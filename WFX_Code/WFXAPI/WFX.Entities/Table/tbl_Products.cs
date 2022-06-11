using System;

namespace WFX.Entities
{
    public class tbl_Products
    {
        public long ProductID { get; set; }
        public int FactoryID { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
