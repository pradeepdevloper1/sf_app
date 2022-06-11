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
    public class ProductFitController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<ProductFitController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ProductFitController(ILogger<ProductFitController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public ActionResult get()
        {
            var obj = _context.tbl_ProductFit.ToList();
            return Ok(obj);
        }


        [HttpPost]
        [Route("GetProductFitList")]
        public ActionResult GetProductFitList([FromBody] tbl_ProductFit _obj)
        {
            try
            {
                IQueryable<tbl_ProductFit> query = _context.tbl_ProductFit;
                if (_obj.ProductFitID > 0)
                    query = query.Where(x => x.ProductFitID == _obj.ProductFitID);
                if (_obj.FactoryID > 0)
                    query = query.Where(x => x.FactoryID == _obj.FactoryID);
                if (!string.IsNullOrEmpty(_obj.FitType))
                    query = query.Where(x => x.FitType.Contains(_obj.FitType));
                var record = query.ToList();
                return Ok(record);
         
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostProductFit")]
        public ActionResult PostProductFit([FromBody] tbl_ProductFit _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_ProductFit.Where(x => x.FitType == _obj.FitType).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_ProductFit.OrderBy(x => x.ProductFitID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.ProductFitID) + 1;
                    _obj.ProductFitID = id;

                    _context.tbl_ProductFit.Add(_obj);
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
        [Route("PostMultiProductFit")]
        public ActionResult PostMultiProductFit([FromBody] List<tbl_ProductFit> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_ProductFit.OrderBy(x => x.ProductFitID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.ProductFitID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_ProductFit onerow in list)
                    {
                        var record = _context.tbl_ProductFit.Where(x => x.FitType == onerow.FitType && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_ProductFit>();
                        if (record == null)
                        {
                            onerow.ProductFitID = id;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.FitType = onerow.FitType;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_ProductFit.Add(onerow);
                            _context.SaveChanges();
                            id++;
                        }
                    }
                }

                return Ok(new { status = 200, message = "Save Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutProductFit")]
        public ActionResult PutProductFit([FromBody] tbl_ProductFit _obj)
        {
            try
            {
                var lastrecord = _context.tbl_ProductFit.Where(x => x.ProductFitID == _obj.ProductFitID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.FactoryID = _obj.FactoryID;
                    lastrecord.FitType = _obj.FitType;
                  
                    _context.tbl_ProductFit.Update(lastrecord);
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
        [Route("DeleteProductFit")]
        public ActionResult DeleteProductFit([FromBody] tbl_ProductFit _obj)
        {
            try
            {
                IQueryable<tbl_ProductFit> query = _context.tbl_ProductFit.Where(x => x.ProductFitID == _obj.ProductFitID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_ProductFit.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillProductFit")]
        public ActionResult FillProductFit()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_ProductFit> query = _context.tbl_ProductFit.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.FitType, Text = x.FitType }).Distinct().ToList();
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
        [Route("FillProductFit")]
        public ActionResult FillProductFit(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_ProductFit> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Fit != null && x.Fit != "");
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
                if (!string.IsNullOrEmpty(_obj.Module))
                    query = query.Where(x => x.Module == _obj.Module);

                if (!string.IsNullOrEmpty(_obj.Line))
                {
                    dataQuery = (IQueryable<tbl_ProductFit>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Fit = p.Fit
                      }
                      into g
                      select new tbl_ProductFit
                      {
                          FitType = g.Key.Fit,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_ProductFit>)
                        query.
                        GroupBy(x => new
                        {
                            Fit = x.Fit
                        }).
                        Select(g => new tbl_ProductFit
                        {
                            FitType = g.Key.Fit
                        });
                }

                data = dataQuery.Select(x => new { ID = x.FitType, Text = x.FitType }).ToList();
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
        [Route("PutMultiProductFit")]
        public ActionResult PutMultiProductFit([FromBody] List<tbl_ProductFit> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_ProductFit onerow in list)
                    {
                        var record = _context.tbl_ProductFit.Where(x => x.ProductFitID == onerow.ProductFitID).SingleOrDefault();
                        if (record != null)
                        {
                            record.FitType = onerow.FitType;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_ProductFit.Update(record);
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
        [Route("GetProductFit/{factoryid}")]
        public ActionResult GetProductFit(int factoryid)
        {
            try
            {

                var obj = _context.tbl_ProductFit.Where(x => x.FactoryID == factoryid).OrderBy(x => x.FitType).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
