using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TimeZoneController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<TimeZoneController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public TimeZoneController(ILogger<TimeZoneController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_TimeZone.ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetTimeZoneList")]
        public ActionResult GetTimeZoneList([FromBody] tbl_TimeZone _obj)
        {
            try
            {
                IQueryable<tbl_TimeZone> query = _context.tbl_TimeZone;
                if (_obj.TimeZoneID > 0)
                    query = query.Where(x => x.TimeZoneID == _obj.TimeZoneID);
                if (!string.IsNullOrEmpty(_obj.TimeZone))
                    query = query.Where(x => x.TimeZone.Contains(_obj.TimeZone));
              
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostTimeZone")]
        public ActionResult PostTimeZone([FromBody] tbl_TimeZone _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_TimeZone.Where(x => x.TimeZone == _obj.TimeZone).FirstOrDefault();
               
                if (data == null)
                {
                    var lastrecord = _context.tbl_TimeZone.OrderBy(x => x.TimeZoneID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.TimeZoneID) + 1;
                    _obj.TimeZoneID = id;
                  
                   
                    _context.tbl_TimeZone.Add(_obj);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Save Success" });
                }
                else
                {
                    return Ok(new { status = 201, message = "Already Exits" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

        [HttpPut]
        [Route("PutTimeZone")]
        public ActionResult PutTimeZone([FromBody] tbl_TimeZone _obj)
        {
            try
            {
                var lastrecord = _context.tbl_TimeZone.Where(x => x.TimeZoneID == _obj.TimeZoneID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.CountryID = _obj.CountryID;
                    lastrecord.TimeZone = _obj.TimeZone;
               
                    _context.tbl_TimeZone.Update(lastrecord);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Update Success" });
                }
                return Ok(new { status = 400, message = "No record found." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteTimeZone")]
        public ActionResult DeleteTimeZone([FromBody] tbl_TimeZone _obj)
        {
            try
            {
                IQueryable<tbl_TimeZone> query = _context.tbl_TimeZone.Where(x => x.TimeZoneID == _obj.TimeZoneID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_TimeZone.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillTimeZone/{countryid}")]
        public ActionResult FillTimeZone(int countryid)
        {
            try
            {
                IQueryable<tbl_TimeZone> query = _context.tbl_TimeZone.Where(x=>x.CountryID== countryid);
                var data = query.Select(x => new { Id = x.TimeZone, Text = x.TimeZone }).ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
