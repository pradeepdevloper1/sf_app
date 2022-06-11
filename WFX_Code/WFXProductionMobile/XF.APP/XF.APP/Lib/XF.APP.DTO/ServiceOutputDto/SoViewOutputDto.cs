using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class SoViewOutputDto : BaseResponseDto
    {
        public List<SoViewWebDto> data { get; set; }
    }

    public class SoViewWebDto
    {
        public string module { get; set; }
        public string soNo { get; set; }
        public string style { get; set; }
        public string fit { get; set; }
        public string product { get; set; }
        public string season { get; set; }
        public string customer { get; set; }
        public int soQty { get; set; }
        public int factoryID { get; set; }
        public int noOfPO { get; set; }
    }

    public class SalesOrder
    {
        public string module { get; set; }
        public string soNo { get; set; }
        public string style { get; set; }
        public string fit { get; set; }
        public string product { get; set; }
        public string season { get; set; }
        public string customer { get; set; }
        public int soQty { get; set; }
        public int factoryID { get; set; }
        public int noOfPO { get; set; }
        public int completedQty { get; set; }
        public int completedPo { get; set; }
        public float poGraphProgress { get; set; }
        public float qtyGraphProgress { get; set; }
    }
}
