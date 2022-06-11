using System;
using System.ComponentModel.DataAnnotations;

namespace WFX.Entities
{
    public class tbl_QCRequest
    {
        public long QCRequestID { get; set; }
        public string TranNum { get; set; }
        public string PONo { get; set; }
        public string SONo { get; set; }
        public int Quantity { get; set; }
        public DateTime SyncedAt { get; set; }
        public string GRNstatus { get; set; }
        public long StockGRNID { get; set; }
        public string ErrorMessage { get; set; }
        public int FactoryID { get; set; }
        public string RequestType { get; set; }
        public string LineNumber { get; set; }
        public string StyleRef { get; set; }
    }
}
