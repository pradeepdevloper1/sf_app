using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class PostQCInputDto
    {
        public int TypeOfWork { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Color { get; set; }
        public string HexCode { get; set; }
        public string Part { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int QCStatus { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string ShiftName { get; set; }
        public string Line { get; set; }
        public string Module { get; set; }
        public string TabletID { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        //public string ProcessCode { get; set; }
        public List<TblQCDefectDetail> tbl_QCDefectDetails { get; set; }
    }

    public class TblQCDefectDetail
    {
        public string DefectType { get; set; }
        public string DefectName { get; set; }
        public string OperationName { get; set; }
    }
}
