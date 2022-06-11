using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class SoPoOutputDto : BaseResponseDto
    {
        public List<SoPoWebDto> data { get; set; }

        public List<PoImageDto> imagelist { get; set; }
        public List<QCMaster> qcmasterlist { get; set; }

    }

    public class PurchaseOrder1
    {
        public int orderID { get; set; }

        public string module { get; set; }

        public string soNo { get; set; }

        public string poNo { get; set; }

        public string style { get; set; }

        public string fit { get; set; }

        public string product { get; set; }

        public string season { get; set; }

        public string customer { get; set; }

        public DateTime planStDt { get; set; }

        public DateTime exFactory { get; set; }

        public int primaryPart { get; set; }

        public string part { get; set; }

        public string color { get; set; }

        public string hexcode { get; set; }

        public string fabric { get; set; }

        public string orderRemark { get; set; }

        public int isSizeRun { get; set; }

        public int poQty { get; set; }

        public string sizeList { get; set; }

        public int orderStatus { get; set; }

        public int userID { get; set; }

        public int factoryID { get; set; }

        public DateTime entryDate { get; set; }

        public string line { get; set; }

        public int completedColors { get; set; }

        public int completedQty { get; set; }

        public float colorsGraphProgress { get; set; }

        public float qtyGraphProgress { get; set; }

        public int totalColors { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string FulfillmentType { get; set; }
    }

    public class SoPoWebDto
    {
        public int orderID { get; set; }

        public string module { get; set; }

        public string soNo { get; set; }

        public string poNo { get; set; }

        public string style { get; set; }

        public string fit { get; set; }

        public string product { get; set; }

        public string season { get; set; }

        public string customer { get; set; }

        public DateTime planStDt { get; set; }

        public DateTime exFactory { get; set; }

        public int primaryPart { get; set; }

        public string part { get; set; }

        public string color { get; set; }

        public string hexcode { get; set; }

        public string fabric { get; set; }

        public string orderRemark { get; set; }

        public int isSizeRun { get; set; }

        public int poQty { get; set; }

        public string sizeList { get; set; }

        public int orderStatus { get; set; }

        public int userID { get; set; }

        public int factoryID { get; set; }

        public DateTime entryDate { get; set; }

        public string line { get; set; }
        public string WFXColorCode { get; set; }
        public string WFXColorName { get; set; }
        //public string ProcessCode { get; set; }
        //public string ProcessName { get; set; }
        //public string FulfillmentType { get; set; }
    }

    public class PoImageDto
    {
        public int poImageID { get; set; }

        public string poNo { get; set; }

        public string imageName { get; set; }

        public string imagePath { get; set; }

        public int factoryID { get; set; }

        public int userID { get; set; }

        public DateTime entryDate { get; set; }
    }

    public class PurchaseOrderShift {
        public string soNo { get; set; }

        public string poNo { get; set; }

        public string color { get; set; }

        public string Size { get; set; }

        public int DefectType { get; set; }

        public int Qty { get; set; }
        public int TypeofWork { get; set; }

    }
    public class QCMaster
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
        public string Module { get; set; }
        public string Line { get; set; }
        public string SONo { get; set; }
        public decimal WFXStockGRNID { get; set; }
        public string BatchNumber { get; set; }
        public long QCRequestID { get; set; }
        public string TabletID { get; set; }

    }
}