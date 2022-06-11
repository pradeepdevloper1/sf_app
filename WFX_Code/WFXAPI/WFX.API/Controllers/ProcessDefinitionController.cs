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
    public class ProcessDefinitionController : ControllerBase

    {
        DBContext _context;
        private readonly ILogger<ProcessDefinitionController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ProcessDefinitionController(IWebHostEnvironment env, ILogger<ProcessDefinitionController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;

        }
        [HttpPost]
        [Route("PostMultiProcessDefinition")]
        public ActionResult PostMultiProcessDefinition([FromBody] List<tbl_ProcessDefinition> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_ProcessDefinition.OrderBy(x => x.ProcessDefinitionID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.ProcessDefinitionID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_ProcessDefinition onerow in list)
                    {
                        var record = _context.tbl_ProcessDefinition.Where(x => x.ProcessCode == onerow.ProcessCode && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_ProcessDefinition>();
                        if (record == null)
                        {
                            onerow.ProcessDefinitionID = id;
                            onerow.ProcessName = onerow.ProcessName;
                            onerow.ProcessCode = onerow.ProcessCode;
                            onerow.ProcessType = onerow.ProcessType;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CreatedOn = DateTime.Now.Date;
                            onerow.LastChangedOn = DateTime.Now.Date;
                            _context.tbl_ProcessDefinition.Add(onerow);
                            _context.SaveChanges();
                            id++;
                        }
                    }
                }

                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("PutMultiProcessDefinition")]
        public ActionResult PutMultiProcessDefinition([FromBody] List<tbl_ProcessDefinition> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_ProcessDefinition onerow in list)
                    {
                        var record = _context.tbl_ProcessDefinition.Where(x => x.ProcessDefinitionID == onerow.ProcessDefinitionID).SingleOrDefault();
                        if (record != null)
                        {
                            record.ProcessName = onerow.ProcessName;
                            record.ProcessCode = onerow.ProcessCode;
                            record.ProcessType = onerow.ProcessType;
                            record.LastChangedOn = DateTime.Now.Date;
                            _context.tbl_ProcessDefinition.Update(record);
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetProcessDefinitionList/{factoryid}")]
        public ActionResult GetProcessDefinitionList(int factoryid)
        {
            try
            {
                IQueryable<tbl_ProcessDefinition> query = _context.tbl_ProcessDefinition;
                if( factoryid > 0)
                {
                    query = query.Where(x => x.FactoryID == factoryid);
                }
                var obj = query.OrderBy(x => x.ProcessDefinitionID).ToList();
                if (obj == null)
                    return Ok(new { status = 400, message = "no Record Found" });

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
    }
}
