
using System;
namespace WFX.Entities
{
    public class tbl_OBFileUpload
    {
        public long OBFileUploadID { get; set; }
        public int ProductID { get; set; }
        public string ProcessCode { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FactoryID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
