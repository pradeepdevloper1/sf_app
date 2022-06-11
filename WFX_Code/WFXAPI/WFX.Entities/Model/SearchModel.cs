using System;

namespace WFX.Entities
{
    public class SearchModel
    {
        public string PONo { get; set; }
        public string SONo { get; set; }
        public string POSearchText { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Line { get; set; }
        public string TargetStartDate { get; set; }
        public string TargetEndDate { get; set; }
        public int IsTargetStartDate { get; set; }
        public int IsTargetEndDate { get; set; }
        public string ShiftName { get; set; }
        public DateTime QCDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OperationName { get; set; }


        public string UserName { get; set; }
        public string Password { get; set; }
        public string TabletID { get; set; }
        public string Season { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public int ProductID { get; set; }
        public string Customer { get; set; }
        public string Style { get; set; }
        public int FactoryID { get; set; }
        public string FromPage { get; set; }
        public string Organisation { get; set; }
        public long QCRequestID { get; set; }
        public string DeviceSerialNo { get; set; }
        public string ProcessCode { get; set; }
        public string RoleName { get; set; }
        public int ExportAutoFill { get; set; }
    }
}
