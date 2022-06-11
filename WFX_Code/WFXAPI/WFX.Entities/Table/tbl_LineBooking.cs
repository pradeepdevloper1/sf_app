using System;

namespace WFX.Entities
{
    public class tbl_LineBooking
    {
        public long LineBookingID { get; set; }
        public string Module { get; set; }
        public string Line { get; set; }
        public string Style { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public int Quantity { get; set; }
        public double SMV { get; set; }
        public double PlannedEffeciency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserID { get; set; }
        public DateTime EntryDate { get; set; }
        public int FactoryID { get; set; }
    }
}
