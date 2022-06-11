
using System;
namespace WFX.Entities
{
    public class tbl_OBFileUploadData
    {
        public Int64 OBFileUploadDataID { get; set; }
        public Int64 OBFileUploadID { get; set; }
        public string OperationCode { get; set; }
        public string OperationName { get; set; }
        public string Section { get; set; }
        public double SMV { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int FactoryID { get; set; }
    }
}
