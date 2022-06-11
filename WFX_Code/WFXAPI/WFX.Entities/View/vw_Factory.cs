using System;

namespace WFX.Entities
{
    public class vw_Factory
    {
        public int FactoryID { get; set; }
        public string FactoryName { get; set; }
        public string FactoryAddress { get; set; }
        public string FactoryType { get; set; }
        public int FactoryCountry { get; set; }
        //public string FactoryHead { get; set; }
        //public string FactoryEmail { get; set; }
        //public int FactoryContactNumber { get; set; }
        public string FactoryTimeZone { get; set; }
        public int NoOfShifts { get; set; }
        //public int DecimalValue { get; set; }        
        //public double PTMPrice { get; set; }
        public int NoOfUsers { get; set; }
        //public string FactoryOffOn { get; set; }
        //public string MeasuringUnit { get; set; }
        //public int DataScale { get; set; }
        //public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string ClusterName { get; set; }
        public string FactoryStatus { get; set; }
        public int ClusterID { get; set; }
        public int OrganisationID { get; set; }
        public int NoOfLine { get; set; }
        public int SmartLines { get; set; }

        public string LinkedwithERP { get; set; }
    }
}
