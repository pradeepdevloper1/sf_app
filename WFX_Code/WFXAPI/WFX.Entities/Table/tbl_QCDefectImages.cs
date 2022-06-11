using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities.Table
{
    public class tbl_QCDefectImages
    {
        public long QCDefectImagesID { get; set; }
        public long QCMasterId { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public int FactoryID { get; set; }
        public int UserID { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public DateTime EntryDate { get; set; }

    }
}
