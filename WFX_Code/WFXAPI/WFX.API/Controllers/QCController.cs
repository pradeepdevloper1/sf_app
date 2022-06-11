using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WFX.API.WebService;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QCController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<QCController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        private readonly IUserWebService _userWebService;
        public QCController(IWebHostEnvironment env, ILogger<QCController> logger, DBContext context, IConfiguration configuration, IUserWebService userWebService)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
            _userWebService = userWebService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("QCController API Call");
        }

        [HttpPost]
        [Route("PostQC")]
        public ActionResult PostQC([FromBody] tbl_QCMaster _obj)
        {
            try
            {
                var SONo = _context.tbl_Orders.Where(x => x.FactoryID == _obj.FactoryID && x.PONo == _obj.PONo).Select(x => x.SONo).FirstOrDefault().ToString();

                long id = 0;
                var lastrecord = _context.tbl_QCMaster.OrderBy(x => x.QCMasterId).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.QCMasterId) + 1;
                _obj.QCMasterId = id;
                _obj.QCDate = System.DateTime.Now;
                _obj.SONo = SONo;

                long childid = 0;
                var lastchildrecord = _context.tbl_QCDefectDetails.OrderBy(x => x.QCDefectDetailsId).LastOrDefault();
                childid = (lastchildrecord == null ? 0 : lastchildrecord.QCDefectDetailsId) + 1;
                foreach (tbl_QCDefectDetails onerow in _obj.tbl_QCDefectDetails)
                {
                    onerow.QCMasterId = id;
                    onerow.QCDefectDetailsId = childid;
                    childid++;
                }
                _obj.BatchNumber = "";
                _context.tbl_QCMaster.Add(_obj);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success", id = _obj.QCMasterId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostDailyActivity")]
        public ActionResult PostDailyActivity([FromBody] tbl_DailActivity _obj)
        {
            try
            {
                long id = 0;
                var lastrecord = _context.tbl_DailActivity.Where(x => x.ActivitDate == DateTime.Now.Date && x.UserID == _obj.UserID && x.IsActive == 1).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.LastUpdatedDateTime = _obj.LastUpdatedDateTime;
                    lastrecord.Seconds = _obj.Seconds;
                    lastrecord.IsActive = _obj.IsActive;
                    lastrecord.IsPause = _obj.IsPause;
                    lastrecord.LastLogOut = _obj.LastLogOut;
                    lastrecord.Manhrs = _obj.Manhrs;
                    _context.tbl_DailActivity.Update(lastrecord);
                    _context.SaveChanges();
                }
                else
                {
                    lastrecord = _context.tbl_DailActivity.OrderBy(x => x.DailActivityID).LastOrDefault();
                    id = (lastrecord == null ? 0 : lastrecord.DailActivityID) + 1;
                    _obj.DailActivityID = id;
                    _context.tbl_DailActivity.Add(_obj);
                    _context.SaveChanges();

                }
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Performance")]
        public ActionResult Performance([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                QCPerformance data = new QCPerformance();

                IQueryable<tbl_Orders> orderquery = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    orderquery = orderquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Fit))
                    orderquery = orderquery.Where(x => x.Fit == _obj.Fit);
                if (!string.IsNullOrEmpty(_obj.Customer))
                    orderquery = orderquery.Where(x => x.Customer == _obj.Customer);
                if (!string.IsNullOrEmpty(_obj.Product))
                    orderquery = orderquery.Where(x => x.Product == _obj.Product);
                if (!string.IsNullOrEmpty(_obj.Season))
                    orderquery = orderquery.Where(x => x.Season == _obj.Season);
                if (!string.IsNullOrEmpty(_obj.Style))
                    orderquery = orderquery.Where(x => x.Style == _obj.Style);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    orderquery = orderquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    orderquery = orderquery.Where(x => x.ProcessCode == _obj.ProcessCode);
                orderquery = orderquery.Where(x => x.EntryDate.Date >= _obj.StartDate.Date && x.EntryDate.Date <= _obj.EndDate.Date);

                IQueryable<tbl_LineTarget> plannedquery = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    plannedquery = plannedquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Line))
                    plannedquery = plannedquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Module))
                    plannedquery = plannedquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.Style))
                    plannedquery = plannedquery.Where(x => x.Style == _obj.Style);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    plannedquery = plannedquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    plannedquery = plannedquery.Where(x => x.ProcessCode == _obj.ProcessCode);
                plannedquery = plannedquery.Where(x => x.Date.Date >= _obj.StartDate.Date && x.Date.Date <= _obj.EndDate.Date);

                IQueryable<vw_QC> viewqc = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    viewqc = viewqc.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Line))
                    viewqc = viewqc.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Module))
                    viewqc = viewqc.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    viewqc = viewqc.Where(x => x.ProcessCode == _obj.ProcessCode);

                viewqc = viewqc.Where(x => x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date);

                IQueryable<tbl_QCMaster> qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcmaster = qcmaster.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcmaster = qcmaster.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcmaster = qcmaster.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    qcmaster = qcmaster.Where(x => x.ProcessCode == _obj.ProcessCode);
                qcmaster = qcmaster.Where(x => x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date);

                List<Planned> planedlist = new List<Planned>();
                List<Actual> actuallist = new List<Actual>();
                List<DHU> dhulist = new List<DHU>();
                List<Rejection> rejlist = new List<Rejection>();
                //Completion in Performance 
                data.OrderQty = orderquery.Sum(x => x.POQty);
                if (plannedquery.ToList().Count > 0)
                {
                    data.PlannedQty = plannedquery.Sum(x => x.PlannedTarget);
                    if (data.OrderQty > 0)
                    {
                        data.PlannedPer = Convert.ToDouble((data.PlannedQty * 100) / data.OrderQty);
                    }
                    data.PlannedSAH = Math.Ceiling(plannedquery.Average(x => x.PlannedTarget * x.SMV / 60));

                    data.EfficiencyPlanedPer = Math.Ceiling(plannedquery.Average(x => x.PlannedEffeciency));
                }
                else
                {
                    data.PlannedQty = 0;
                    data.PlannedPer = 0;
                    data.PlannedSAH = 0;

                    data.EfficiencyPlanedPer = 0;
                }
                if (viewqc.ToList().Count > 0)
                {
                    data.CompletedQty = viewqc.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                    if (data.OrderQty > 0)
                    {
                        data.CompletedPer = Convert.ToDouble(data.CompletedQty * 100 / data.OrderQty);
                    }
                    if (data.CompletedQty > 0 && plannedquery.ToList().Count > 0) { data.ActualSAH = Math.Ceiling(data.CompletedQty * plannedquery.FirstOrDefault().SMV / 60); }
                    else { data.ActualSAH = 0; }

                    data.TotalInspected = qcmaster.Where(x => x.TypeOfWork == 1).Sum(x => x.Qty);
                    data.CurrInspected = qcmaster.Where(x => x.QCDate.Date == DateTime.Now.Date && x.TypeOfWork == 1).Sum(x => x.Qty);

                    data.TotalDefects = viewqc.Where(x => x.QCDefectDetailsId > 0).ToList().Count;
                    data.CurrDefects = viewqc.Where(x => x.QCDate.Date == DateTime.Now.Date && x.QCDefectDetailsId > 0).ToList().Count;

                    data.TotalRejected = qcmaster.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                    data.CurrRejected = qcmaster.Where(x => x.QCStatus == 3 && x.QCDate.Date == DateTime.Now.Date).Sum(x => x.Qty);

                    // Eff           
                    var targetlist = plannedquery.ToList();
                    double total_eff = 0;
                    int daycount = 0;
                    int total_bal = 0;
                    double total_eff_diffrence = 0;

                    int day_plan_eff = 0;
                    double day_target_100per = 0;
                    double todayPlannedEfficiency = 0;
                    int day_goodpcs = 0;
                    double day_eff = 0;
                    double day_actual_eff = 0;
                    int todayPassPcs = 0;
                    int pendingTarget = 0;
                    int todayPlannedTarget = 0;
                   int totalPlannedTarget = 0;
                    double PlannedEfficiency = 0;
                    int PassPcs = 0;
                    int day_target = 0;
                    double daytarget100per = 0;
                     foreach (tbl_LineTarget oneday in targetlist)
                    {
                        day_target = oneday.PlannedTarget;
                        day_plan_eff = Convert.ToInt16(oneday.PlannedEffeciency);
                        if (day_plan_eff > 0)
                            day_target_100per = (double)day_target * 100 / (double)day_plan_eff;
                        day_goodpcs = qcmaster.Where(x => x.QCStatus == 1 && x.QCDate.Date == oneday.Date.Date && x.Color == oneday.Color).Sum(x => x.Qty);
                        total_bal += day_target - day_goodpcs;
                       
                        day_eff = (double)day_goodpcs * (double)day_plan_eff / (double)day_target;
                        day_actual_eff = (double)day_goodpcs * 100 / day_target_100per;
                        total_eff_diffrence += day_plan_eff - day_actual_eff;

                        total_eff += day_eff;
                        daycount++;
                    }
                    if (daycount > 0)
                        data.EfficiencyActualPer = Math.Ceiling((double)total_eff /(double)daycount);
                    if (day_target_100per > 0)
                        data.EfficiencyRequiredPer = Math.Ceiling(total_bal * 100 / day_target_100per);
                    if (data.EfficiencyRequiredPer < 0)
                        data.EfficiencyRequiredPer = 0;
                    //Eff end here

                 

                    //   data.DHUMax = viewqc.Where(x => x.QCDefectDetailsId > 0).GroupBy(x => new { QCDate = x.QCDate.Date })
                    //                    .Select(g => new
                    //                    {
                    //                        DHUMax =g.Count() * 100 ,
                    //                    }).Max(x => x.DHUMax);
                    //data.DHUMax = (long) Math.Ceiling((double)data.DHUMax / qcmaster.Where(x => x.TypeOfWork == 1).Count());
                    
                    var isrejexist = qcmaster.Where(x => x.QCStatus == 3).Count();
                    if (isrejexist > 0)
                    {
                        data.RejMax = qcmaster.Where(x => x.QCStatus == 3).GroupBy(x => new { QCDate = x.QCDate.Date })
                                       .Select(g => new
                                       {
                                           RejMax = g.Where(x => x.QCStatus == 3).Sum(p => p.Qty),
                                       }).Max(x => x.RejMax);


                        data.RejMaxPer = qcmaster.GroupBy(x => new { QCDate = x.QCDate.Date })
                                        .Select(g => new
                                        {
                                            RejectPer = g.Where(x => x.QCStatus == 3).Sum(p => p.Qty) * 100 / qcmaster.Where(x => x.TypeOfWork == 1).Sum( x=> x.Qty),
                                        }).Max(x => x.RejectPer);
                        
                    }
                    else
                    {
                        data.RejMax = 0;
                        data.RejMaxPer = 0;
                    }
                    #region Plan vs Actual
                    //Production Variance Plan vs Actual
                    var planed = plannedquery.GroupBy(x => new { PlanDate = x.Date.Date, PlanDay = x.Date.Day, PlanMonth = x.Date.Month })
                                        .Select(g => new
                                        {
                                            PlanQty = g.Sum(p => p.PlannedTarget),
                                            PlanDate = g.Key.PlanDate,
                                            PlanDay = g.Key.PlanDay,
                                            PlanMonth = g.Key.PlanMonth,
                                        }).ToList();
                    foreach (var v in planed)
                    {
                        Planned _oPlanned = new Planned();
                        _oPlanned.PlanQty = v.PlanQty;
                        _oPlanned.PlanDate = v.PlanDate;
                        _oPlanned.PlanDay = v.PlanDay;
                        _oPlanned.PlanMonth = v.PlanMonth;
                        planedlist.Add(_oPlanned);
                    }

                    var actual = qcmaster.Where(x => x.QCStatus == 1).GroupBy(x => new { QCDate = x.QCDate.Date, QCDay = x.QCDate.Day, QCMonth = x.QCDate.Month })
                    .Select(g => new
                    {
                        QCQty = g.Sum(p => p.Qty),
                        QCDate = g.Key.QCDate,
                        QCDay = g.Key.QCDay,
                        QCMonth = g.Key.QCMonth,
                    }).ToList(); 
                    foreach (var v in actual)
                    {
                        Actual _oActual = new Actual();
                        _oActual.QCQty = v.QCQty;
                        _oActual.QCDate = v.QCDate;
                        _oActual.QCDay = v.QCDay;
                        _oActual.QCMonth = v.QCMonth;
                        actuallist.Add(_oActual);
                        var planed1 = planed.Where(x => x.PlanDate == v.QCDate && x.PlanDay == v.QCDay && x.PlanMonth == v.QCMonth);

                        if (planed1.Count() == 0) {
                            Planned _oPlanned = new Planned();
                            _oPlanned.PlanQty = 0;
                            _oPlanned.PlanDate = v.QCDate;
                            _oPlanned.PlanDay = v.QCDay;
                            _oPlanned.PlanMonth = v.QCMonth;
                            planedlist.Add(_oPlanned);
                        }

                    }

                    //Production Variance Plan vs Actual end here 
                    #endregion

                    #region DHU Variance
                    // DHU Variance Graph
                    var dhu = viewqc.GroupBy(x => new { QCDate = x.QCDate.Date, QCDay = x.QCDate.Day, QCMonth = x.QCDate.Month})
                    .Select(g => new
                    {
                        
                    DayCheckedPCS = g.Sum(p => p.Qty),
                    DayDefectsCount = g.Where(x => x.QCDefectDetailsId > 0).Count(),
                        DayDefects = g.Where(x => x.QCDefectDetailsId > 0).Count(),
                        DayPass = g.Sum(p => p.Qty),
                        QCDate = g.Key.QCDate,
                        QCDay = g.Key.QCDay,
                        QCMonth = g.Key.QCMonth
                    }).ToList();

                    foreach (var v in dhu)
                    {
                        DHU _oDHU = new DHU();
                         _oDHU.DayCheckedPCS = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.TypeOfWork==1).Sum(x=>x.Qty);
                
                        _oDHU.DayDefectsCount = v.DayDefectsCount;
                        if (v.DayPass > 0)
                        {
                            //double DayDefectsPer = ((double)v.DayDefects * 100) / (double)v.DayCheckedPCS;
                            _oDHU.DayDefectsPer = v.DayDefects * 100/ _oDHU.DayCheckedPCS;
                        }
                        else
                            _oDHU.DayDefectsPer = v.DayDefects;
                        _oDHU.QCDate = v.QCDate;
                        _oDHU.QCDay = v.QCDay;
                        _oDHU.QCMonth = v.QCMonth;
                        dhulist.Add(_oDHU);
                    }

                    data.DHUMax = dhulist.Select(x => x.DayDefectsPer).Max();
                    var rej = qcmaster.GroupBy(x => new { QCDate = x.QCDate.Date, QCDay = x.QCDate.Day, QCMonth = x.QCDate.Month })
                        .Select(g => new
                        {
                            DayCheckedPCS = qcmaster.Where(x => x.TypeOfWork == 1).Sum(x => x.Qty),
                            RejectCount = g.Where(x => x.QCStatus == 3).Sum(p => p.Qty),
                            Reject = g.Where(x => x.QCStatus == 3).Sum(p => p.Qty),
                            passQty = qcmaster.Where(x => x.TypeOfWork == 1).Count(),
                            QCDate = g.Key.QCDate,
                            QCDay = g.Key.QCDay,
                            QCMonth = g.Key.QCMonth
                        }).ToList();

                    foreach (var v in rej)
                    {
                        Rejection _oRejection = new Rejection();
                        _oRejection.DayCheckedPCS = v.DayCheckedPCS;
                        _oRejection.RejectCount = v.RejectCount;
                        if (v.passQty > 0)
                        {
                            //double RejectPer = ((double)v.Reject * 100 )/ (double)v.DayCheckedPCS;
                            _oRejection.RejectPer = v.Reject * 100 /v.DayCheckedPCS;
                        }
                        else
                            _oRejection.RejectPer = v.Reject;
                        _oRejection.QCDate = v.QCDate;
                        _oRejection.QCDay = v.QCDay;
                        _oRejection.QCMonth = v.QCMonth;
                        rejlist.Add(_oRejection);
                    }
                    
                    // DHU Variance Graph end here 
                    #endregion
                }
                else
                {
                    data.CompletedQty = 0;
                    data.CompletedPer = 0;
                    data.ActualSAH = 0;
                    data.TotalInspected = 0;
                    data.CurrInspected = 0;

                    data.TotalDefects = 0;
                    data.CurrDefects = 0;

                    data.TotalRejected = 0;
                    data.CurrRejected = 0;

                    data.EfficiencyActualPer = 0;
                    data.EfficiencyRequiredPer = 0;

                    data.DHUMax = 0;
                    data.RejMax = 0;
                    data.RejMaxPer = 0;
                }

                //DHU  in Performance
                if (data.CurrDefects > 0 && data.CurrInspected > 0)
                    data.DHUCurrent = data.CurrDefects * 100 / data.CurrInspected;
                else
                    data.DHUCurrent = 0;

                if (data.TotalInspected > 0)
                    data.DHUAvg = data.TotalDefects * 100 / data.TotalInspected;
                else
                    data.DHUAvg = 0;
                //DHU  in Performance end here

                //Reject Rate in Performance
                if (data.CurrInspected > 0)
                    data.RejCurrent = (data.CurrRejected * 100) / data.CurrInspected;
                else
                    data.RejCurrent = 0;

                if (data.TotalInspected > 0)
                    data.RejAvg = data.TotalRejected * 100/ data.TotalInspected;
                else
                    data.RejAvg = 0;
                //Reject Rate in Performance end here

                #region POBasedGraph & Photo
                List<POOperation> operationlist = new List<POOperation>();
                List<POOperation> operationlistforchart = new List<POOperation>();
                List<PODefect> defectlist = new List<PODefect>();
                List<POImage> imagelist = new List<POImage>();
                List<POImage> shiftimagelist = new List<POImage>();

                POOperation _operation = new POOperation();
                if (!string.IsNullOrEmpty(_obj.PONo))
                {
                    IQueryable<vw_QC> opquery = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo);
                    if (opquery.Where(x => x.QCDefectDetailsId > 0).ToList().Count > 0)
                    {
                        var oplist = opquery.Where(x => x.QCDefectDetailsId > 0).GroupBy(x => new { OperationName = x.OperationName })
                                            .Select(g => new
                                            {
                                                DefectCount = g.Count(),
                                                OperationName = g.Key.OperationName,
                                            }).ToList().OrderByDescending(x => x.DefectCount).Take(5);
                        var totaldefect = opquery.Where(x => x.QCDefectDetailsId > 0).Count();


                        _operation = new POOperation();
                        _operation.OperationName = "All Operations";
                        _operation.DefectCount = totaldefect;
                        operationlist.Add(_operation);

                        foreach (var v in oplist)
                        {
                            _operation = new POOperation();
                            _operation.OperationName = v.OperationName;
                            _operation.DefectCount = v.DefectCount;
                            operationlist.Add(_operation);
                        }

                        //Operation List For Chart
                        var opforchart = opquery.Where(x => x.QCDefectDetailsId > 0).GroupBy(x => new { OperationName = x.OperationName })
                                          .Select(g => new
                                          {
                                              DefectCount = g.Count(),
                                              OperationName = g.Key.OperationName,
                                          }).ToList().OrderByDescending(x => x.DefectCount).Take(10).OrderBy(x => x.DefectCount);
                        foreach (var v in opforchart)
                        {
                            _operation = new POOperation();
                            _operation.OperationName = v.OperationName;
                            _operation.DefectCount = v.DefectCount;
                            operationlistforchart.Add(_operation);
                        }

                        //Operation Data               
                        var deflist = opquery.Where(x => x.QCDefectDetailsId > 0).GroupBy(x => new { DefectName = x.DefectName })
                                            .Select(g => new
                                            {
                                                OperationCount = g.Count(),
                                                DefectName = g.Key.DefectName,
                                            }).ToList().OrderByDescending(x => x.OperationCount).Take(5);

                        PODefect _defect = new PODefect();
                        _defect = new PODefect();
                        _defect.DefectName = "All Defects";
                        _defect.OperationCount = totaldefect;
                        defectlist.Add(_defect);

                        foreach (var v in deflist)
                        {
                            _defect = new PODefect();
                            _defect.DefectName = v.DefectName;
                            _defect.OperationCount = v.OperationCount;
                            defectlist.Add(_defect);
                        }
                    }

                    //PO Images
                    var imglist = _context.tbl_POImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                    POImage _poimage = new POImage();
                    foreach (var v in imglist)
                    {
                        _poimage = new POImage();
                        _poimage.ImagePath = v.ImagePath;
                        _poimage.ImageName = v.ImageName;
                        _poimage.PONo = v.PONo;
                        imagelist.Add(_poimage);
                    }

                    //PO shiftImages
                    var shiftimglist = _context.tbl_POShiftImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                    _poimage = new POImage();
                    foreach (var v in shiftimglist)
                    {
                        _poimage = new POImage();
                        _poimage.ImagePath = v.ImagePath;
                        _poimage.ImageName = v.ImageName;
                        _poimage.PONo = v.PONo;
                        shiftimagelist.Add(_poimage);
                    }
                }
                #endregion
                planedlist = planedlist.OrderBy(x => x.PlanDate).ToList();
                return Ok(new { status = 200, message = "Success", data, actuallist, planedlist, operationlist, operationlistforchart, defectlist, imagelist, shiftimagelist, dhulist, rejlist });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("POOperationDefect")]
        //Pono and operation name based api
        public ActionResult POOperationDefect([FromBody] SearchModel _obj)
        {
            try
            {
                var data = _context.vw_QC.Where(x => x.PONo == _obj.PONo)
                    .Where(x => x.OperationName == _obj.OperationName).
                    GroupBy(x => new { DefectName = x.DefectName })
                .Select(g => new
                {
                    DefectName = g.Key.DefectName,
                    DefectCount = g.Count(),
                }).ToList();

                List<POOPDef> opdefectlist = new List<POOPDef>();
                POOPDef _oPOOPDef = new POOPDef();
                foreach (var v in data)
                {
                    _oPOOPDef = new POOPDef();
                    _oPOOPDef.PONo = _obj.PONo;
                    _oPOOPDef.OperationName = _obj.OperationName;
                    _oPOOPDef.DefectName = v.DefectName;
                    _oPOOPDef.DefectCode = "-";
                    _oPOOPDef.DefectCount = v.DefectCount;
                    opdefectlist.Add(_oPOOPDef);
                }

                return Ok(new { status = 200, message = "Success", opdefectlist });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Mobile App API
        [HttpPost]
        [Route("UndoQC")]
        public ActionResult UndoQC(tbl_QCMaster _obj)
        {
            try
            {
                var data = _context.tbl_QCMaster.Where(x => x.QCMasterId == _obj.QCMasterId).FirstOrDefault();
                if (data == null)
                {
                    return Ok(new { status = 400, message = "No record found." });
                }
                var defectdetails = _context.tbl_QCDefectDetails.Where(x => x.QCMasterId == _obj.QCMasterId).ToList();
                var qcDefectImages = _context.tbl_QCDefectImages.Where(x => x.QCMasterId == _obj.QCMasterId).ToList();

                var tblPoshiftImages = _context.tbl_POShiftImages.Where(x => x.FactoryID == data.FactoryID && x.PONo == data.PONo);

                _context.tbl_QCDefectImages.RemoveRange(qcDefectImages);
                _context.tbl_QCDefectDetails.RemoveRange(defectdetails);
                _context.tbl_QCMaster.Remove(data);
                _context.SaveChanges();
                foreach (var rec in tblPoshiftImages)
                {
                    var qcDefectImagesUndo= _context.tbl_QCDefectImages.Where(x => x.FactoryID == rec.FactoryID && x.PONo==rec.PONo  && x.ImageName==rec.ImageName).OrderBy(x => x.QCDefectImagesID).LastOrDefault();
                    if (qcDefectImagesUndo != null)
                        rec.ImagePath = qcDefectImagesUndo.ImagePath;
                    else
                        rec.ImagePath = "";
                }
                _context.UpdateRange(tblPoshiftImages);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //ERP check Record Pending for GRN
        [HttpPost]
        [Route("QCStatus")]
        public ActionResult CheckQCPendingforGRN(tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_QCRequest.Where(x => x.FactoryID == _UserTokenInfo.FactoryID //&& x.Module == _obj.Module 
                                                    && x.PONo == _obj.PONo && x.StockGRNID == 0).FirstOrDefault();
                if (data == null)
                {
                    return Ok(new { status = 404, message = "No record found." });
                }
                return Ok(new { status = 200, message = "Some Receiving Pending in Base ERP", });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //Mobile App API
        [HttpPost]
        [Route("EndShift")]
        public  ActionResult EndShift(tbl_QCMaster tbl_QCMaster)
        {
            var status = 200;
            var message = "Success";
            List<tbl_QCRequest> data = new List<tbl_QCRequest>();
            _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

            try
            {           
                IQueryable<tbl_Orders> query = _context.tbl_Orders;
                var source = query.Where(x => x.FactoryID == tbl_QCMaster.FactoryID && x.PONo == tbl_QCMaster.PONo)
                    .Select(x=>x.Source).FirstOrDefault();
                static String GetTimestamp(DateTime value)
                {
                    return value.ToString("yyyyMMddHHmmssffff");
                }
                String timeStamp = GetTimestamp(System.DateTime.Now);
                if (source == "ERPApp")
                {
                    long QCRequestID = 0;
                    var lastorder = _context.tbl_QCRequest.OrderBy(x => x.QCRequestID).LastOrDefault();
                    QCRequestID = (lastorder == null ? 0 : lastorder.QCRequestID) + 1;
                    var QCMaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == tbl_QCMaster.PONo && x.QCRequestID == 0).ToList();
                    if (QCMaster.Count > 0)
                    {
                        tbl_QCRequest objRS = new tbl_QCRequest();
                        objRS.QCRequestID = QCRequestID;
                        objRS.TranNum = tbl_QCMaster.PONo + timeStamp;
                        objRS.PONo = tbl_QCMaster.PONo;
                        objRS.Quantity = QCMaster.Where(x => x.QCStatus == 1 || x.QCStatus == 3).Select(x => x.Qty).Sum();
                        objRS.SyncedAt = System.DateTime.Now;
                        objRS.GRNstatus = "Not Created";
                        objRS.StockGRNID = 0;
                        objRS.ErrorMessage = "";
                        objRS.FactoryID = _UserTokenInfo.FactoryID;
                        objRS.SONo = tbl_QCMaster.SONo;
                        _context.tbl_QCRequest.Add(objRS);
                        _context.SaveChanges();


                        QCMaster.ForEach(x => x.QCRequestID = QCRequestID);

                        _context.tbl_QCMaster.UpdateRange(QCMaster);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
            return Ok(new { status = status, message = message });

        }
        //Mobile App API
        [HttpPost]
        [Route("GetTotalQCQuantity")]
        public  ActionResult TotalQCQuantity(tbl_QCMaster tbl_QCMaster)
        {
            try
            {
                int qty = 0;
                int tempQty = 0;
                IQueryable<tbl_QCMaster> query = _context.tbl_QCMaster.Where(x => x.FactoryID == tbl_QCMaster.FactoryID && x.Module == tbl_QCMaster.Module
                            && x.PONo == tbl_QCMaster.PONo && x.Color == tbl_QCMaster.Color && x.Size == tbl_QCMaster.Size);

                if (tbl_QCMaster.QCStatus == 1)
                {
                    qty = query.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                }
                else if (tbl_QCMaster.QCStatus == 2) {
                    qty = query.Where(x => x.Line == tbl_QCMaster.Line && x.QCStatus ==2 && x.TypeOfWork ==1).Count();
                    tempQty = query.Where(x => x.Line == tbl_QCMaster.Line && x.TypeOfWork == 2 && x.QCStatus !=2).Sum(x => x.Qty);
                    qty = qty - tempQty;
                }

                return Ok(new { status = 200, message = "Success", qty = qty });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //Mobile Issue to Next Process Screen API
        [HttpPost]
        [Route("GetIssueToNextProcessData")]
        public ActionResult GetIssueToNextProcessData(tbl_QCMaster _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                string[] colors = _obj.Color.Split(',');
                IQueryable<tbl_Orders> orderQuery = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo && x.PONo == _obj.PONo && x.Part == _obj.Part);
                IQueryable<vw_QC> qcQuery = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONO == _obj.SONo && x.PONo == _obj.PONo && x.Part == _obj.Part);
                IQueryable<tbl_OrderIssue> orderIssueQuery= _context.tbl_OrderIssue.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo && x.PONo == _obj.PONo);
                List<IssueToNextProcessData> data = new List<IssueToNextProcessData>();
                foreach (string color in colors)
                {
                    var orderData = orderQuery.Where(x => x.Color == color).FirstOrDefault();
                    string[] sizelist = orderData.SizeList.Split(',');
                    foreach (string size in sizelist)
                    {
                        string sizeName = size.Split('-')[0];
                        int sizeQty = Convert.ToInt32(size.Split('-')[1]);
                        if (sizeQty > 0)
                        {
                            IssueToNextProcessData IssueToNextProcessData = new IssueToNextProcessData();
                            var isExists = data.Where(x => x.color ==  color).Count();
                            if(isExists == 0)
                            {
                                IssueToNextProcessData.color = color;
                                IssueToNextProcessData.ColorHexName = GetKnownColor(System.Drawing.ColorTranslator.FromHtml(orderData.Hexcode).ToArgb());
                            }
                            else
                            {
                                IssueToNextProcessData.color = "";
                                IssueToNextProcessData.ColorHexName = "transparent";
                            }

                            IssueToNextProcessData.size = sizeName;
                            IssueToNextProcessData.WFXColorCode = orderData.WFXColorCode;
                            IssueToNextProcessData.WFXColorName = orderData.WFXColorName;
                            IssueToNextProcessData.OrderedQty = Convert.ToInt32(sizeQty);
                            IssueToNextProcessData.CompletedQty = qcQuery.Where(x => x.Color == color && x.Size == sizeName && x.QCStatus == 1).Sum(x => x.Qty);
                            IssueToNextProcessData.IssuedQty = orderIssueQuery.Where(x => x.Color == color && x.Size == sizeName).Sum(x => x.Qty);
                            IssueToNextProcessData.ReceivedbyNextDept = IssueToNextProcessData.IssuedQty;
                            IssueToNextProcessData.RemainingQtytobeIssued = IssueToNextProcessData.CompletedQty - IssueToNextProcessData.IssuedQty;
                            IssueToNextProcessData.IssueColorQty = 0;                     
                            data.Add(IssueToNextProcessData);
                        }

                    }
                }

                if (data == null)
                {
                    return Ok(new { status = 404, message = "No record found." });
                }
                return Ok(new { status = 200, message = "", data});
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        public String GetKnownColor(int iARGBValue)
        {
            Color someColor;

            Array aListofKnownColors = Enum.GetValues(typeof(KnownColor));
            foreach (KnownColor eKnownColor in aListofKnownColors)
            {
                someColor = Color.FromKnownColor(eKnownColor);
                if (iARGBValue == someColor.ToArgb() && !someColor.IsSystemColor)
                {
                    return someColor.Name;
                }
            }
            return "";
        }

        //Mobile Issue to Next Process Screen API
        [HttpPost]
        [Route("PostOrderIssue")]
        public ActionResult PostOrderIssue(List<tbl_OrderIssue> _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                long id = 0;
                string source = "", PONo = "", SONo = "";
                var lastrecord = _context.tbl_OrderIssue.OrderBy(x => x.OrderIssueId).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.OrderIssueId) + 1;
                foreach (tbl_OrderIssue onerow in _obj) {
                    onerow.OrderIssueId = id;
                    onerow.UserID = _UserTokenInfo.UserID;
                    onerow.FactoryID = _UserTokenInfo.FactoryID;
                    onerow.BatchNumber = "";
                    onerow.WFXStockGRNID = 0;
                    onerow.QCRequestID = 0;
                    onerow.IssueDate = System.DateTime.Now;
                    _context.tbl_OrderIssue.Add(onerow);
                    _context.SaveChanges();
                    id++;
                }
                PONo = _obj[0].PONo;
                SONo = _obj[0].SONo;
                source = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == PONo).Select(x => x.Source).FirstOrDefault();
                if (source == "ERPApp")
                {
                    long QCRequestID = 0;
                    var lastorder = _context.tbl_QCRequest.OrderBy(x => x.QCRequestID).LastOrDefault();
                    QCRequestID = (lastorder == null ? 0 : lastorder.QCRequestID) + 1;
                    var orderIssue = _context.tbl_OrderIssue.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == PONo && x.QCRequestID == 0).ToList();
                    if (orderIssue.Count > 0)
                    {
                        tbl_QCRequest objRS = new tbl_QCRequest();
                        objRS.QCRequestID = QCRequestID;
                        objRS.TranNum = PONo + System.DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        objRS.PONo = PONo;
                        objRS.Quantity = orderIssue.Select(x => x.Qty).Sum();
                        objRS.SyncedAt = System.DateTime.Now;
                        objRS.GRNstatus = "Not Created";
                        objRS.StockGRNID = 0;
                        objRS.ErrorMessage = "";
                        objRS.FactoryID = _UserTokenInfo.FactoryID;
                        objRS.SONo = SONo;
                        objRS.RequestType = "IssueToNextProcess";
                        _context.tbl_QCRequest.Add(objRS);
                        _context.SaveChanges();


                        orderIssue.ForEach(x => x.QCRequestID = QCRequestID);

                        _context.tbl_OrderIssue.UpdateRange(orderIssue);
                        _context.SaveChanges();
                    }
                }
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Mobile App API
        [HttpPost]
        [Route("GetIssueToWOData")]
        public ActionResult GetIssueToWOData(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var processType = _context.tbl_ProcessDefinition.Where(x => x.FactoryID == _UserTokenInfo.FactoryID
                                      && x.ProcessCode == _obj.ProcessCode).Select(x => x.ProcessType).FirstOrDefault();

                var processCodeList = _context.tbl_ProcessDefinition.Where(x => x.FactoryID == _UserTokenInfo.FactoryID
                                       && x.ProcessType == processType).Select(x => new { x.FactoryID, x.ProcessCode }).ToArray();
                IEnumerable<vw_POList> query =
                      from p in processCodeList
                      join s in _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo)
                        on new { p.FactoryID, p.ProcessCode } equals new
                        {
                            s.FactoryID,
                            s.ProcessCode
                        }
                      join q in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo)
                      on new { p.FactoryID, s.PONo } equals new
                      {
                          q.FactoryID,
                          q.PONo
                      }
                      group p by new
                      {
                          PONo = s.PONo,
                          Line = q.Line
                      } into g
                      select new vw_POList
                      {
                          PONo = g.Key.PONo,
                          Line = g.Key.Line
                      };
                var data = query.Select(x => new { PONo = x.PONo, Line = x.Line }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });
                return Ok(new { status = 200, message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        public class QCPerformance
        {
            public long OrderQty { get; set; }
            public long PlannedQty { get; set; }
            public double PlannedPer { get; set; }
            public long CompletedQty { get; set; }
            public double CompletedPer { get; set; }

            public double EfficiencyPlanedPer { get; set; }
            public double EfficiencyActualPer { get; set; }
            public double EfficiencyRequiredPer { get; set; }

            public double PlannedSAM { get; set; }
            public double ActualSAM { get; set; }
            public double ActualSAMOfOneGarment { get; set; }
            public double PlannedManHrs { get; set; }
            public double ActualManHrs { get; set; }

            public long DHUCurrent { get; set; }
            public long TotalDefects { get; set; }
            public long CurrDefects { get; set; }
            public long DHUAvg { get; set; }
            public long DHUMax { get; set; }
            public long RejCurrent { get; set; }
            public long TotalRejected { get; set; }
            public long TotalInspected { get; set; }

            public long CurrRejected { get; set; }
            public long CurrInspected { get; set; }

            public long RejAvg { get; set; }
            public long RejMax { get; set; }
            public long RejMaxPer { get; set; }

            public double PlannedSAH { get; set; }
            public double ActualSAH { get; set; }           
        }
        public class Planned
        {
            public int PlanQty { get; set; }
            public DateTime PlanDate { get; set; }
            public int PlanDay { get; set; }
            public int PlanMonth { get; set; }
        }
        public class Actual
        {
            public int QCQty { get; set; }
            public DateTime QCDate { get; set; }
            public int QCDay { get; set; }
            public int QCMonth { get; set; }
        }
        public class POOperation
        {
            public int DefectCount { get; set; }
            public string OperationName { get; set; }
            public string OperationCode { get; set; }
        }
        public class PODefect
        {
            public int OperationCount { get; set; }
            public string DefectName { get; set; }
            public string DefectCode { get; set; }
        }
        public class POOPDef
        {
            public string PONo { get; set; }
            public string OperationName { get; set; }
            public string DefectName { get; set; }
            public string DefectCode { get; set; }
            public int DefectCount { get; set; }
        }

        public class DHU
        {
            public int DayCheckedPCS { get; set; }
            public int DayDefectsCount { get; set; }
            public int DayDefectsPer { get; set; }
            public DateTime QCDate { get; set; }
            public int QCDay { get; set; }
            public int QCMonth { get; set; }
        }

        public class Rejection
        {
            public int DayCheckedPCS { get; set; }
            public int RejectCount { get; set; }
            public int RejectPer { get; set; }
            public DateTime QCDate { get; set; }
            public int QCDay { get; set; }
            public int QCMonth { get; set; }
        }
        public class IssueToNextProcessData {
            public string color { get; set; }
            public string size { get; set; }
            public int OrderedQty { get; set; }
            public int CompletedQty { get; set; }
            public int IssuedQty { get; set; }
            public int ReceivedbyNextDept { get; set; }
            public int RemainingQtytobeIssued { get; set; }
            public int IssueColorQty { get; set; }
            public string WFXColorCode { get; set; }
            public string WFXColorName { get; set; }
            public string ColorHexName { get; set; }
        }
    }
}
