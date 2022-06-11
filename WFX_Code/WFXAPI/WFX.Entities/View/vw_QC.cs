using System;

namespace WFX.Entities
{
    public class vw_QC
    {
        public long QCMasterId { get; set; }
        public DateTime QCDate { get; set; }
        public int TypeOfWork { get; set; }
        public string PONo { get; set; }
        public string Color { get; set; }
        public string Part { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int QCStatus { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string ShiftName { get; set; }

        public long QCDefectDetailsId { get; set; }
        public string DefectType { get; set; }
        public string DefectName { get; set; }
        public string OperationName { get; set; }

        public string Module { get; set; }
        public string ProcessName { get; set; }

        public string Line { get; set; }
        public string ProcessCode { get; set; }
        public string SONO { get; set; }

    }
}
