using System;

namespace WFX.Entities
{
    public class tbl_POImages
    {
        public long POImageID { get; set; }
        public string PONo { get; set; }
        public string ImageName { get; set; }        
        public string ImagePath { get; set; }
        public int IMGNo { get; set; }
        public int FactoryID { get; set; }        
        public int UserID { get; set; }
        public DateTime EntryDate { get; set; }
        public string SONo { get; set; }
    }
}
