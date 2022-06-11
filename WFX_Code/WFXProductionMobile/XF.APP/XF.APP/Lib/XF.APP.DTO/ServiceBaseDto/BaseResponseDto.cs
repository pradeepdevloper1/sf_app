using System;
using System.Collections.Generic;
using System.Net;

namespace XF.APP.DTO
{
    public class BaseResponseDto : BaseDto
    {
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public string auth { get; set; } 
    } 
}
