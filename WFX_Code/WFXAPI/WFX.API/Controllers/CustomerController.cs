using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public CustomerController(IWebHostEnvironment env, ILogger<CustomerController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Customer API Call");
        }

        [Route("PostIsCustomerExist")]
        [HttpPost]
        public ActionResult PostIsCustomerExist([FromBody] tbl_Customer _obj)
        {
            try
            {
                var data = _context.tbl_Customer.Where(x => x.CustomerName == _obj.CustomerName).FirstOrDefault();
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
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult get()
        {
            var obj = _context.tbl_Customer.ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetCustomerList")]
        public ActionResult GetCustomerList([FromBody] tbl_Customer _obj)
        {
            try
            {
                IQueryable<tbl_Customer> query = _context.tbl_Customer;
                if (_obj.CustomerID > 0)
                    query = query.Where(x => x.CustomerID == _obj.CustomerID);
                if (_obj.FactoryID > 0)
                    query = query.Where(x => x.FactoryID == _obj.FactoryID);
                if (!string.IsNullOrEmpty(_obj.CustomerName))
                    query = query.Where(x => x.CustomerName.Contains(_obj.CustomerName));

                var record = query.ToList();
                return Ok(record);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostCustomer")]
        public ActionResult PostCustomer([FromBody] tbl_Customer _obj)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_Customer.OrderBy(x => x.CustomerID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.CustomerID) + 1;
                _obj.CustomerID = id;

                _context.tbl_Customer.Add(_obj);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Save Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("PutCustomer")]
        public ActionResult PutCustomer([FromBody] tbl_Customer _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Customer.Where(x => x.CustomerID == _obj.CustomerID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.FactoryID = _obj.FactoryID;
                    lastrecord.CustomerName = _obj.CustomerName;

                    _context.tbl_Customer.Update(lastrecord);
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

        //[HttpGet]
        //[Route("GetLineList")]
        //public ActionResult GetLineList()
        //{
        //    try
        //    {
        //        _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
        //        IQueryable<vw_Lines> query = _context.vw_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
        //        var data = query.Select(x => new { Id = x.LineID, Value = x.LineName }).Distinct().ToList();
        //        if (data.Count <= 0)
        //            return Ok(new { status = 400, message = "No record found." });

        //        return Ok(new { status = 200, message = "Success", data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("DeleteCustomer")]
        public ActionResult DeleteCustomer([FromBody] tbl_Customer _obj)
        {
            try
            {
                IQueryable<tbl_Customer> query = _context.tbl_Customer.Where(x => x.CustomerID == _obj.CustomerID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Customer.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Delete Success" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FillCustomer")]
        public ActionResult FillCustomer()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Customer> query = _context.tbl_Customer.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.CustomerName, Text = x.CustomerName }).Distinct().ToList();
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
        [Route("FillCustomer")]
        public ActionResult FillCustomer(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Customer> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Customer != null && x.Customer != "");
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
                if (!string.IsNullOrEmpty(_obj.Module))
                    query = query.Where(x => x.Module == _obj.Module);

                if (!string.IsNullOrEmpty(_obj.Line))
                {
                    dataQuery = (IQueryable<tbl_Customer>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Customer = p.Customer
                      }
                      into g
                      select new tbl_Customer
                      {
                          CustomerName = g.Key.Customer,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Customer>)
                        query.
                        GroupBy(x => new
                        {
                            Customer = x.Customer
                        }).
                        Select(g => new tbl_Customer
                        {
                            CustomerName = g.Key.Customer
                        });
                }

                data = dataQuery.Select(x => new { ID = x.CustomerName, Text = x.CustomerName }).ToList();
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
        [Route("PostMultiCustomer")]
        public ActionResult PostMultiCustomer([FromBody] List<tbl_Customer> list)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_Customer.OrderBy(x => x.CustomerID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.CustomerID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Customer onerow in list)
                    {

                        var record = _context.tbl_Customer.Where(x => x.CustomerName == onerow.CustomerName && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Customer>();
                        if (record == null)
                        {
                            onerow.CustomerID = id;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.CustomerName = onerow.CustomerName;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Customer.Add(onerow);
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
        [Route("PutMultiCustomer")]
        public ActionResult PutMultiCustomer([FromBody] List<tbl_Customer> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Customer onerow in list)
                    {
                        var record = _context.tbl_Customer.Where(x => x.CustomerID == onerow.CustomerID).SingleOrDefault();
                        if (record != null)
                        {
                            record.CustomerName = onerow.CustomerName;
                            record.UpdatedDate = DateTime.Now.Date;
                            _context.tbl_Customer.Update(record);
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
        [Route("GetCustomer/{factoryid}")]
        public ActionResult GetCustomer(int factoryid)
        {
            try
            {
                var obj = _context.tbl_Customer.Where(x => x.FactoryID == factoryid && x.CustomerName != null).OrderBy(x => x.CustomerName).ToList();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
