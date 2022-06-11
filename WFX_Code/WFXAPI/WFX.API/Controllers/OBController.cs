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
    public class OBController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<OBController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();

        public OBController(IWebHostEnvironment env, ILogger<OBController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("API Call");
        }

        [HttpPost]
        [Route("PostOB")]
        public ActionResult PostOB([FromBody] List<tbl_OB> list)
        { 
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var record = _context.tbl_OB.Where(x => x.SONo == list.FirstOrDefault().SONo && x.FactoryID == _UserTokenInfo.FactoryID).ToList();
                _context.tbl_OB.RemoveRange (record);
                _context.SaveChanges();

                long id = 0;
                var lastrecord = _context.tbl_OB.OrderBy(x=> x.OBID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.OBID) + 1;

                foreach (tbl_OB onerow in list)
                {
                    onerow.OBID = id;
                    onerow.Section = "NA";
                    onerow.UserID = _UserTokenInfo.UserID;
                    onerow.EntryDate = System.DateTime.Now.Date;
                    onerow.FactoryID = _UserTokenInfo.FactoryID;
                    id++;
                }
               
                _context.tbl_OB.AddRange(list);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("POOB")]
        public ActionResult POOB([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_OB> query = _context.tbl_OB.Where(x=>x.FactoryID== _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo );
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                var data = query.ToList();
                if (data.Count <= 0) { return Ok(new { status = 400, message = "No record found." }); }
                
                { return Ok(new { status = 200, message = "Success", data }); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
