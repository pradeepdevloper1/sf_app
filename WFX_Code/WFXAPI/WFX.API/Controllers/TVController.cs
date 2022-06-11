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
    public class TVController : ControllerBase
    {

        DBContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public TVController(IWebHostEnvironment env, ILogger<OrderController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;

        }

        //HOURLY PRODUCTION SCREEN-1
        [HttpPost]
        [Route("HourlyProductionBackup")]
        public ActionResult HourlyProductionBackup([FromBody] SearchModel _obj)
        {
            HourlyProductionModel data = new HourlyProductionModel();
            List<POImage> imagelist = new List<POImage>();
            var status = 200;
            var message = "Success";
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                //_obj.Line = "L1";
                //_obj.Module = "FL1";
                //_obj.ShiftName = "S1";
                var shiftdetails = _context.tbl_Shift.Where(x => x.ShiftName == _obj.ShiftName && x.FactoryID == _UserTokenInfo.FactoryID).FirstOrDefault();
                //double starttime = Convert.ToDouble(shiftdetails.ShiftStartTime);
                //double endtime = Convert.ToDouble(shiftdetails.ShiftEndTime);
                 
                int starttime = Convert.ToInt16(shiftdetails.ShiftStartTime);
                int endtime = Convert.ToInt16(shiftdetails.ShiftEndTime);


                DateTime ShiftEndDateTime = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, endtime, 0, 0);
                DateTime CurrDateTime = DateTime.Now;
                TimeSpan t = new TimeSpan();
                t = ShiftEndDateTime - DateTime.Now;
                if (CurrDateTime < ShiftEndDateTime)
                    data.TimeLeft = t.Hours.ToString() + " hr : " + t.Minutes.ToString() + " min ";
                else
                    data.TimeLeft = "0 hr : 0 min ";

                IQueryable<tbl_LineTarget> planquery = _context.tbl_LineTarget.Where(x => x.Line == _obj.Line && x.Module == _obj.Module && x.ShiftName == _obj.ShiftName && x.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID);
                var planresult = planquery.FirstOrDefault();
                if (planresult == null)
                {
                    status = 204;
                    message = "No record found.";
                }

                var pono = planresult.PONo;
                data.EffTarget = planresult.PlannedEffeciency;
                var target100 = planresult.PlannedTarget * 100 / planresult.PlannedEffeciency;

                IQueryable<tbl_Orders> orderquery = _context.tbl_Orders.Where(x => x.PONo == pono && x.PrimaryPart == 1 && x.FactoryID==_UserTokenInfo.FactoryID);
                var orderresult = orderquery.FirstOrDefault();
                //Header    
                data.Product = orderresult.Product;
                data.Style = orderresult.Style;
                data.Fit = orderresult.Fit;
                data.Season = orderresult.Season;
                data.Customer = orderresult.Customer;
                data.Module = orderresult.Module;
                data.Line = planresult.Line;
                data.SONo = orderresult.SONo;
                data.PONo = orderresult.PONo;
                data.FactoryID = orderresult.FactoryID;
                data.ExFactoryDate = orderresult.ExFactory;
                data.RecvDate = orderresult.EntryDate;
                //Header end                  

                IQueryable<tbl_QCMaster> qcquery = _context.tbl_QCMaster.Where(x => x.PONo == pono && x.ShiftName == _obj.ShiftName && x.QCDate.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID);
                var qcresult = qcquery.ToList();
                //bottom   
                data.Target = planresult.PlannedTarget;

                data.OutputGoodPCS = qcquery.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                data.ReWorkDefectPCS = qcquery.Where(x => x.QCStatus == 2).Sum(x => x.Qty);
                data.RejectPCS = qcquery.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                data.PicesChecked = qcquery.Sum(x => x.Qty);

                data.Variation = data.OutputGoodPCS - data.Target;

                data.EffAchieve = Math.Ceiling( data.OutputGoodPCS * 100 / target100);

                if (data.PicesChecked > 0)
                {
                    var QCMasterIds = qcquery.Select(x => x.QCMasterId).ToList();
                    var TotalDefects = _context.tbl_QCDefectDetails.Where(x => QCMasterIds.Contains(x.QCMasterId)).Count();
                    data.DefectRate = (TotalDefects * 100) / data.PicesChecked;

                    data.RejectRate = (data.RejectPCS * 100) / data.PicesChecked;
                }
                else
                {
                    data.DefectRate = 0;
                    data.RejectRate = 0;
                }
                //bottom  end 

                //list
                var qcids = qcquery.Select(x => x.QCMasterId).ToList();

                List<HourlyProductionList> _HourlyProductionList = new List<HourlyProductionList>();
                data.Hourlytarget = Convert.ToInt16(planresult.PlannedTarget / planresult.ShiftHours);
                var totaloutput = 0;
                var hrscount = 0;
                while (starttime < endtime)
                {
                    DateTime strdate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, starttime, 0, 0);
                    DateTime enddate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, starttime + 1, 0, 0);

                    HourlyProductionList lst = new HourlyProductionList();
                    double starttime_to = starttime + 1;
                    lst.Hour = starttime.ToString() + "-" + starttime_to.ToString();
                    lst.Target = data.Hourlytarget;
                    lst.Output = qcquery.Where(x => x.QCStatus == 1 && x.QCDate >= strdate && x.QCDate <= enddate).Sum(x => x.Qty);
                    totaloutput += lst.Output;
                    hrscount++;
                    lst.Variation = lst.Output - lst.Target;
                    var PicesChecked = qcquery.Where(x => x.QCDate >= strdate && x.QCDate <= enddate).Sum(x => x.Qty);
                    if (PicesChecked > 0)
                    {
                        var hrsqcid = qcquery.Where(x => x.QCStatus > 1 && x.QCDate >= strdate && x.QCDate <= enddate && x.FactoryID == _UserTokenInfo.FactoryID).Select(x => x.QCMasterId).ToList();
                        var hrstotaldefects = _context.tbl_QCDefectDetails.Where(x => hrsqcid.Contains(x.QCMasterId)).Count();
                        lst.DefectRate = hrstotaldefects * 100 / PicesChecked;
                    }
                    else
                    {
                        lst.DefectRate = 0;
                    }
                    _HourlyProductionList.Add(lst);
                    starttime += 1;
                }
                data.HourlyProductionList = _HourlyProductionList;              
                data.HourlyAchieve = totaloutput / hrscount;

                //IQueryable<tbl_QCMaster> qcquery = _context.tbl_QCMaster.Where(x => x.PONo == pono && x.ShiftName == _obj.ShiftName && x.QCDate == DateTime.Now.Date);
                //var qcresult = qcquery.ToList();
                //var qcids = qcquery.Select(x => x.QCMasterId).ToList();
                var defectlist = _context.tbl_QCDefectDetails.Where(x => qcids.Contains(x.QCMasterId))
                                   .GroupBy(x => new { DefectName = x.DefectName })
                                   .Select(g => new
                                   {
                                       DefectCount = g.Count(),
                                       DefectName = g.Key.DefectName
                                   }).ToList().OrderByDescending(x => x.DefectCount).Take(10);

                List<Defect> _DefectList = new List<Defect>();
                foreach (var onerow in defectlist)
                {
                    Defect _oDefect = new Defect();
                    _oDefect.DefectCount = onerow.DefectCount;
                    _oDefect.DefectName = onerow.DefectName;
                    _DefectList.Add(_oDefect);
                }
                data.DefectList = _DefectList;

                //Rejection List For screen 5 
                //QUALITY BOTTLENECKS
                var qcids_rej = qcquery.Where(x => x.QCStatus == 3).Select(x => x.QCMasterId).ToList();
                var defectlist_rej = _context.tbl_QCDefectDetails.Where(x => qcids_rej.Contains(x.QCMasterId))
                                   .GroupBy(x => new { DefectName = x.DefectName })
                                   .Select(g => new
                                   {
                                       DefectCount = g.Count(),
                                       DefectName = g.Key.DefectName
                                   }).ToList().OrderByDescending(x => x.DefectCount).Take(5);

                List<RejectionDefect> _RejectionDefectList = new List<RejectionDefect>();
                foreach (var onerow in defectlist_rej)
                {
                    RejectionDefect _oDefect = new RejectionDefect();
                    _oDefect.DefectCount = onerow.DefectCount;
                    _oDefect.DefectName = onerow.DefectName;
                    _RejectionDefectList.Add(_oDefect);
                }
                data.RejectionDefectList = _RejectionDefectList;

                //PO Images
                
                var imglist = _context.tbl_POShiftImages.Where(x => x.PONo == pono && x.FactoryID == _UserTokenInfo.FactoryID).ToList();
                POImage _poimage = new POImage();
                foreach (var v in imglist)
                {
                    _poimage = new POImage();
                    _poimage.ImagePath = v.ImagePath;
                    _poimage.ImageName = v.ImageName;
                    _poimage.PONo = v.PONo;
                    imagelist.Add(_poimage);
                }

                if (data == null)
                {
                    status = 204;
                    message = "No record found.";
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(new { status = status, message = message, data, imagelist });
        }

        //HOURLY PRODUCTION SCREEN-1
        [HttpPost]
        [Route("HourlyProduction")]
        public ActionResult HourlyProduction([FromBody] SearchModel _obj)
        {
            HourlyProductionModel data = new HourlyProductionModel();
            List<POImage> imagelist = new List<POImage>();
            var status = 200;
            var message = "Success";
            data.Line = _obj.Line;
            try
            {
                if (string.IsNullOrEmpty(_obj.Module) || string.IsNullOrEmpty(_obj.Line) || string.IsNullOrEmpty(_obj.ShiftName))
                {
                    return Ok(new { status = 400, message = data });

                }
                    _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
        
                var shiftdetails = _context.tbl_Shift.Where(x => x.ShiftName == _obj.ShiftName && x.FactoryID == _UserTokenInfo.FactoryID).FirstOrDefault();

                TimeSpan starttime = TimeSpan.Parse(shiftdetails.ShiftStartTime);
                TimeSpan endtime = TimeSpan.Parse(shiftdetails.ShiftEndTime);

                int starthours = starttime.Hours;
                TimeSpan timespandiff = new TimeSpan();

                if (endtime.Hours <= 12 && starttime.Hours > 12)
                {
                    timespandiff = (endtime + TimeSpan.FromHours(12)) - (starttime - TimeSpan.FromHours(12));
                }
                else {
                    timespandiff = endtime - starttime;
                }
                int hoursdiff = Math.Abs(timespandiff.Hours);
                int minutediff = timespandiff.Minutes;
                if (minutediff > 0)
                    hoursdiff = hoursdiff + 1;

                DateTime ShiftEndDateTime = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, endtime.Hours, endtime.Minutes, 0);
                DateTime CurrDateTime = DateTime.Now;
                TimeSpan t = new TimeSpan();
                t = ShiftEndDateTime - DateTime.Now;
                if (CurrDateTime < ShiftEndDateTime)
                    data.TimeLeft = t.Hours.ToString() + " hr : " + t.Minutes.ToString() + " min ";
                else
                    data.TimeLeft = "0 hr : 0 min ";

                tbl_QCMaster current_execution_po = _context.tbl_QCMaster.Where(x => x.Line == _obj.Line && x.Module == _obj.Module && x.ShiftName == _obj.ShiftName && x.QCDate.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID).OrderBy(x=> x.QCMasterId).LastOrDefault();

                var pono = "";
                if (current_execution_po == null)
                {
                    status = 204;
                    message = "No record found.";
                }
                else
                {
                    pono = current_execution_po.PONo;

                    IQueryable<tbl_Orders> orderquery = _context.tbl_Orders.Where(x => x.PONo == pono && x.PrimaryPart == 1 && x.FactoryID == _UserTokenInfo.FactoryID);
                    var orderresult = orderquery.FirstOrDefault();
                    //Header    
                    data.Product = orderresult.Product;
                    data.Style = orderresult.Style;
                    data.Fit = orderresult.Fit;
                    data.Season = orderresult.Season;
                    data.Customer = orderresult.Customer;
                    data.Module = orderresult.Module;

                    data.SONo = orderresult.SONo;
                    data.PONo = orderresult.PONo;
                    data.FactoryID = orderresult.FactoryID;
                    data.ExFactoryDate = orderresult.ExFactory;
                    data.RecvDate = orderresult.EntryDate;
                    //Header end  


                    double target100 = 0;
                    IQueryable<tbl_LineTarget> planquery = _context.tbl_LineTarget.Where(x => x.PONo == pono && x.Line == _obj.Line && x.Module == _obj.Module && x.ShiftName == _obj.ShiftName && x.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID);
                    var planresult = planquery;

                    if (planresult == null)
                    {
                        //status = 204;
                        //message = "No record found.";
                        data.EffTarget = 0;
                        target100 = 0;
                        data.Target = 0;
                        data.Hourlytarget = 0;
                    }
                    else
                    
                    {
                        foreach (var onerow in planresult)
                        {
                            data.EffTarget += onerow.PlannedEffeciency;
                            target100 += onerow.PlannedTarget * 100 / onerow.PlannedEffeciency;
                            data.Target += onerow.PlannedTarget;
                            data.Hourlytarget += Convert.ToInt16(onerow.PlannedTarget / onerow.ShiftHours);
                        }
                        if(planresult.Count()>0)
                            data.EffTarget = Math.Ceiling(data.EffTarget / planresult.Count());
                    }

                    IQueryable<tbl_QCMaster> qcquery = _context.tbl_QCMaster.Where(x => x.PONo == pono && x.ShiftName == _obj.ShiftName && x.QCDate.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line);
                    var qcresult = qcquery.ToList();
                    //bottom   


                    data.OutputGoodPCS = qcquery.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                    data.ReWorkDefectPCS = qcquery.Where(x => x.QCStatus == 2 && x.TypeOfWork == 1 ).Sum(x => x.Qty)
                                            - qcquery.Where(x => x.TypeOfWork == 2 && x.QCStatus != 2).Sum(x => x.Qty);
                    data.ReWorkDefectPCS = data.ReWorkDefectPCS < 0 ? 0 : data.ReWorkDefectPCS;
                    data.RejectPCS = qcquery.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                    data.PicesChecked = qcquery.Where(x=> x.TypeOfWork ==1).Sum(x => x.Qty);

                    data.PicesCheckedForDisplay = data.OutputGoodPCS + data.ReWorkDefectPCS + data.RejectPCS;

                    data.Variation = data.OutputGoodPCS - data.Target;
                    if (target100 > 0)
                        data.EffAchieve = Math.Ceiling(data.OutputGoodPCS * 100 / target100);
                    else
                        data.EffAchieve = 0;

                    if (data.PicesChecked > 0)
                    {
                        var QCMasterIds = qcquery.Select(x => x.QCMasterId).ToList();
                        var TotalDefects = _context.tbl_QCDefectDetails.Where(x => QCMasterIds.Contains(x.QCMasterId)).Count();
                        data.DefectRate = (TotalDefects * 100) / data.PicesChecked;

                        data.RejectRate = (data.RejectPCS * 100) / data.PicesChecked;
                    }
                    else
                    {
                        data.DefectRate = 0;
                        data.RejectRate = 0;
                    }
                    //bottom  end 

                    //list
                    var qcids = qcquery.Select(x => x.QCMasterId).ToList();

                    List<HourlyProductionList> _HourlyProductionList = new List<HourlyProductionList>();

                    //var totaloutput = 0;
                    var hrscount = 0;
                    int counter = 0;
                    var showoutput =0;

                    while (counter < hoursdiff)
                    {
                        TimeSpan endMins = endtime;
                        if (minutediff > 0 && counter + 1 == hoursdiff)
                        {
                            endMins = endtime - TimeSpan.FromHours(1);
                        }
                        else
                            endMins = starttime;
                        TimeSpan to_endTime = endMins + TimeSpan.FromHours(1);
                        DateTime strdate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, starttime.Hours, starttime.Minutes, 0);
                        DateTime enddate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Month, DateTime.Now.Day, to_endTime.Hours, to_endTime.Minutes, 0);
                        
                        HourlyProductionList lst = new HourlyProductionList();

                        TimeSpan starttime_to = enddate.TimeOfDay;


                        lst.Hour = starttime.ToString(@"hh\:mm") + "-" + starttime_to.ToString(@"hh\:mm");
 
                        lst.Target = data.Hourlytarget;
                        lst.Output = qcquery.Where(x => x.QCStatus == 1 && x.QCDate >= strdate && x.QCDate <= enddate).Sum(x => x.Qty);

                        TimeSpan start = TimeSpan.Parse(starttime.ToString(@"hh\:mm")); 
                        TimeSpan end = TimeSpan.Parse(starttime_to.ToString(@"hh\:mm")); 
                        TimeSpan now = DateTime.Now.TimeOfDay;
                        if ((now > start) && (now < end))
                        {
                            showoutput = lst.Output;
                        }
                        //totaloutput += lst.Output;
                        hrscount++;
                        lst.Variation = lst.Output - lst.Target;
                        var PicesChecked = qcquery.Where(x => x.QCDate >= strdate && x.QCDate <= enddate && x.TypeOfWork == 1).Sum(x => x.Qty);
                        if (PicesChecked > 0)
                        {
                            var hrsqcid = qcquery.Where(x => x.QCStatus > 1 && x.QCDate >= strdate && x.QCDate <= enddate && x.FactoryID == _UserTokenInfo.FactoryID).Select(x => x.QCMasterId).ToList();
                            var hrstotaldefects = _context.tbl_QCDefectDetails.Where(x => hrsqcid.Contains(x.QCMasterId)).Count();
                            lst.DefectRate = hrstotaldefects * 100 / PicesChecked;
                        }
                        else
                        {
                            lst.DefectRate = 0;
                        }
                        _HourlyProductionList.Add(lst);
                        starttime += TimeSpan.FromHours(1);
                        counter += 1;
                    }
                    data.HourlyProductionList = _HourlyProductionList;
                    data.HourlyAchieve = showoutput;

                    //IQueryable<tbl_QCMaster> qcquery = _context.tbl_QCMaster.Where(x => x.PONo == pono && x.ShiftName == _obj.ShiftName && x.QCDate == DateTime.Now.Date);
                    //var qcresult = qcquery.ToList();
                    //var qcids = qcquery.Select(x => x.QCMasterId).ToList();
                    var defectlist = _context.tbl_QCDefectDetails.Where(x => qcids.Contains(x.QCMasterId))
                                       .GroupBy(x => new { DefectName = x.DefectName })
                                       .Select(g => new
                                       {
                                           DefectCount = g.Count(),
                                           DefectName = g.Key.DefectName
                                       }).ToList().OrderByDescending(x => x.DefectCount).Take(10);

                    List<Defect> _DefectList = new List<Defect>();
                    foreach (var onerow in defectlist)
                    {
                        Defect _oDefect = new Defect();
                        _oDefect.DefectCount = onerow.DefectCount;
                        _oDefect.DefectName = onerow.DefectName;
                        _DefectList.Add(_oDefect);
                    }
                    data.DefectList = _DefectList;

                    //Rejection List For screen 5 
                    //QUALITY BOTTLENECKS
                    var qcids_rej = qcquery.Where(x => x.QCStatus == 3).Select(x => x.QCMasterId).ToList();
                    var defectlist_rej = _context.tbl_QCDefectDetails.Where(x => qcids_rej.Contains(x.QCMasterId))
                                       .GroupBy(x => new { DefectName = x.DefectName })
                                       .Select(g => new
                                       {
                                           DefectCount = g.Count(),
                                           DefectName = g.Key.DefectName
                                       }).ToList().OrderByDescending(x => x.DefectCount).Take(5);

                    List<RejectionDefect> _RejectionDefectList = new List<RejectionDefect>();
                    foreach (var onerow in defectlist_rej)
                    {
                        RejectionDefect _oDefect = new RejectionDefect();
                        _oDefect.DefectCount = onerow.DefectCount;
                        _oDefect.DefectName = onerow.DefectName;
                        _RejectionDefectList.Add(_oDefect);
                    }
                    data.RejectionDefectList = _RejectionDefectList;

                    //PO Images

                    var imglist = _context.tbl_POShiftImages.Where(x => x.PONo == pono && x.FactoryID == _UserTokenInfo.FactoryID).ToList();
                    POImage _poimage = new POImage();
                    foreach (var v in imglist)
                    {
                        _poimage = new POImage();
                        _poimage.ImagePath = v.ImagePath;
                        _poimage.ImageName = v.ImageName;
                        _poimage.PONo = v.PONo;
                        imagelist.Add(_poimage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(new { status = status, message = message, data, imagelist });
        }

        //DEFECT HOTSPOT SCREEN-2
        [HttpPost]
        [Route("DefectHotspotBackup")]
        public ActionResult DefectHotspotBackup([FromBody] SearchModel _obj)
        {
            HourlyProductionModel data = new HourlyProductionModel();
            List<POImage> imagelist = new List<POImage>();
            var status = 200;
            var message = "Success";
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                //_obj.Line = "L1";
                //_obj.Module = "FL1";
                //_obj.ShiftName = "S1";
                var shiftdetails = _context.tbl_Shift.Where(x => x.ShiftName == _obj.ShiftName && x.FactoryID ==_UserTokenInfo.FactoryID).FirstOrDefault();
                double starttime = Convert.ToDouble(shiftdetails.ShiftStartTime);
                double endtime = Convert.ToDouble(shiftdetails.ShiftEndTime);

                IQueryable<tbl_LineTarget> planquery = _context.tbl_LineTarget.Where(x => x.Line == _obj.Line && x.Module == _obj.Module && x.ShiftName == _obj.ShiftName && x.Date == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID);
                var planresult = planquery.FirstOrDefault();
                if (planresult == null)
                {
                    status = 204;
                    message = "No record found.";
                }

                var pono = planresult.PONo;
                IQueryable<tbl_Orders> orderquery = _context.tbl_Orders.Where(x => x.PONo == pono && x.PrimaryPart == 1 && x.FactoryID == _UserTokenInfo.FactoryID);
                var orderresult = orderquery.FirstOrDefault();
                //Header    
                data.Product = orderresult.Product;
                data.Style = orderresult.Style;
                data.Fit = orderresult.Fit;
                data.Season = orderresult.Season;
                data.Customer = orderresult.Customer;
                data.Module = orderresult.Module;
                data.Line = planresult.Line;
                data.SONo = orderresult.SONo;
                data.PONo = orderresult.PONo;
                data.FactoryID = orderresult.FactoryID;
                data.ExFactoryDate = orderresult.ExFactory;
                data.RecvDate = orderresult.EntryDate;
                //Header end                  

                IQueryable<tbl_QCMaster> qcquery = _context.tbl_QCMaster.Where(x => x.PONo == pono && x.ShiftName == _obj.ShiftName && x.QCDate == DateTime.Now.Date && x.FactoryID == _UserTokenInfo.FactoryID);
                var qcresult = qcquery.ToList();
                var qcids = qcquery.Select(x => x.QCMasterId).ToList();
                var defectlist = _context.tbl_QCDefectDetails.Where(x => qcids.Contains(x.QCMasterId))
                                   .GroupBy(x => new { DefectName = x.DefectName })
                                   .Select(g => new
                                   {
                                       DefectCount = g.Count(),
                                       DefectName = g.Key.DefectName
                                   }).ToList().OrderByDescending(x => x.DefectCount).Take(10);

                List<Defect> _DefectList = new List<Defect>();
                foreach (var onerow in defectlist)
                {
                    Defect _oDefect = new Defect();
                    _oDefect.DefectCount = onerow.DefectCount;
                    _oDefect.DefectName = onerow.DefectName;
                    _DefectList.Add(_oDefect);
                }
                data.DefectList = _DefectList;

                //Rejection List For screen 5 
                //QUALITY BOTTLENECKS
                var qcids_rej = qcquery.Where(x => x.QCStatus == 3).Select(x => x.QCMasterId).ToList();
                var defectlist_rej = _context.tbl_QCDefectDetails.Where(x => qcids_rej.Contains(x.QCMasterId))
                                   .GroupBy(x => new { DefectName = x.DefectName })
                                   .Select(g => new
                                   {
                                       DefectCount = g.Count(),
                                       DefectName = g.Key.DefectName
                                   }).ToList().OrderByDescending(x => x.DefectCount).Take(5);

                List<RejectionDefect> _RejectionDefectList = new List<RejectionDefect>();
                foreach (var onerow in defectlist_rej)
                {
                    RejectionDefect _oDefect = new RejectionDefect();
                    _oDefect.DefectCount = onerow.DefectCount;
                    _oDefect.DefectName = onerow.DefectName;
                    _RejectionDefectList.Add(_oDefect);
                }
                data.RejectionDefectList = _RejectionDefectList;

                if (data == null)
                {
                    status = 204;
                    message = "No record found.";
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(new { status = status, message = message, data });
        }
    }
    public class HourlyProductionModel
    {
        public int Target { get; set; }
        public int OutputGoodPCS { get; set; }
        public int ReWorkDefectPCS { get; set; }
        public int RejectPCS { get; set; }
        public int Variation { get; set; }
        public int PicesChecked { get; set; }
        public int PicesCheckedForDisplay { get; set; }
        
        public double DefectRate { get; set; }
        public int Hourlytarget { get; set; }

        public string Module { get; set; }
        public string Line { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string Product { get; set; }
        public string Season { get; set; }
        public string Customer { get; set; }
        public int FactoryID { get; set; }
        public DateTime RecvDate { get; set; }
        public DateTime ExFactoryDate { get; set; }
        public List<HourlyProductionList> HourlyProductionList { get; set; }
        public List<Defect> DefectList { get; set; }
        public List<RejectionDefect> RejectionDefectList { get; set; }

        public string TimeLeft { get; set; }
        public int HourlyAchieve { get; set; }
        public double EffTarget { get; set; }
        public double EffAchieve { get; set; }
        public double RejectRate { get; set; }
    }

    public class HourlyProductionList
    {
        public string Hour { get; set; }
        public int Target { get; set; }
        public int Output { get; set; }
        public int Variation { get; set; }
        public double DefectRate { get; set; }
    }

    public class Defect
    {
        public string DefectName { get; set; }
        public int DefectCount { get; set; }
    }

    public class RejectionDefect
    {
        public string DefectName { get; set; }
        public int DefectCount { get; set; }
    }
}
