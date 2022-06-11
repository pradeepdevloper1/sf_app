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
    public class FactoryController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<FactoryController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public FactoryController(ILogger<FactoryController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public ActionResult get()
        {
            var obj = _context.tbl_Factory.ToList();
            return Ok(obj);
        }
      

        [HttpPost]
        [Route("GetFactoryList")]
        public ActionResult GetFactoryList([FromBody] tbl_Factory _obj)
        {
            try
            {
                IQueryable<tbl_Factory> query = _context.tbl_Factory;
                if (_obj.FactoryID > 0)
                    query = query.Where(x => x.FactoryID == _obj.FactoryID);
                if (_obj.ClusterID > 0)
                    query = query.Where(x => x.ClusterID == _obj.ClusterID);
                if (!string.IsNullOrEmpty(_obj.FactoryName))
                    query = query.Where(x => x.FactoryName.Contains(_obj.FactoryName));
                if (!string.IsNullOrEmpty(_obj.FactoryAddress))
                    query = query.Where(x => x.FactoryAddress.Contains(_obj.FactoryAddress));
                if (!string.IsNullOrEmpty(_obj.FactoryType))
                    query = query.Where(x => x.FactoryType.Contains(_obj.FactoryType));
                if (!string.IsNullOrEmpty(_obj.FactoryHead))
                    query = query.Where(x => x.FactoryHead.Contains(_obj.FactoryHead));
                if (!string.IsNullOrEmpty(_obj.FactoryEmail))
                    query = query.Where(x => x.FactoryEmail.Contains(_obj.FactoryEmail));
                if (_obj.FactoryContactNumber > 0)
                    query = query.Where(x => x.FactoryContactNumber == _obj.FactoryContactNumber);
                if (!string.IsNullOrEmpty(_obj.FactoryTimeZone))
                    query = query.Where(x => x.FactoryTimeZone.Contains(_obj.FactoryTimeZone));
                if (_obj.NoOfShifts > 0)
                    query = query.Where(x => x.NoOfShifts == _obj.NoOfShifts);
                if (_obj.DecimalValue > 0)
                    query = query.Where(x => x.DecimalValue == _obj.DecimalValue);
                if (_obj.PTMPrice > 0)
                    query = query.Where(x => x.PTMPrice == _obj.PTMPrice);
                if (_obj.NoOfUsers > 0)
                    query = query.Where(x => x.NoOfUsers == _obj.NoOfUsers);
                if (!string.IsNullOrEmpty(_obj.FactoryOffOn))
                    query = query.Where(x => x.FactoryOffOn.Contains(_obj.FactoryOffOn));
                if (!string.IsNullOrEmpty(_obj.MeasuringUnit))
                    query = query.Where(x => x.MeasuringUnit.Contains(_obj.MeasuringUnit));
                if (_obj.DataScale > 0)
                    query = query.Where(x => x.DataScale == _obj.DataScale);

                var record = query.ToList();
                return Ok(record);
             
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostFactory")]
        public ActionResult PostFactory([FromBody] tbl_Factory _obj)
        {
            try
            {
                int id = 0;
                var data = _context.tbl_Factory.Where(x => x.FactoryName == _obj.FactoryName).FirstOrDefault();

                if (data == null)
                {
                    var lastrecord = _context.tbl_Factory.OrderBy(x => x.FactoryID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.FactoryID) + 1;
                    _obj.FactoryID = id;
                    _obj.FactoryHead = "-";//_obj.FactoryHead;
                    _obj.FactoryEmail = "-";//_obj.FactoryEmail;
                    _obj.FactoryContactNumber = 0;// _obj.FactoryContactNumber;
                    _obj.NoOfShifts = 0;
                    _obj.DecimalValue = 0;// _obj.NoOfShifts;
                    _obj.PTMPrice = 0;// _obj.PTMPrice;
                    _obj.NoOfUsers = 0;// _obj.NoOfUsers;
                    _obj.FactoryOffOn = "-";// _obj.FactoryOffOn;
                    _obj.MeasuringUnit = "-";//_obj.MeasuringUnit;
                    _obj.DataScale = 0;// _obj.DataScale;
                    _context.tbl_Factory.Add(_obj);
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

        [HttpPost]
        [Route("PutFactory")]
        public ActionResult PutFactory([FromBody] tbl_Factory _obj)
        {
            try
            {
                var lastrecord = _context.tbl_Factory.Where(x => x.FactoryID == _obj.FactoryID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.ClusterID = _obj.ClusterID;
                    lastrecord.FactoryName = _obj.FactoryName;
                    lastrecord.FactoryAddress = _obj.FactoryAddress;
                    lastrecord.FactoryType = _obj.FactoryType;
                    //lastrecord.FactoryHead = _obj.FactoryHead;
                    //lastrecord.FactoryEmail = _obj.FactoryEmail;
                    //lastrecord.FactoryContactNumber = _obj.FactoryContactNumber;
                    lastrecord.FactoryTimeZone = _obj.FactoryTimeZone;
                    lastrecord.FactoryCountry = _obj.FactoryCountry;
                    //lastrecord.NoOfShifts = _obj.NoOfShifts;
                    //lastrecord.PTMPrice = _obj.PTMPrice;
                    //lastrecord.NoOfUsers = _obj.NoOfUsers;
                    //lastrecord.FactoryOffOn = _obj.FactoryOffOn;
                    //lastrecord.MeasuringUnit = _obj.MeasuringUnit;
                    //lastrecord.DataScale = _obj.DataScale;
                    lastrecord.NoOfLine = _obj.NoOfLine;
                    lastrecord.SmartLines = _obj.SmartLines;
                    lastrecord.LinkedwithERP = _obj.LinkedwithERP;
                    _context.tbl_Factory.Update(lastrecord);
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
        [Route("DeleteFactory")]
        public ActionResult DeleteFactory([FromBody] tbl_Orders _obj)
        {
            try
            {
                IQueryable<tbl_Factory> query = _context.tbl_Factory.Where(x => x.FactoryID == _obj.FactoryID);
                var data = query.FirstOrDefault();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                _context.tbl_Factory.RemoveRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetFactory")]
        public ActionResult GetFactory()
        {
            try
            {
                IQueryable<vw_Factory> query = _context.vw_Factory;
                var data = query.OrderByDescending(x => x.FactoryID).ToList();
                if (data.Count <= 0) { return Ok(new { status = 400, message = "No record found." }); }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("FactoryView")]
        public ActionResult FactoryView([FromBody] vw_Factory _obj)
        {
            try
            {
                IQueryable<vw_Factory> query = _context.vw_Factory.Where(x=>x.FactoryID==_obj.FactoryID);
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
        [Route("FactoryView/{factoryid}")]
        public ActionResult FactoryView(int factoryid)
        {
            try
            {
                IQueryable<vw_Factory> query = _context.vw_Factory.Where(x => x.FactoryID == factoryid);
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
        [Route("ExcelUpload/{factoryid}")]
        public ActionResult ExcelUpload(int factoryid)
        {
            AdminExcelModel data = new AdminExcelModel();

            var product = _context.tbl_Products.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Products>();
            var fit = _context.tbl_ProductFit.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_ProductFit>();
            var customer = _context.tbl_Customer.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Customer>();
            var line = _context.tbl_Lines.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Lines>();
            var shift = _context.tbl_Shift.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Shift>();
            var qccode = _context.tbl_Defects.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Defects>();
            var user = _context.tbl_Users.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Users>();
            var module = _context.tbl_Modules.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_Modules>();
            var processdefinition = _context.tbl_ProcessDefinition.Where(x => x.FactoryID == factoryid).FirstOrDefault<tbl_ProcessDefinition>();

            //Product
            data.Product_Count = _context.tbl_Products.Where(x=>x.FactoryID== factoryid).Count();

            if (data.Product_Count > 0)
            {
                data.Product_Status = "Uploaded";
                data.Product_CreateDate = product.CreatedDate.ToString("MM/dd/yyyy");
                data.Product_UpdateDate = product.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Product_Status = "Pending";
                data.Product_CreateDate = "";
                data.Product_UpdateDate = "";
            }
          
            //Fit
            data.Fit_Count = _context.tbl_ProductFit.Where(x=>x.FactoryID== factoryid).Count();
            if (data.Fit_Count > 0)
            {
                data.Fit_Status = "Uploaded";
                data.Fit_CreateDate = fit.CreatedDate.ToString("MM/dd/yyyy");
                data.Fit_UpdateDate = fit.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            { 
                data.Fit_Status = "Pending";
                data.Fit_CreateDate = "";
                data.Fit_UpdateDate = "";
            }
           

            //Customer
            data.Customer_Count = _context.tbl_Customer.Where(x => x.FactoryID == factoryid).Count();
            if (data.Customer_Count > 0)
            {
                data.Customer_Status = "Uploaded";
                data.Customer_CreateDate = customer.CreatedDate.ToString("MM/dd/yyyy");
                data.Customer_UpdateDate = customer.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Customer_Status = "Pending";
                data.Customer_CreateDate = "";
                data.Customer_UpdateDate = "";
            }
        

            //Line
            data.Line_Count = _context.tbl_Lines.Where(x => x.FactoryID == factoryid).Count();
            if (data.Line_Count > 0)
            {
                data.Line_Status = "Uploaded";
                data.Line_CreateDate = line.CreatedDate.ToString("MM/dd/yyyy");
                data.Line_UpdateDate = line.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Line_Status = "Pending";
                data.Line_CreateDate = "";
                data.Line_UpdateDate = "";
            }
           

            //Shift
            data.Shift_Count = _context.tbl_Shift.Where(x => x.FactoryID == factoryid).Count();
            if (data.Shift_Count > 0)
            {
                data.Shift_Status = "Uploaded";
                data.Shift_CreateDate = shift.CreatedDate.ToString("MM/dd/yyyy");
                data.Shift_UpdateDate = shift.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Shift_Status = "Pending";
                data.Shift_CreateDate = "";
                data.Shift_UpdateDate = "";
            }
       

            //QcCode
            data.QCCode_Count = _context.tbl_Defects.Where(x => x.FactoryID == factoryid).Count();
            if (data.QCCode_Count > 0)
            {
                data.QCCode_Status = "Uploaded";
                data.QCCode_CreateDate = qccode.CreatedDate.ToString("MM/dd/yyyy");
                data.QCCode_UpdateDate = qccode.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.QCCode_Status = "Pending";
                data.QCCode_CreateDate = "";
                data.QCCode_UpdateDate = "";
            }
          

            //User
            data.User_Count = _context.tbl_Users.Where(x => x.FactoryID == factoryid).Count();
            if (data.User_Count > 0)
            {
                data.User_Status = "Uploaded";
                data.User_CreateDate = user.CreatedDate.ToString("MM/dd/yyyy");
                data.User_UpdateDate = user.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.User_Status = "Pending";
                data.User_CreateDate = "";
                data.User_UpdateDate = "";
            }
         
            //Holidays
            data.Holidays_Count = 1;
            if (data.Holidays_Count > 0)
            {
                data.Holidays_Status = "Uploaded";
                data.Holidays_CreateDate = DateTime.Now.ToString("MM/dd/yyyy");
                data.Holidays_UpdateDate = DateTime.Now.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Holidays_Status = "Pending";
                data.User_CreateDate = "";
                data.User_UpdateDate = "";
            }
            data.Module_Count = _context.tbl_Modules.Where(x => x.FactoryID == factoryid).Count();
            if (data.Module_Count > 0)
            {
                data.Module_Status = "Uploaded";
                data.Module_CreateDate =  module.CreatedDate.ToString("MM/dd/yyyy");
                data.Module_UpdateDate = module.UpdatedDate.ToString("MM/dd/yyyy");
            }
            else
            {
                data.Module_Status = "Pending";
                data.Module_CreateDate = "";
                data.Module_UpdateDate = "";
            }
            data.processDefinition_Count = _context.tbl_ProcessDefinition.Where(x => x.FactoryID == factoryid).Count();
            if (data.processDefinition_Count > 0)
            {
                data.processDefinition_Status = "Uploaded";
                data.processDefinition_CreatedOn = processdefinition.CreatedOn.ToString("MM/dd/yyyy");
                data.processDefinition_LastChangedOn = processdefinition.LastChangedOn.ToString("MM/dd/yyyy");
            }
            else
            {
                data.processDefinition_Status = "Pending";
                data.processDefinition_CreatedOn = "";
                data.processDefinition_LastChangedOn = "";
            }
            return Ok(data);
        }
        [HttpGet]
        [Route("GetLinkedWithERPByFactoryID/{factoryid}")]
        public ActionResult GetLinkedWithERPByFactoryID(int factoryid)
        {
            try
            {
                IQueryable<tbl_Factory> query = _context.tbl_Factory.Where(x => x.FactoryID == factoryid);
                var data = query.Select(x => new { Id = x.LinkedwithERP, Text = x.LinkedwithERP }).ToList();
                if (data.Count == 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 401, message = ex.Message });
            }
        }
    }
}
