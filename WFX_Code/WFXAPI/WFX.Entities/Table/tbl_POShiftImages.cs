using System;

namespace WFX.Entities
{
    public class tbl_POShiftImages
    {
        public long POShiftImageID { get; set; }
        public string PONo { get; set; }
        public string ImageName { get; set; }        
        public string ImagePath { get; set; }
        public string ShiftName { get; set; }
        public int FactoryID { get; set; }
        public int UserID { get; set; }
        public DateTime EntryDate { get; set; }
        public string SONo { get; set; }
    }
}
