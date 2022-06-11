using System;

namespace WFX.Entities
{
    public class tbl_DailActivity
    {
        public long DailActivityID { get; set; }
        public DateTime ActivitDate { get; set; }
        public int UserID { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public int Seconds { get; set; }
        public int IsActive { get; set; }
        public int IsPause { get; set; }
        public string PONo { get; set; }
        public string LineName { get; set; }
        public int OverTime { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogOut { get; set; }
        public int Manhrs { get; set; }
    }
}
