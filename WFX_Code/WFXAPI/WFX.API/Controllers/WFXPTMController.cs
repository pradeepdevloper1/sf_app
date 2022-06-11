using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFX.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class WFXPTMController : ControllerBase
    {
        [HttpGet]
        [Route("APITest")]
        [EnableCors("AllowAllOrigins")]
        public ActionResult Get()
        {
            return new OkObjectResult("WFXPTM API is working!");
        }
    }
}
