using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OperationsController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<OperationsController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public OperationsController(ILogger<OperationsController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_Operations.ToList();
            return Ok(obj);
        }
        [HttpPost]
        [Route("GetOperationsList")]
        public ActionResult GetOperationsList([FromBody] tbl_Operations _obj)
        {
            try
            {
                IQueryable<tbl_Operations> query = _context.tbl_Operations;
                if (_obj.OperationID > 0)
                    query = query.Where(x => x.OperationID == _obj.OperationID);
                if (!string.IsNullOrEmpty(_obj.OperationName))
                    query = query.Where(x => x.OperationName.Contains(_obj.OperationName));

                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostOperations")]
        public ActionResult PostOperations([FromBody] tbl_Operations _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Operations.Where(x => x.OperationName == _obj.OperationName).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Operations.OrderBy(x => x.OperationID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.OperationID) + 1;
                    _obj.OperationID = id;

                    _context.tbl_Operations.Add(_obj);
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
        [Route("PutOperations")]
        public ActionResult PutOperations([FromBody] tbl_Operations _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Operations.Where(x => x.OperationID == _obj.OperationID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.OperationName = _obj.OperationName;

                    _context.tbl_Operations.Update(lastrecord);
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
        [Route("DeleteOperations")]
        public ActionResult DeleteOperations([FromBody] tbl_Operations _obj)
        {
            try
            {
                IQueryable<tbl_Operations> query = _context.tbl_Operations.Where(x => x.OperationID == _obj.OperationID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Operations.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
