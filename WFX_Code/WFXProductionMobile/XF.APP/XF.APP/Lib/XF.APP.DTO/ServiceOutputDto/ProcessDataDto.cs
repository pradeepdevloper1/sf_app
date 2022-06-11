using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class ProcessDataDto : BaseResponseDto
    {
        public List<ProcessData> data { get; set; }
        public double Totalqty { get; set; }

    }
    public class ProcessData
    {
        public string color { get; set; }
        public string size { get; set; }
        public int orderedQty { get; set; }
        public int completedQty { get; set; }
        public int issuedQty { get; set; }
        public int receivedbyNextDept { get; set; }
        public int remainingQtytobeIssued { get; set; }
        public int issueColorQty { get; set; }
        public string wfxColorCode { get; set; }
        public string wfxColorName { get; set; }
        public string colorHexName { get; set; }
        
    }
    public class OrderIssue
    {
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Color { get; set; }
        public string Part { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public string TabletID { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        public string Line { get; set; }
        public string ToPONo { get; set; }
        public string ToLine { get; set; }
    }
}
