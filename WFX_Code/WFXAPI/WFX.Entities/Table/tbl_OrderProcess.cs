using System;

namespace WFX.Entities
{
    public class tbl_OrderProcess
    {
        public long OrderProcessID { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string AfterProcessCode { get; set; }
        public string AfterProcessName { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public int FactoryID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool ProcessEnabled { get; set; }
    }
}

   