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
    public class ModuleController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<ModuleController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ModuleController(IWebHostEnvironment env, ILogger<ModuleController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Module API Call");
        }

        [Route("PostIsModuleExist")]
        [HttpPost]
        public ActionResult PostIsModuleExist([FromBody] tbl_Modules _obj)
        {
            try
            {
                var data = _context.tbl_Modules.Where(x => x.FactoryID == _obj.FactoryID  &&  x.ModuleName == _obj.ModuleName  ).FirstOrDefault();
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
        [Route("FillModule")]
        public ActionResult FillModule()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_Module> query = _context.vw_Module.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID );
                var data = query.Select(x => new { Id = x.ModuleName, Text = x.ModuleName }).Distinct().ToList();
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
        [Route("FillModule")]
        public ActionResult FillModule(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Modules> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Module != null && x.Module != "");
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                if (!string.IsNullOrEmpty(_obj.Product))
                    query = query.Where(x => x.Product == _obj.Product);
                if (!string.IsNullOrEmpty(_obj.Style))
                    query = query.Where(x => x.Style == _obj.Style);
                if (!string.IsNullOrEmpty(_obj.Fit))
                    query = query.Where(x => x.Fit == _obj.Fit);
                if (!string.IsNullOrEmpty(_obj.Season))
                    query = query.Where(x => x.Season == _obj.Season);
                if (!string.IsNullOrEmpty(_obj.Customer))
                    query = query.Where(x => x.Customer == _obj.Customer);

                if (!string.IsNullOrEmpty(_obj.Line))
                {
                    dataQuery = (IQueryable<tbl_Modules>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Module = p.Module
                      }
                      into g
                      select new tbl_Modules
                      {
                          ModuleName = g.Key.Module,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Modules>)
                        query.
                        GroupBy(x => new
                        {
                            Module = x.Module
                        }).
                        Select(g => new tbl_Modules
                        {
                            ModuleName = g.Key.Module
                        });
                }

                data = dataQuery.Select(x => new { ID = x.ModuleName, Text = x.ModuleName }).ToList();
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
        [Route("GetModuleList")]
        public ActionResult GetModuleList([FromBody] tbl_Modules _obj)
        {
            try
            {
                IQueryable<tbl_Modules> query = _context.tbl_Modules;
                if (_obj.ModuleID > 0)
                    query = query.Where(x => x.ModuleID == _obj.ModuleID);
                if (_obj.DepartmentID > 0)
                    query = query.Where(x => x.DepartmentID == _obj.DepartmentID);
             
                if (!string.IsNullOrEmpty(_obj.ModuleName))
                    query = query.Where(x => x.ModuleName.Contains(_obj.ModuleName));
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostModule")]
        public ActionResult PostModule([FromBody] tbl_Modules _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Modules.Where(x => x.ModuleName == _obj.ModuleName).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Modules.OrderBy(x => x.ModuleID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.ModuleID) + 1;
                    _obj.ModuleID = id;

                    _context.tbl_Modules.Add(_obj);
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

        [HttpPut]
        [Route("PutModule")]
        public ActionResult PutModule([FromBody] tbl_Modules _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Modules.Where(x => x.ModuleID == _obj.ModuleID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.DepartmentID = _obj.DepartmentID;
                    lastrecord.ModuleName = _obj.ModuleName;
         
                    _context.tbl_Modules.Update(lastrecord);
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
        [Route("DeleteModule")]
        public ActionResult DeleteModule([FromBody] tbl_Modules _obj)
        {
            try
            {
                IQueryable<tbl_Modules> query = _context.tbl_Modules.Where(x => x.ModuleID == _obj.ModuleID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Modules.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Successs" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillFactoryModule")]
        public ActionResult FillFactoryModule()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_Module> query = _context.vw_Module.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.ModuleName, Text = x.ModuleName }).Distinct().ToList();
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
        [Route("PostMultiModule")]
        public ActionResult PostMultiModule([FromBody] List<tbl_Modules> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Modules.OrderBy(x => x.ModuleID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.ModuleID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Modules onerow in list)
                    {
                        var record = _context.tbl_Modules.Where(x => x.ModuleName == onerow.ModuleName && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Modules>();
                        if (record == null)
                        {
                            onerow.ModuleID = id;
                            onerow.DepartmentID = onerow.DepartmentID;
                            onerow.ModuleName = onerow.ModuleName;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Modules.Add(onerow);
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

        [HttpGet]
        [Route("GetModuleList/{factoryid}")]
        public ActionResult GetModuleList(int factoryid)
        {
            try
            {
                var obj = _context.tbl_Modules.Where(x => x.FactoryID == factoryid).OrderBy(x => x.ModuleName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutMultiModule")]
        public ActionResult PutMultiModule([FromBody] List<tbl_Modules> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Modules onerow in list)
                    {
                        var record = _context.tbl_Modules.Where(x => x.ModuleID == onerow.ModuleID).SingleOrDefault();
                        if (record != null)
                        {
                            record.DepartmentID = onerow.DepartmentID;
                            record.ModuleName = onerow.ModuleName;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Modules.Update(record);
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
