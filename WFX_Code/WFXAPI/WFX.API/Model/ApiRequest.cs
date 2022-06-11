using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFX.API.WebService
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Auth { get; set; }
    }
}
