using System;

namespace WFX.Entities
{
    public class tbl_Customer
    {
        public long CustomerID { get; set; }
        public int FactoryID { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
