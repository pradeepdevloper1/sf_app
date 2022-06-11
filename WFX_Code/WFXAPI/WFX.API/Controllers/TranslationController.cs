using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<TranslationController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public TranslationController(IWebHostEnvironment env, ILogger<TranslationController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;

        }
        [HttpGet]
        [Route("GetData/{ObjectName}")]
        public ActionResult GetData(string ObjectName)
        {
            try
            {
                var obj = _context.tbl_Translation.Where(x => x.ObjectName == ObjectName).OrderBy(x=> x.TranslationID).ToList();
                if (obj == null)
                    return Ok(new { status = 400, message = "no Record Found" });
                return Ok(new { status = 200, obj });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
    }
}
