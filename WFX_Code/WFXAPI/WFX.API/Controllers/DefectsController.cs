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
    public class DefectsController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<DefectsController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public DefectsController(ILogger<DefectsController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
            var obj = _context.tbl_Defects.Where(x => x.FactoryID == _UserTokenInfo.FactoryID).ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetDefectsList")]
        public ActionResult GetDefectsList([FromBody] tbl_Defects _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Defects> query = _context.tbl_Defects.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID);
                if (_obj.DefectID > 0)
                    query = query.Where(x => x.DefectID == _obj.DefectID);
                if (_obj.DepartmentID > 0)
                    query = query.Where(x => x.DepartmentID == _obj.DepartmentID);
                if (!string.IsNullOrEmpty(_obj.DefectCode))
                    query = query.Where(x => x.DefectCode.Contains(_obj.DefectCode));
                if (!string.IsNullOrEmpty(_obj.DefectName))
                    query = query.Where(x => x.DefectName.Contains(_obj.DefectName));
                if (!string.IsNullOrEmpty(_obj.DefectType))
                    query = query.Where(x => x.DefectType.Contains(_obj.DefectType));
                if (!string.IsNullOrEmpty(_obj.DefectLevel))
                    query = query.Where(x => x.DefectLevel.Contains(_obj.DefectLevel));
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostDefect")]
        public ActionResult PostDefect([FromBody] tbl_Defects _obj)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Defects.OrderBy(x => x.DefectID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.DefectID) + 1;
                _obj.DefectID = id;

                _context.tbl_Defects.Add(_obj);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Save Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }       

        [HttpPost]
        [Route("PutDefect")]
        public ActionResult PutDefect([FromBody] tbl_Defects _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Defects.Where(x => x.DefectID == _obj.DefectID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.DepartmentID = _obj.DepartmentID;
                    lastrecord.DefectCode = _obj.DefectCode;
                    lastrecord.DefectName = _obj.DefectName;
                    lastrecord.DefectType = _obj.DefectType;
                    lastrecord.DefectLevel = _obj.DefectLevel;

                    _context.tbl_Defects.Update(lastrecord);
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
        [Route("DeleteDefect")]
        public ActionResult DeleteDefect([FromBody] tbl_Defects _obj)
        {
            try
            {
                IQueryable<tbl_Defects> query = _context.tbl_Defects.Where(x => x.DefectID == _obj.DefectID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Defects.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDefectsList/{factoryid}")]
        public ActionResult GetDefectsList(int factoryid)
        {
            try
            {
                var obj = _context.tbl_Defects.Where(x => x.FactoryID == factoryid).OrderBy(x => x.DefectName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostMultiDefects")]
        public ActionResult PostMultiDefects([FromBody] List<tbl_Defects> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Defects.OrderBy(x => x.DefectID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.DefectID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Defects onerow in list)
                    {
                        var record = _context.tbl_Defects.Where(x => x.DefectCode == onerow.DefectCode && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Defects>();
                        if (record == null)
                        {
                            onerow.DefectID = id;
                            onerow.DepartmentID = onerow.DepartmentID;
                            onerow.DefectCode = onerow.DefectCode;
                            onerow.DefectName = onerow.DefectName;
                            onerow.DefectType = onerow.DefectType;
                            onerow.DefectLevel = onerow.DefectLevel;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Defects.Add(onerow);
                            _context.SaveChanges();
                            id++;
                        }
                    }
                }

                return Ok(new { status = 200, message =  "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutMultiDefects")]
        public ActionResult PutMultiDefects([FromBody] List<tbl_Defects> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Defects onerow in list)
                    {
                        var record = _context.tbl_Defects.Where(x => x.DefectID == onerow.DefectID).SingleOrDefault();
                        if (record != null)
                        {
                            record.DepartmentID = onerow.DepartmentID;
                            record.DefectCode = onerow.DefectCode;
                            record.DefectName = onerow.DefectName;
                            record.DefectType = onerow.DefectType;
                            record.DefectLevel = onerow.DefectLevel;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Defects.Update(record);
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

    }
}
