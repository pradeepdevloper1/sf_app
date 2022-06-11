using System;

namespace WFX.Entities
{
    public class vw_SOView
    {
        public string Module { get; set; }
        public string SONo { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Season { get; set; }
        public string Customer { get; set; }
        public int SOQty { get; set; }
        public int FactoryID { get; set; }
        public int NoOfPO { get; set; }
        public int OrderStatus{ get; set; }


    }
}
