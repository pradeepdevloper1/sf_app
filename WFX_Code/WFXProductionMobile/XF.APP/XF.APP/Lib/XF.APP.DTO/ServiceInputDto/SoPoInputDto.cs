namespace XF.APP.DTO
{
    public class SoPoInputDto
    {
        public string SONo { get; set; }
        public string Line { get; set; }
        public string TabletID { get; set; }

    }

    public class SoViewInputDto
    {
        public int FactoryID { get; set; }
        public string RoleName { get; set; }
        public string Line { get; set; }

    }

    public class PoInputDto
    {
        public string PONo { get; set; }
        public string Line { get; set; }
    }
}
