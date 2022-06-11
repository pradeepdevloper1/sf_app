using System;

namespace WFX.Entities
{
    public class AdminExcelModel
    {
        //Product
        public int Product_Count { get; set; }
        public string Product_Status { get; set; }
        public string Product_CreateDate { get; set; }
        public string Product_UpdateDate { get; set; }

        //Fit
        public int Fit_Count { get; set; }
        public string Fit_Status { get; set; }
        public string Fit_CreateDate { get; set; }
        public string Fit_UpdateDate { get; set; }

        //Customer
        public int Customer_Count { get; set; }
        public string Customer_Status { get; set; }
        public string Customer_CreateDate { get; set; }
        public string Customer_UpdateDate { get; set; }

        //Line
        public int Line_Count { get; set; }
        public string Line_Status { get; set; }
        public string Line_CreateDate { get; set; }
        public string Line_UpdateDate { get; set; }

        //Shift
        public int Shift_Count { get; set; }
        public string Shift_Status { get; set; }
        public string Shift_CreateDate { get; set; }
        public string Shift_UpdateDate { get; set; }

        //QCCode
        public int QCCode_Count { get; set; }
        public string QCCode_Status { get; set; }
        public string QCCode_CreateDate { get; set; }
        public string QCCode_UpdateDate { get; set; }
        
        //User
        public int User_Count { get; set; }
        public string User_Status { get; set; }
        public string User_CreateDate { get; set; }
        public string User_UpdateDate { get; set; }

        //Holidays
        public int Holidays_Count { get; set; }
        public string Holidays_Status { get; set; }
        public string Holidays_CreateDate { get; set; }
        public string Holidays_UpdateDate { get; set; }

        //Module
        public int Module_Count { get; set; }
        public string Module_Status { get; set; }
        public string Module_CreateDate { get; set; }
        public string Module_UpdateDate { get; set; }

        public int processDefinition_Count { get; set; }
        public string processDefinition_Status { get; set; }
        public string processDefinition_CreatedOn { get; set; }
        public string processDefinition_LastChangedOn { get; set; }
    }
}
