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
    public class FactoryTypeController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<FactoryTypeController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public FactoryTypeController(ILogger<FactoryTypeController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_FactoryType.ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetFactoryTypeList")]
        public ActionResult GetFactoryTypeList([FromBody] tbl_FactoryType _obj)
        {
            try
            {
                IQueryable<tbl_FactoryType> query = _context.tbl_FactoryType;
                if (_obj.FactorytypeID > 0)
                    query = query.Where(x => x.FactorytypeID == _obj.FactorytypeID);
                if (!string.IsNullOrEmpty(_obj.FactoryType))
                    query = query.Where(x => x.FactoryType.Contains(_obj.FactoryType));
              
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostFactoryType")]
        public ActionResult PostFactoryType([FromBody] tbl_FactoryType _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_FactoryType.Where(x => x.FactoryType == _obj.FactoryType).FirstOrDefault();
               
                if (data == null)
                {
                    var lastrecord = _context.tbl_FactoryType.OrderBy(x => x.FactorytypeID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.FactorytypeID) + 1;
                    _obj.FactorytypeID = id;
                  
                   
                    _context.tbl_FactoryType.Add(_obj);
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
        [Route("PutFactoryType")]
        public ActionResult PutFactoryType([FromBody] tbl_FactoryType _obj)
        {
            try
            {
                var lastrecord = _context.tbl_FactoryType.Where(x => x.FactorytypeID == _obj.FactorytypeID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.FactoryType = _obj.FactoryType;
               
                    _context.tbl_FactoryType.Update(lastrecord);
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
        [Route("DeleteFactoryType")]
        public ActionResult DeleteFactoryType([FromBody] tbl_FactoryType _obj)
        {
            try
            {
                IQueryable<tbl_FactoryType> query = _context.tbl_FactoryType.Where(x => x.FactorytypeID == _obj.FactorytypeID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_FactoryType.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillFactoryType")]
        public ActionResult FillFactoryType()
        {
            try
            {
                IQueryable<tbl_FactoryType> query = _context.tbl_FactoryType;
                var data = query.Select(x => new { Id = x.FactoryType, Text = x.FactoryType }).ToList();
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
