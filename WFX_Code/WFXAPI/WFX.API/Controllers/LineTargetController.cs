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
using System.Dynamic;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LineTargetController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<LineTargetController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public LineTargetController(IWebHostEnvironment env, ILogger<LineTargetController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("API Call");
        }

        [HttpPost]
        [Route("PostLineTarget")]
        public ActionResult PostLineTarget([FromBody] List<tbl_LineTarget> list)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_LineTarget.OrderBy(x => x.LineTargetID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.LineTargetID) + 1;
                foreach (tbl_LineTarget onerow in list)
                {
                    var obj = _context.tbl_LineTarget.Where(x => x.FactoryID == onerow.FactoryID && x.PONo == onerow.PONo && x.Date.Date == onerow.Date.Date && x.ShiftName == onerow.ShiftName && x.Color == onerow.Color && x.Module == onerow.Module && x.Line == onerow.Line).FirstOrDefault();
                    if (obj != null)
                    {
                        _context.tbl_LineTarget.Remove(obj);
                    }
                    onerow.LineTargetID = id;
                    onerow.EntryDate = System.DateTime.Now.Date;
                    id++;
                }
                _context.tbl_LineTarget.AddRange(list);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Target Save" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("GetLineTargetList")]
        public ActionResult GetLineTargetList([FromBody] SearchModel _obj)
        {
            try
            {
                List<dynamic> LineTargetList = new List<dynamic>();

                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var query = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                query = query.Where(x => x.Date >= Convert.ToDateTime(_obj.TargetStartDate) && x.Date <= Convert.ToDateTime(_obj.TargetEndDate));
                if (!string.IsNullOrEmpty(_obj.Line))
                    query = query.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Module))
                    query = query.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ShiftName))
                    query = query.Where(x => x.ShiftName == _obj.ShiftName);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                var g   = query.ToList();
                foreach (var p in g)
                {
                    dynamic dynamicObj = new ExpandoObject();
                    dynamicObj.LineTargetID = p.LineTargetID;
                    dynamicObj.factoryid = p.FactoryID;
                    dynamicObj.line = p.Line;
                    dynamicObj.module = p.Module;
                    dynamicObj.ProcessName = p.ProcessName;
                    dynamicObj.ProcessCode = p.ProcessCode;
                    dynamicObj.section = p.Section;
                    dynamicObj.style = p.Style;
                    dynamicObj.poNo = p.PONo;
                    dynamicObj.soNo = p.SONo;
                    dynamicObj.color = p.Color;
                    dynamicObj.smv = p.SMV;
                    dynamicObj.part = p.Part;
                    dynamicObj.operators = p.Operators;
                    dynamicObj.helpers = p.Helpers;
                    dynamicObj.shiftHours = p.ShiftHours;
                    dynamicObj.shiftName = p.ShiftName;
                    dynamicObj.sizeList = p.SizeList;
                    dynamicObj.userid = p.UserID;
                    dynamicObj.plannedEffeciency = p.PlannedEffeciency;
                    dynamicObj.plannedTarget = p.PlannedTarget;
                    dynamicObj.entryDate = p.EntryDate;
                    dynamicObj.date = p.Date;
                    dynamicObj.isorderrun = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == p.PONo && x.SONo == p.SONo && x.Color== p.Color && x.QCDate.Date == p.Date.Date).Select(x => x.QCMasterId).FirstOrDefault();


                    LineTargetList.Add(dynamicObj);
                }
                     
                //if (_obj.IsTargetStartDate == 1 && _obj.IsTargetEndDate == 1)
                //    query = query.Where(x => x.Date >= Convert.ToDateTime(_obj.TargetStartDate) && x.Date <= Convert.ToDateTime(_obj.TargetEndDate));
                //else if (_obj.IsTargetStartDate == 1)
                //    query = query.Where(x => x.Date >= Convert.ToDateTime(_obj.TargetStartDate));
                return Ok(new { status = 200, message = "Success", data=LineTargetList });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [Route("PostIsPOPlanExist")]
        [HttpPost]
        public ActionResult PostIsPOPlanExist([FromBody] tbl_LineTarget _obj)
        {
            try
            {
                var data = _context.tbl_LineTarget.Where(x => x.PONo == _obj.PONo).FirstOrDefault();

                //var data = _context.tbl_LineTarget.Where(x => x.PONo == _obj.PONo && x.Date== Convert.ToDateTime( _obj.SONo) && x.ShiftName == _obj.ShiftName).FirstOrDefault();
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


        //Mobile App API
        [HttpPost]
        [Route("POShiftTarget")]
        public ActionResult POShiftTarget([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_LineTarget> query = _context.tbl_LineTarget.Where(x => x.Date == DateTime.Now.Date && x.FactoryID ==_UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ShiftName))
                    query = query.Where(x => x.ShiftName == _obj.ShiftName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    query = query.Where(x => x.Line == _obj.Line);

                var data = query.ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetPOforLineTarget")]
        public ActionResult GetPOforLineTarget([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.OrderStatus != 2);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                query = query.Where(x => x.EntryDate >= _obj.StartDate && x.EntryDate <= _obj.EndDate);
                var data = query.Select(x => new { PONo = x.PONo, SONo = x.SONo, PlanStDt = x.PlanStDt,Style = x.Style,ProcessName=x.ProcessName,ProcessCode=x.ProcessCode }).Distinct().OrderBy(x => x.PlanStDt).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetLineTargetExcelData")]
        public ActionResult GetLineTargetExcelData([FromBody] List<SearchModel> _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);



                int daysCount = 0;
                double totalUnits = 0;
                double sizewiseQty = 0;
                List<dynamic> orderlst = new List<dynamic>();

                int ExportAutoFill = _obj[0].ExportAutoFill;


                foreach (var t in _obj)
                {
                    var orderdetail = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == t.PONo && x.SONo == t.SONo).ToList();
                    daysCount = (orderdetail[0].PlanStDt - orderdetail[0].ExFactory).Days;
                    if (daysCount < 0)
                        daysCount = -daysCount;
                    foreach (var row in orderdetail)
                    {



                        var dt = row.PlanStDt;
                        do
                        {
                            dynamic dynamicObj = new ExpandoObject();
                            dynamicObj.Module = row.Module;
                            dynamicObj.Process = row.ProcessCode;
                            dynamicObj.Section = "";
                            dynamicObj.Line = "";
                            dynamicObj.Style = row.Style;
                            dynamicObj.SO = row.SONo;
                            dynamicObj.PO = row.PONo;
                            dynamicObj.Part = row.Part;
                            dynamicObj.Color = row.Color;
                            dynamicObj.SMV = "";
                            dynamicObj.Operators = "";
                            dynamicObj.Helpers = "";
                            dynamicObj.ShiftName = "";
                            dynamicObj.ShiftHours = "";
                            dynamicObj.Date = dt.ToString("dd-MM-yyyy");
                            dynamicObj.PlannedEfficiency = "";
                            dynamicObj.PlannedTarget = "";
                            string[] sa = row.SizeList.Split(',');



                            int index = 0;
                            if (row.IsSizeRun == 0)
                            {



                                var sizename = sa[0].Split('-').First();
                                var sizeqty = Convert.ToInt16(sa[0].Split('-').Last());
                                if (sizeqty > 0)
                                {
                                    AddProperty(dynamicObj, "Size1", sizename);
                                    AddProperty(dynamicObj, "Size1" + "Target", "");
                                }
                            }
                            else
                            {
                                foreach (string s in sa)
                                {

                                    //will check, need to validate total Units check  --Lalit
                                    //totalUnits = 0;
                                    var sizename = s.Split('-').First();
                                    var sizeqty = Convert.ToInt16(s.Split('-').Last());



                                    if (sizeqty > 0)
                                    {
                                        index++;
                                        AddProperty(dynamicObj, "Size" + index.ToString(), sizename);
                                        if (ExportAutoFill == 1)
                                        {
                                            if (daysCount > 0)
                                                sizewiseQty = Math.Ceiling((double)sizeqty / daysCount);
                                            else
                                                sizewiseQty = sizeqty;
                                            //totalUnits = totalUnits + sizewiseQty;
                                            //if (totalUnits > sizeqty)
                                            //    sizewiseQty = sizewiseQty - (totalUnits - sizeqty);
                                            AddProperty(dynamicObj, "Size" + index.ToString() + "Target", Convert.ToString(sizewiseQty));

                                        }
                                        else
                                        {
                                            AddProperty(dynamicObj, "Size" + index.ToString() + "Target", "");
                                        }
                                    }



                                }
                            }



                            orderlst.Add(dynamicObj);
                            dt = dt.AddDays(1);
                        }
                        while (dt <= row.ExFactory);
                    }
                }



                return Ok(new { status = 200, message = "Success", orderlst });
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        [HttpPost]
        [Route("PutEditedLineTarget")]
        public ActionResult PutEditedLineTarget([FromBody] List<tbl_LineTarget> _obj)
        {
            try
            {
                foreach (var l in _obj)
                {
                    var Linetarget = _context.tbl_LineTarget.Where(x => x.LineTargetID == l.LineTargetID && x.FactoryID == l.FactoryID).ToList();
                    if (Linetarget != null)
                    {
                        _context.tbl_LineTarget.RemoveRange(Linetarget);
                        tbl_LineTarget otbl_LineTarget = new tbl_LineTarget();

                        otbl_LineTarget.LineTargetID = l.LineTargetID;
                        otbl_LineTarget.Module = l.Module;
                        otbl_LineTarget.Section = l.Section;
                        otbl_LineTarget.Line = l.Line;
                        otbl_LineTarget.Style = l.Style;
                        otbl_LineTarget.SONo = l.SONo;
                        otbl_LineTarget.PONo = l.PONo;
                        otbl_LineTarget.Part = l.Part;
                        otbl_LineTarget.Color = l.Color;
                        otbl_LineTarget.SMV = l.SMV;
                        otbl_LineTarget.Helpers = l.Helpers;
                        otbl_LineTarget.Operators = l.Operators;
                        otbl_LineTarget.ShiftHours = l.ShiftHours;
                        otbl_LineTarget.ShiftName = l.ShiftName;
                        otbl_LineTarget.Date = l.Date;
                        otbl_LineTarget.PlannedEffeciency = l.PlannedEffeciency;
                        otbl_LineTarget.PlannedTarget = l.PlannedTarget;
                        otbl_LineTarget.UserID = l.UserID;
                        otbl_LineTarget.EntryDate = l.EntryDate;
                        otbl_LineTarget.SizeList = l.SizeList;
                        otbl_LineTarget.FactoryID = l.FactoryID;
                        _context.tbl_LineTarget.Add(otbl_LineTarget);
                        _context.SaveChanges();

                    }
                    else
                    {
                        { return Ok(new { status = 400, message = "No Record Found" }); }
                    }

                }
                { return Ok(new { status = 200, message = "Success" }); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
    }

    