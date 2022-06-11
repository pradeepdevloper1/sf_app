using System;

namespace WFX.Entities
{
    public class tbl_OB
    {
        public long OBID { get; set; }
        public string PONo { get; set; }
        public int SNo { get; set; }
        public string OperationCode { get; set; }        
        public string OperationName { get; set; }
        public string Section { get; set; }
        public double SMV { get; set; }
        public int UserID { get; set; }
        public DateTime EntryDate { get; set; }
        public string OBLocation { get; set; }
        public int FactoryID { get; set; }
        public string SONo { get; set; }
    }
}
