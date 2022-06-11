using System.Net;

namespace WFX.API.Model
{
    public class ResponseDto
    {
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public string auth { get; set; }
        public object data { get; set; }
    }
}
