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
    public class UploadOBController : ControllerBase
    {
        private readonly ILogger<UploadOBController> _logger;
        private readonly DBContext _context;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();

        public UploadOBController(ILogger<UploadOBController> logger, DBContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet]
        public string TestAPI()
        {
            return "API is working!";
        }

        [HttpPost]
        [Route("GetOBFileUploadList")]
        public ActionResult GetOBFileUploadList([FromBody] SearchModel searchModel)
        {
            IQueryable<tbl_OBFileUpload> query = _context.tbl_OBFileUpload;
            if (searchModel.ProductID > 0)
            {
                query = query.Where(x => x.ProductID == searchModel.ProductID);
            }
            if (!string.IsNullOrEmpty(searchModel.ProcessCode))
            {
                query = query.Where(x => x.ProcessCode == searchModel.ProcessCode);
            }
            var obj = query.OrderBy(x => x.CreatedOn).ToList();
            if (obj == null)
                return Ok(new { status = 400, message = "no Record Found" });

            return Ok(obj);
        }

        [HttpGet]
        [Route("GetOBFileUpdateData/{OBFileUploadID}")]
        public ActionResult GetOBFileUpdateData(Int64 OBFileUploadID)
        {
            var obj = _context.tbl_OBFileUploadData.Where(e => e.OBFileUploadID == OBFileUploadID).ToList();
            if (obj == null)
                return Ok(new { status = 400, message = "no Record Found" });

            return Ok(obj);
        }

        [HttpPost]
        [Route("PostOBFileUploadData")]
        public ActionResult PostOBFileUploadData([FromBody] List<tbl_OBFileUploadData> list)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var record = _context.tbl_OBFileUploadData.Where(x => x.OBFileUploadID == list.FirstOrDefault().OBFileUploadID && x.FactoryID == _UserTokenInfo.FactoryID).ToList();
                _context.tbl_OBFileUploadData.RemoveRange(record);
                _context.SaveChanges();

                Int64 id = 0;
                var lastrecord = _context.tbl_OBFileUploadData.OrderBy(x => x.OBFileUploadDataID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.OBFileUploadDataID) + 1;

                foreach (tbl_OBFileUploadData onerow in list)
                {
                    onerow.OBFileUploadDataID = id;
                    onerow.Section = "NA";
                    onerow.UserID = _UserTokenInfo.UserID;
                    onerow.CreatedOn = System.DateTime.Now.Date;
                    onerow.FactoryID = _UserTokenInfo.FactoryID;
                    id++;
                }

                _context.tbl_OBFileUploadData.AddRange(list);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("PostOBFileUpload")]
        public ActionResult PostOBFileUpload([FromBody] tbl_OBFileUpload data)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var record = _context.tbl_OBFileUpload.Where(x => x.OBFileUploadID == data.OBFileUploadID).FirstOrDefault();
                if(record != null)
                {
                    _context.tbl_OBFileUpload.Remove(record);
                    _context.SaveChanges();
                }

                long id = 0;
                var lastrecord = _context.tbl_OBFileUpload.OrderBy(x => x.OBFileUploadID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.OBFileUploadID) + 1;

                data.OBFileUploadID = id;
                data.UserID = _UserTokenInfo.UserID;
                data.CreatedOn = System.DateTime.Now.Date;
                data.FactoryID = _UserTokenInfo.FactoryID;

                _context.tbl_OBFileUpload.Add(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success", OBFileUploadID = data.OBFileUploadID });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutMultiOBFileUploadData")]
        public ActionResult PutMultiOBFileUploadData([FromBody] List<tbl_OBFileUploadData> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_OBFileUploadData onerow in list)
                    {
                        var record = _context.tbl_OBFileUploadData.Where(x => x.OBFileUploadDataID == onerow.OBFileUploadDataID).SingleOrDefault();
                        if (record != null)
                        {
                            record.OperationCode = onerow.OperationCode;
                            record.OperationName = onerow.OperationName;
                            record.Section = onerow.Section;
                            record.SMV = onerow.SMV;
                            _context.tbl_OBFileUploadData.Update(record);
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
    }
}
