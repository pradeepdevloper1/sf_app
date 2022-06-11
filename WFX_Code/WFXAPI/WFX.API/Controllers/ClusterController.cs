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
    public class ClusterController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<ClusterController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ClusterController(ILogger<ClusterController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public ActionResult get()
        {
            var obj = _context.tbl_Clusters.ToList();
            return Ok(new {obj });
        }

        [HttpPost]
        [Route("GetClusterList")]
        public ActionResult GetClusterList([FromBody] tbl_Clusters _obj)
        {
            try
            {
                IQueryable<tbl_Clusters> query = _context.tbl_Clusters;
                if (_obj.ClusterID > 0)
                    query = query.Where(x => x.ClusterID == _obj.ClusterID);
                if (_obj.OrganisationID > 0)
                    query = query.Where(x => x.OrganisationID == _obj.OrganisationID);
                if (!string.IsNullOrEmpty(_obj.ClusterName))
                    query = query.Where(x => x.ClusterName.Contains(_obj.ClusterName));
                if (!string.IsNullOrEmpty(_obj.ClusterHead))
                    query = query.Where(x => x.ClusterHead.Contains(_obj.ClusterHead));
                if (!string.IsNullOrEmpty(_obj.ClusterEmail))
                    query = query.Where(x => x.ClusterEmail.Contains(_obj.ClusterEmail));
                if (!string.IsNullOrEmpty(_obj.ClusterRegion))
                    query = query.Where(x => x.ClusterRegion.Contains(_obj.ClusterRegion));

                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostCluster")]
        public ActionResult PostCluster([FromBody] tbl_Clusters _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Clusters.Where(x => x.ClusterName == _obj.ClusterName ).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Clusters.OrderBy(x => x.ClusterID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.ClusterID) + 1;
                    _obj.ClusterID = id;
                    _obj.ClusterHead = "-";
                    _obj.ClusterRegion = "-";
                    _obj.ClusterEmail = "-";
                    _context.tbl_Clusters.Add(_obj);
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

        [HttpPost]
        [Route("PutCluster")]
        public ActionResult PutCluster([FromBody] tbl_Clusters _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Clusters.Where(x => x.ClusterID == _obj.ClusterID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.OrganisationID = _obj.OrganisationID;
                    lastrecord.ClusterName = _obj.ClusterName;
                    //lastrecord.ClusterHead = _obj.ClusterHead;
                    //lastrecord.ClusterEmail = _obj.ClusterEmail;
                    //lastrecord.ClusterRegion = _obj.ClusterRegion;
                    _context.tbl_Clusters.Update(lastrecord);
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
        [Route("DeleteCluster")]
        public ActionResult DeleteCluster([FromBody] tbl_Clusters _obj)
        {
            try
            {
                IQueryable<tbl_Clusters> query = _context.tbl_Clusters.Where(x => x.ClusterID == _obj.ClusterID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Clusters.Remove(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Cluster" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillCulster/{orgid}")]
        public ActionResult FillCulster(int orgid)
        {
            try
            {
                IQueryable<tbl_Clusters> query = _context.tbl_Clusters.Where(x=>x.OrganisationID== orgid);
                var data = query.Select(x => new { Id = x.ClusterID, Text = x.ClusterName }).ToList();
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
        [Route("CulsterView")]
        public ActionResult CulsterView([FromBody] vw_Culster _obj)
        {
            try
            {
                IQueryable<vw_Culster> query = _context.vw_Culster;
                if (!string.IsNullOrEmpty(_obj.ClusterName))
                    query = query.Where(x => x.ClusterName == _obj.ClusterName);
                if (!string.IsNullOrEmpty(_obj.OrganisationName))
                    query = query.Where(x => x.OrganisationName == _obj.OrganisationName);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetCulsterListView")]
        public ActionResult GetCulsterListView()
        {
            try
            {
                IQueryable<vw_Culster> query = _context.vw_Culster;
                var data = query.OrderByDescending(x => x.ClusterID).ToList();
                if (data.Count <= 0) { return Ok(new { status = 400, message = "No record found." }); }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("CulsterView/{clusterid}")]
        public ActionResult CulsterView(int clusterid)
        {
            try
            {
                IQueryable<tbl_Clusters> query = _context.tbl_Clusters.Where(x => x.ClusterID == clusterid);
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

        [HttpGet]
        [Route("GetOrganisationID/{clusterid}")]
        public ActionResult GetOrganisationID(int clusterid)
        {
            try
            {
                int organisationid = 0;
                var last = _context.tbl_Clusters.Where(x=>x.ClusterID== clusterid).SingleOrDefault();
                organisationid = (last == null ? 0 : last.OrganisationID) ;
                return Ok(organisationid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
