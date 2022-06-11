using System;

namespace WFX.Entities
{
    public class UserDashboardModel
    {
        public int RunningOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int DelayedOrders { get; set; }
        public int Compliance{ get; set; }
        public int PicesChecked { get; set; }
        public int RejectionRate { get; set; }
        public int CurrentDHU { get; set; }

        public int QualityBottlenecks { get; set; }

        public double LowestLlineEff { get; set; }
        public double AveragetLlineEff { get; set; }
        public double HighestLlineEff { get; set; }
    }

    public class TopDefect
    {
        public int Count { get; set; }
        public string DefectName { get; set; }
        public int Per { get; set; }
    }

    // for Mobile
    public class UserLoginDetails
    {
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserRoleID { get; set; }
        public int Members { get; set; }
        public string UserType { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string LineName { get; set; }
        public string LinkedwithERP { get; set; }
        public string Module { get; set; }


    }
}
