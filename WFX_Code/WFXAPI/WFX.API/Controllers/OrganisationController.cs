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
    public class OrganisationController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<OrganisationController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public OrganisationController(ILogger<OrganisationController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_Organisations.ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetOrganisationList")]
        public ActionResult GetOrganisationList([FromBody] tbl_Organisations _obj)
        {
            try
            {
                IQueryable<tbl_Organisations> query = _context.tbl_Organisations;
                if (_obj.OrganisationID > 0)
                    query = query.Where(x => x.OrganisationID == _obj.OrganisationID);
                if (!string.IsNullOrEmpty(_obj.OrganisationName))
                    query = query.Where(x => x.OrganisationName.Contains(_obj.OrganisationName));
                if (!string.IsNullOrEmpty(_obj.OrganisationAddress))
                    query = query.Where(x => x.OrganisationAddress.Contains(_obj.OrganisationAddress));
               
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostOrganisation")]
        public ActionResult PostOrganisation([FromBody] tbl_Organisations _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Organisations.Where(x => x.OrganisationName == _obj.OrganisationName).FirstOrDefault();
               
                if (data == null)
                {
                    var lastrecord = _context.tbl_Organisations.OrderBy(x => x.OrganisationID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.OrganisationID) + 1;
                    _obj.OrganisationID = id;
                    _obj.OrganisationAddress = "-";
                  
                    _context.tbl_Organisations.Add(_obj);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Success" });
                }
                else
                {
                    return Ok(new { status = 400, message = "Already Exits" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("AdminDashboard")]
        public ActionResult AdminDashboard()
        {
            AdminDashboardModel data = new AdminDashboardModel();
            data.Organisations_Count = _context.tbl_Organisations.Count();
            data.Clusters_Count = _context.tbl_Clusters.Count();
            data.Factories_Count = _context.tbl_Factory.Count();
            return Ok(data);
        }

        [HttpPost]
        [Route("PutOrganisation")]
        public ActionResult PutOrganisation([FromBody] tbl_Organisations _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Organisations.Where(x => x.OrganisationID == _obj.OrganisationID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.OrganisationName = _obj.OrganisationName;
                    //lastrecord.OrganisationAddress = _obj.OrganisationAddress;
                    lastrecord.OrganisationLogoPath = _obj.OrganisationLogoPath;
                    _context.tbl_Organisations.Update(lastrecord);
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
        [Route("DeleteOrganisation")]
        public ActionResult DeleteOrganisation([FromBody] tbl_Organisations _obj)
        {
            try
            {
                var existing = _context.tbl_Clusters.Where(x => x.OrganisationID == _obj.OrganisationID).SingleOrDefault();
                if (existing != null)
                {
                    return Ok(new { status = 400, message = "Organisation in use." });

                }
                else
                {
                    IQueryable<tbl_Organisations> query = _context.tbl_Organisations.Where(x => x.OrganisationID == _obj.OrganisationID);
                    var data = query.FirstOrDefault();
                    _context.tbl_Organisations.RemoveRange(data);
                    _context.SaveChanges();
                    return Ok(new { status = 200, message = "Successs" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillOrganisation")]
        public ActionResult FillOrganisation()
        {
            try
            {
                IQueryable<tbl_Organisations> query = _context.tbl_Organisations;
                var data = query.Select(x => new { Id = x.OrganisationID, Text = x.OrganisationName }).ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetOrganisationListView")]
        public ActionResult GetOrganisationListView()
        {
            try
            {
                IQueryable<vw_Organisation> query = _context.vw_Organisation;
                var data = query.OrderByDescending(x => x.OrganisationID).ToList();
                if (data.Count <= 0) { return Ok(new { status = 400, message = "No record found." }); }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("OrganisationView/{orgid}")]
        public ActionResult OrganisationView(int orgid)
        {
            try
            {
                IQueryable<tbl_Organisations> query = _context.tbl_Organisations.Where(x => x.OrganisationID == orgid);
                var data = query.SingleOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
