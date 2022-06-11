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
    public class ProductController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<ProductController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ProductController(ILogger<ProductController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public ActionResult get()
        {
            var obj = _context.tbl_Products.ToList();
            return Ok(obj);
        }


        [HttpPost]
        [Route("GetProductList")]
        public ActionResult GetProductList([FromBody] tbl_Products _obj)
        {
            try
            {
                IQueryable<tbl_Products> query = _context.tbl_Products;
                if (_obj.ProductID > 0)
                    query = query.Where(x => x.ProductID == _obj.ProductID);
                if (_obj.FactoryID > 0)
                    query = query.Where(x => x.FactoryID == _obj.FactoryID);
                if (!string.IsNullOrEmpty(_obj.ProductName))
                    query = query.Where(x => x.ProductName.Contains(_obj.ProductName));
                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetProduct/{factoryid}")]
        public ActionResult GetProduct(int factoryid)
        {
            try
            {
              
                var obj = _context.tbl_Products.Where(x => x.FactoryID == factoryid).OrderBy(x => x.ProductName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("PostProduct")]
        public ActionResult PostProduct([FromBody] tbl_Products _obj)
        {
            try
            {
                long id = 0;
                var data = _context.tbl_Products.Where(x => x.ProductName == _obj.ProductName).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Products.OrderBy(x => x.ProductID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.ProductID) + 1;
                    _obj.ProductID = id;
                    _obj.FactoryID = 1;
                    _context.tbl_Products.Add(_obj);
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
        [Route("PostMultiProduct")]
        public ActionResult PostMultiProduct([FromBody] List<tbl_Products> list)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_Products.OrderBy(x => x.ProductID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.ProductID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Products onerow in list)
                    {
                        var record = _context.tbl_Products.Where(x => x.ProductName == onerow.ProductName && x.FactoryID== onerow.FactoryID).FirstOrDefault<tbl_Products>();
                        if (record == null)
                        {
                            onerow.ProductID = id;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.ProductName = onerow.ProductName;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Products.Add(onerow);
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
        [Route("PutProduct")]
        public ActionResult PutProduct([FromBody] tbl_Products _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Products.Where(x => x.ProductID == _obj.ProductID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.FactoryID = _obj.FactoryID;
                    lastrecord.ProductName = _obj.ProductName;

                    _context.tbl_Products.Update(lastrecord);
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
        [Route("DeleteProduct")]
        public ActionResult DeleteProduct([FromBody] tbl_Products _obj)
        {
            try
            {
                IQueryable<tbl_Products> query = _context.tbl_Products.Where(x => x.ProductID == _obj.ProductID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Products.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutMultiProduct")]
        public ActionResult PutMultiProduct([FromBody] List<tbl_Products> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Products onerow in list)
                    {
                        var record = _context.tbl_Products.Where(x => x.ProductID == onerow.ProductID).SingleOrDefault();
                        if (record != null)
                        {
                            record.ProductName = onerow.ProductName;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Products.Update(record);
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
        [Route("FillProduct")]
        public ActionResult FillProduct()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Products> query = _context.tbl_Products.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID );
                var data = query.Select(x => new { Id = x.ProductName, Text = x.ProductName }).Distinct().ToList();
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
        [Route("FillProduct")]
        public ActionResult FillProduct(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Products> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Product != null && x.Product != "");
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
                    dataQuery = (IQueryable<tbl_Products>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Product = p.Product
                      }
                      into g
                      select new tbl_Products
                      {
                          ProductName = g.Key.Product,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Products>)
                        query.
                        GroupBy(x => new
                        {
                            Product = x.Product
                        }).
                        Select(g => new tbl_Products
                        {
                            ProductName = g.Key.Product
                        });
                }

                data = dataQuery.Select(x => new { ID = x.ProductName, Text = x.ProductName }).ToList();
                if (data == null)
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
