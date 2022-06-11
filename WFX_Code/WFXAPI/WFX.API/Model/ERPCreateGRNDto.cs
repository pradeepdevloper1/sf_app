using System.Net;

namespace WFX.API.Model
{
    public class ERPCreateGRNOutputDto
    {
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public ERPCreateGRNDto data { get; set; }
    }

    public class ERPCreateGRNDto { 
        public long StockGRNID { get; set; }
    }
}
