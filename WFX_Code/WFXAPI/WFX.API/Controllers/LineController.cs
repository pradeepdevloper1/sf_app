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
    public class LineController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<LineController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public LineController(IWebHostEnvironment env, ILogger<LineController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Line API Call");
        }

        [Route("PostIsLineExist")]
        [HttpPost]
        public ActionResult PostIsLineExist([FromBody] tbl_Lines _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.LineName == _obj.LineName).FirstOrDefault();
                if (data == null)
                {
                    return Ok(new { status = 401, message = "Not Found" });
                }
                else
                {
                    return Ok(new { status = 200, message = "Found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillLine")]
        public ActionResult FillLine()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_Lines> query = _context.vw_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.LineName, Text = x.LineName }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("FillLine")]
        public ActionResult FillLine(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_LineTarget> dataQuery;
                dynamic data = null;
                var lineQuery = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line != null && x.Line != "");
                if (!string.IsNullOrEmpty(_obj.SONo))
                    lineQuery = lineQuery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    lineQuery = lineQuery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    lineQuery = lineQuery.Where(x => x.ProcessCode == _obj.ProcessCode);
                if (!string.IsNullOrEmpty(_obj.Style))
                    lineQuery = lineQuery.Where(x => x.Style == _obj.Style);
                if (!string.IsNullOrEmpty(_obj.Module))
                    lineQuery = lineQuery.Where(x => x.Module == _obj.Module);

                var orderQuery = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    orderQuery = orderQuery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    orderQuery = orderQuery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    orderQuery = orderQuery.Where(x => x.ProcessCode == _obj.ProcessCode);
                if (!string.IsNullOrEmpty(_obj.Product))
                    orderQuery = orderQuery.Where(x => x.Product == _obj.Product);
                if (!string.IsNullOrEmpty(_obj.Style))
                    orderQuery = orderQuery.Where(x => x.Style == _obj.Style);
                if (!string.IsNullOrEmpty(_obj.Fit))
                    orderQuery = orderQuery.Where(x => x.Fit == _obj.Fit);
                if (!string.IsNullOrEmpty(_obj.Season))
                    orderQuery = orderQuery.Where(x => x.Season == _obj.Season);
                if (!string.IsNullOrEmpty(_obj.Customer))
                    orderQuery = orderQuery.Where(x => x.Customer == _obj.Customer);
                if (!string.IsNullOrEmpty(_obj.Module))
                    orderQuery = orderQuery.Where(x => x.Module == _obj.Module);

                dataQuery = (IQueryable<tbl_LineTarget>)
                    from p in lineQuery
                    join s in orderQuery
                    on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                    group p by new
                    {
                        Line = p.Line
                    }
                    into g
                    select new tbl_LineTarget
                    {
                        Line = g.Key.Line,
                    };

                data = dataQuery.Select(x => new { ID = x.Line, Text = x.Line }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetLineList")]
        public ActionResult GetLineList([FromBody] tbl_Lines _obj)
        {
            try
            {
                IQueryable<tbl_Lines> query = _context.tbl_Lines;
                if (_obj.LineID > 0)
                    query = query.Where(x => x.LineID == _obj.LineID);
                if (_obj.ModuleID > 0)
                    query = query.Where(x => x.ModuleID == _obj.ModuleID);
                if (_obj.LineID > 0)
                    query = query.Where(x => x.LineID == _obj.LineID);
                if (!string.IsNullOrEmpty(_obj.LineName))
                    query = query.Where(x => x.LineName.Contains(_obj.LineName));
                if (!string.IsNullOrEmpty(_obj.InternalLineName))
                    query = query.Where(x => x.InternalLineName.Contains(_obj.InternalLineName));
                if (_obj.NoOfMachine > 0)
                    query = query.Where(x => x.NoOfMachine == _obj.NoOfMachine);
                if (_obj.LineCapacity > 0)
                    query = query.Where(x => x.LineCapacity == _obj.LineCapacity);
                if (!string.IsNullOrEmpty(_obj.LineloadingPoint))
                    query = query.Where(x => x.LineloadingPoint.Contains(_obj.LineloadingPoint));
                if (!string.IsNullOrEmpty(_obj.TabletID))
                    query = query.Where(x => x.TabletID.Contains(_obj.TabletID));

                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostLine")]
        public ActionResult PostLine([FromBody] tbl_Lines _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Lines.Where(x => x.LineName == _obj.LineName && x.ModuleID==_obj.ModuleID).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Lines.OrderBy(x => x.LineID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.LineID) + 1;
                    _obj.LineID = id;

                    _context.tbl_Lines.Add(_obj);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Save Success" });
                }
                else
                {
                    return Ok(new { status = 200, message = "Already Exits" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       

        [HttpPost]
        [Route("PutLine")]
        public ActionResult PutLine([FromBody] tbl_Lines _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Lines.Where(x => x.LineID == _obj.LineID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.ModuleID = _obj.ModuleID;
                    lastrecord.LineName = _obj.LineName;
                    lastrecord.InternalLineName = _obj.InternalLineName;
                    lastrecord.NoOfMachine = _obj.NoOfMachine;
                    lastrecord.LineCapacity = _obj.LineCapacity;
                    lastrecord.LineloadingPoint = _obj.LineloadingPoint;
                    lastrecord.TabletID = _obj.TabletID;
                    _context.tbl_Lines.Update(lastrecord);
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
        [Route("DeleteLine")]
        public ActionResult DeleteLine([FromBody] tbl_Lines _obj)
        {
            try
            {
                IQueryable<tbl_Lines> query = _context.tbl_Lines.Where(x => x.LineID == _obj.LineID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Lines.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostMultiLine")]
        public ActionResult PostMultiLine([FromBody] List<tbl_Lines> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Lines.OrderBy(x => x.LineID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.LineID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Lines onerow in list)
                    {
                        var record = _context.tbl_Lines.Where(x => x.LineName == onerow.LineName && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Lines>();
                        if (record == null)
                        {
                            onerow.LineID = id;
                            onerow.ModuleID = 0;// onerow.ModuleID;
                            onerow.LineName = onerow.LineName;
                            onerow.InternalLineName = onerow.InternalLineName;
                            onerow.NoOfMachine = onerow.NoOfMachine;
                            onerow.LineCapacity = onerow.LineCapacity;
                            onerow.LineloadingPoint = onerow.LineloadingPoint;
                            onerow.TabletID = onerow.TabletID;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            onerow.ModuleName = onerow.ModuleName;
                            onerow.ProcessType = onerow.ProcessType;
                            _context.tbl_Lines.Add(onerow);
                            _context.SaveChanges();
                            id++;
                        }


                    }
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

        [HttpPost]
        [Route("PutMultiLine")]
        public ActionResult PutMultiLine([FromBody] List<tbl_Lines> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Lines onerow in list)
                    {
                        var record = _context.tbl_Lines.Where(x => x.LineID == onerow.LineID).SingleOrDefault();
                        if (record != null)
                        {
                            record.LineName = onerow.LineName;
                            record.InternalLineName = onerow.InternalLineName;
                            record.NoOfMachine = onerow.NoOfMachine;
                            record.LineCapacity = onerow.LineCapacity;
                            record.LineloadingPoint = onerow.LineloadingPoint;
                            record.TabletID = onerow.TabletID;
                            record.ModuleName = onerow.ModuleName;
                            record.UpdatedDate = DateTime.Now.Date;
                            record.ProcessType = onerow.ProcessType;
                            record.DeviceSerialNo = onerow.DeviceSerialNo;
                            _context.tbl_Lines.Update(record);
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
        [Route("GetLine/{factoryid}")]
        public ActionResult GetLine(int factoryid)
        {
            try
            {
                var obj = _context.tbl_Lines.Where(x=>x.FactoryID== factoryid).OrderBy(x => x.LineName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillFactoryLine")]
        public ActionResult FillFactoryLine()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_Lines> query = _context.vw_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.LineName, Text = x.LineName }).Distinct().ToList();
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
