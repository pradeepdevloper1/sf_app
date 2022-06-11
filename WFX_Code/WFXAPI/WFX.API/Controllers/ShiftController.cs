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
    public class ShiftController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<ShiftController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ShiftController(ILogger<ShiftController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Shift.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID).ToList().OrderBy(x=> x.ShiftID);
                if (data == null)
                {
                    return Ok(new { status = 400, message = "No record found." });
                }
                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("GetShiftList")]
        public ActionResult GetShiftList([FromBody] tbl_Shift _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Shift> query = _context.tbl_Shift.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (_obj.ShiftID > 0)
                    query = query.Where(x => x.ShiftID == _obj.ShiftID);
                if (_obj.ModuleID > 0)
                    query = query.Where(x => x.ModuleID == _obj.ModuleID);

                if (!string.IsNullOrEmpty(_obj.ShiftName))
                    query = query.Where(x => x.ShiftName.Contains(_obj.ShiftName));
                if (!string.IsNullOrEmpty(_obj.ShiftStartTime))
                    query = query.Where(x => x.ShiftStartTime.Contains(_obj.ShiftStartTime));
                if (!string.IsNullOrEmpty(_obj.ShiftEndTime))
                    query = query.Where(x => x.ShiftEndTime.Contains(_obj.ShiftEndTime));
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost]
        [Route("DeleteShift")]
        public ActionResult DeleteShift([FromBody] tbl_Shift _obj)
        {
            try
            {
                IQueryable<tbl_Shift> query = _context.tbl_Shift.Where(x => x.ShiftID == _obj.ShiftID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Shift.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostMultiShift")]
        public ActionResult PostMultiShift([FromBody] List<tbl_Shift> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Shift.OrderBy(x => x.ShiftID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.ShiftID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Shift onerow in list)
                    {
                        var record = _context.tbl_Shift.Where(x => x.ShiftName == onerow.ShiftName && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Shift>();
                        if (record == null)
                        {
                            onerow.ShiftID = id;
                            onerow.ModuleID = 1;
                            onerow.ShiftName = onerow.ShiftName;
                            onerow.ShiftStartTime = onerow.ShiftStartTime;
                            onerow.ShiftEndTime = onerow.ShiftEndTime;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Shift.Add(onerow);
                            _context.SaveChanges();
                            id++;
                        }
                    }
                }

                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutMultiShift")]
        public ActionResult PutMultiShift([FromBody] List<tbl_Shift> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Shift onerow in list)
                    {
                        var record = _context.tbl_Shift.Where(x => x.ShiftID == onerow.ShiftID).SingleOrDefault();
                        if (record != null)
                        {
                            record.ShiftName = onerow.ShiftName;
                            record.ShiftStartTime = onerow.ShiftStartTime;
                            record.ShiftEndTime = onerow.ShiftEndTime;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Shift.Update(record);

                        }

                    }
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Success" });
                }
                else
                {
                    return Ok(new { status = 400, message = "No Record Found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShiftList/{factoryid}")]
        public ActionResult GetShiftList(int factoryid)
        {
            try
            {
                var obj = _context.tbl_Shift.Where(x => x.FactoryID == factoryid).OrderBy(x => x.ShiftName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillFactoryShift")]
        public ActionResult FillFactoryShift()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Shift> query = _context.tbl_Shift.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.ShiftName, Text = x.ShiftName }).Distinct().ToList();
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
