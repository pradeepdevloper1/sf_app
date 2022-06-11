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
    public class LineBookingController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<LineBookingController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public LineBookingController(IWebHostEnvironment env, ILogger<LineBookingController> logger, DBContext context, IConfiguration configuration)
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
        [Route("PostLineBooking")]
        public ActionResult PostLineBooking([FromBody] List<tbl_LineBooking> list)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_LineBooking.OrderBy(x => x.LineBookingID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.LineBookingID) + 1;
                foreach (tbl_LineBooking onerow in list)
                {
                    onerow.LineBookingID = id;
                    onerow.EntryDate = System.DateTime.Now.Date;
                    id++;
                }
                _context.tbl_LineBooking.AddRange(list);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Booking Save" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetLineBookingList")]
        public ActionResult GetLineBookingList([FromBody] tbl_LineBooking _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_LineBooking> query = _context.tbl_LineBooking.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.Line))
                    query = query.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Module))
                    query = query.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                var data = query.ToList();
                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
