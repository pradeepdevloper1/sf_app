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
    public class CountryController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<CountryController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public CountryController(ILogger<CountryController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_Country.ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetCountryList")]
        public ActionResult GetCountryList([FromBody] tbl_Country _obj)
        {
            try
            {
                IQueryable<tbl_Country> query = _context.tbl_Country;
                if (_obj.CountryID > 0)
                    query = query.Where(x => x.CountryID == _obj.CountryID);
                if (!string.IsNullOrEmpty(_obj.CountryName))
                    query = query.Where(x => x.CountryName.Contains(_obj.CountryName));
              
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostCountry")]
        public ActionResult PostCountry([FromBody] tbl_Country _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Country.Where(x => x.CountryName == _obj.CountryName).FirstOrDefault();
               
                if (data == null)
                {
                    var lastrecord = _context.tbl_Country.OrderBy(x => x.CountryID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.CountryID) + 1;
                    _obj.CountryID = id;
                  
                   
                    _context.tbl_Country.Add(_obj);
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
        [Route("PutCountry")]
        public ActionResult PutCountry([FromBody] tbl_Country _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Country.Where(x => x.CountryID == _obj.CountryID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.CountryName = _obj.CountryName;
               
                    _context.tbl_Country.Update(lastrecord);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Success" });
                }
                return Ok(new { status = 400, message = "No record found." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteCountry")]
        public ActionResult DeleteCountry([FromBody] tbl_Country _obj)
        {
            try
            {
                IQueryable<tbl_Country> query = _context.tbl_Country.Where(x => x.CountryID == _obj.CountryID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Country.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillCountry")]
        public ActionResult FillCountry()
        {
            try
            {
                IQueryable<tbl_Country> query = _context.tbl_Country;
                var data = query.Select(x => new { Id = x.CountryID, Text = x.CountryName }).ToList();
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
