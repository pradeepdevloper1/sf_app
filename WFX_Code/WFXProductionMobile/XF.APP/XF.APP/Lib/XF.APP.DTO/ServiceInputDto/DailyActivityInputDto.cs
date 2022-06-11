namespace XF.APP.DTO
{
    public class DailyActivityInputDto
    {
        public string ActivitDate { get; set; }
        public int UserID { get; set; }
        public string LastUpdatedDateTime { get; set; }
        public long Seconds { get; set; }
        public int IsActive { get; set; }
        public int IsPause { get; set; }
        public string PONo { get; set; }
        public string LineName { get; set; }
        public int OverTime { get; set; }
        public string FirstLogin { get; set; }
        public string LastLogOut { get; set; }
        public int Manhrs { get; set; }
    }
}
