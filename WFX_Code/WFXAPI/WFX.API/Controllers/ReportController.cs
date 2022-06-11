using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using WFX.Data;
using WFX.Entities;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {

        DBContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public ReportController(IWebHostEnvironment env, ILogger<OrderController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;

        }

        //ProductionSummaryModel
        [HttpPost]
        [Route("ProductionSummary")]
        public ActionResult ProductionSummary([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Color = x.Color, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date, QCDay = x.QCDate.Day, QCMonth = x.QCDate.Month })
                                   .Select(g => new
                                   {
                                       QCDate = g.Key.QCDate,
                                       Module = g.Key.Module,
                                       ProcessName = g.Key.ProcessName,
                                       Line = g.Key.Line,
                                       SONo = g.Key.SONo,
                                       PONo = g.Key.PONo,
                                       Customer = g.Key.Customer,
                                       Product = g.Key.Product,
                                       Style = g.Key.Style,
                                       Color = g.Key.Color,
                                       PiecesChecked = g.Sum(x => x.Qty)
                                   }).ToList().OrderBy(x => x.QCDate);

                List<ProductionSummaryModel> data = new List<ProductionSummaryModel>();
                ProductionSummaryModel _productionSummaryModel = new ProductionSummaryModel();
                foreach (var v in result)
                {
                    _productionSummaryModel = new ProductionSummaryModel();
                    _productionSummaryModel.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _productionSummaryModel.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _productionSummaryModel.Module = v.Module;
                    _productionSummaryModel.ProcessName = v.ProcessName;
                    _productionSummaryModel.Customer = v.Customer;
                    _productionSummaryModel.Line = v.Line;
                    _productionSummaryModel.Style = v.Style;
                    _productionSummaryModel.Product = v.Product;
                    _productionSummaryModel.SO = v.SONo;
                    _productionSummaryModel.PO = v.PONo;
                    _productionSummaryModel.Color = v.Color;
                    _productionSummaryModel.PiecesChecked = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCDate.Month == v.QCDate.Month && x.QCDate.Day == v.QCDate.Day && x.TypeOfWork == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color && x.Line == v.Line).Sum(x => x.Qty);
                    _productionSummaryModel.PlannedOutput = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color && x.Line == v.Line).Sum(x => x.PlannedTarget);
                    _productionSummaryModel.ActualOutput = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color && x.Line == v.Line).Sum(x => x.Qty);
                    double smv = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color).Select(x => x.SMV).FirstOrDefault();
                    _productionSummaryModel.SMV = smv.ToString();
                    _productionSummaryModel.PlannedSAH = Convert.ToInt32(_productionSummaryModel.PlannedOutput * smv / 60);
                    _productionSummaryModel.ActualSAH = Convert.ToInt32(_productionSummaryModel.ActualOutput * smv / 60);
                    int plannEffCount = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color &&
                                    x.Line == v.Line).Count();
                    double plannEffSum = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Color == v.Color &&
                                                       x.Line == v.Line).Sum(x => x.PlannedEffeciency);
                    if (plannEffCount > 0)
                    {
                        _productionSummaryModel.PlannedEff = Math.Ceiling(plannEffSum / plannEffCount);
                    }
                    else
                    {
                        _productionSummaryModel.PlannedEff = 0;
                    }
                    if (_productionSummaryModel.PlannedEff > 0)
                    {
                        var target100 = _productionSummaryModel.PlannedOutput * 100 / _productionSummaryModel.PlannedEff;
                        _productionSummaryModel.ActualEff = Math.Ceiling(_productionSummaryModel.ActualOutput * 100 / target100);
                    }
                    _productionSummaryModel.PlannedManHours = 0;
                    _productionSummaryModel.ActualManHours = 0;
                    data.Add(_productionSummaryModel);
                }

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //EndlineQCSummary
        [HttpPost]
        [Route("EndlineQCSummary")]
        public ActionResult EndlineQCSummary([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                //var result = qcquery.OrderBy(x=> x.QCDate).ToList();
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date, ShiftName = x.ShiftName, ShiftStartTime = x.ShiftStartTime, ShiftEndTime = x.ShiftEndTime, UserFirstName = x.UserFirstName })
                                   .Select(g => new
                                   {
                                       QCDate = g.Key.QCDate,
                                       Module = g.Key.Module,
                                       ProcessName = g.Key.ProcessName,
                                       Line = g.Key.Line,
                                       SONo = g.Key.SONo,
                                       PONo = g.Key.PONo,
                                       Customer = g.Key.Customer,
                                       Product = g.Key.Product,
                                       Style = g.Key.Style,
                                       ShiftName = g.Key.ShiftName,
                                       ShiftStartTime = g.Key.ShiftStartTime,
                                       ShiftEndTime = g.Key.ShiftEndTime,
                                       UserFirstName = g.Key.UserFirstName,
                                       PiecesChecked = g.Sum(x => x.Qty)
                                   }).ToList().OrderBy(x => x.QCDate);

                IQueryable<tbl_QCMaster> qcmaster = _context.tbl_QCMaster.Where(x=> x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);

                List<EndlineQCSummaryModel> data = new List<EndlineQCSummaryModel>();
                EndlineQCSummaryModel _EndlineQCSummaryModel = new EndlineQCSummaryModel();
                foreach (var v in result)
                {
                    _EndlineQCSummaryModel = new EndlineQCSummaryModel();
                    _EndlineQCSummaryModel.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _EndlineQCSummaryModel.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _EndlineQCSummaryModel.Module = v.Module;
                    _EndlineQCSummaryModel.ProcessName = v.ProcessName;
                    _EndlineQCSummaryModel.Line = v.Line;
                    _EndlineQCSummaryModel.QC = v.UserFirstName;
                    _EndlineQCSummaryModel.ShiftTimings =  v.ShiftStartTime + "-" + v.ShiftEndTime;
                    _EndlineQCSummaryModel.SO = v.SONo;
                    _EndlineQCSummaryModel.PO = v.PONo;
                    _EndlineQCSummaryModel.Style = v.Style;
                    _EndlineQCSummaryModel.PiecesChecked = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCDate.Month == v.QCDate.Month && x.QCDate.Day == v.QCDate.Day && x.TypeOfWork == 1  && x.Line == v.Line && x.ShiftName == v.ShiftName).Sum(x => x.Qty); 
                    _EndlineQCSummaryModel.DefectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 2 && x.TypeOfWork == 1 && x.Line == v.Line && x.ShiftName == v.ShiftName).Sum(x => x.Qty);
                    _EndlineQCSummaryModel.ReworkedPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.TypeOfWork == 2 && x.ShiftName == v.ShiftName && x.Line == v.Line).Sum(x => x.Qty);
                    _EndlineQCSummaryModel.RejectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 3 && x.ShiftName == v.ShiftName && x.Line == v.Line).Sum(x => x.Qty);
                    _EndlineQCSummaryModel.ActualOutput = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.ShiftName == v.ShiftName && x.Line == v.Line).Sum(x => x.Qty);
                    if (_EndlineQCSummaryModel.PiecesChecked > 0)
                    {
                        _EndlineQCSummaryModel.ReworkRate = (_EndlineQCSummaryModel.ReworkedPieces * 100) / _EndlineQCSummaryModel.PiecesChecked;
                        _EndlineQCSummaryModel.RejectRate = (_EndlineQCSummaryModel.RejectPieces * 100) / _EndlineQCSummaryModel.PiecesChecked;
                    }
                    else
                    {
                        _EndlineQCSummaryModel.ReworkRate = 0;
                        _EndlineQCSummaryModel.RejectRate = 0;
                    }
                    data.Add(_EndlineQCSummaryModel);
                }

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //TargetVsActual
        [HttpPost]
        [Route("TargetVsActual")]
        public ActionResult TargetVsActual([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date })
                                   .Select(g => new
                                   {
                                       QCDate = g.Key.QCDate,
                                       Module = g.Key.Module,
                                       ProcessName = g.Key.ProcessName,
                                       Line = g.Key.Line,
                                       SONo = g.Key.SONo,
                                       PONo = g.Key.PONo,
                                       Customer = g.Key.Customer,
                                       Product = g.Key.Product,
                                       Style = g.Key.Style,
                                       PiecesChecked = g.Sum(x => x.Qty)
                                   }).ToList().OrderBy(x => x.QCDate);

                List<TargetVsActualModel> data = new List<TargetVsActualModel>();
                TargetVsActualModel _TargetVsActualModel = new TargetVsActualModel();
                foreach (var v in result)
                {
                    _TargetVsActualModel = new TargetVsActualModel();
                    _TargetVsActualModel.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _TargetVsActualModel.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _TargetVsActualModel.Module = v.Module;
                    _TargetVsActualModel.ProcessName = v.ProcessName;
                    _TargetVsActualModel.Customer = v.Customer;
                    _TargetVsActualModel.Line = v.Line;
                    _TargetVsActualModel.Style = v.Style;
                    _TargetVsActualModel.Product = v.Product;
                    _TargetVsActualModel.SO = v.SONo;
                    _TargetVsActualModel.PO = v.PONo;
                    _TargetVsActualModel.PiecesChecked = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCDate.Month == v.QCDate.Month && x.QCDate.Day == v.QCDate.Day && x.TypeOfWork == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Sum(x => x.Qty);

                    _TargetVsActualModel.PlannedOutput = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Sum(x => x.PlannedTarget);
                    _TargetVsActualModel.ActualOutput = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Sum(x => x.Qty);
                    _TargetVsActualModel.OutPutVariation = _TargetVsActualModel.PlannedOutput - _TargetVsActualModel.ActualOutput;

                    double smv = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Select(x => x.SMV).FirstOrDefault();
                    _TargetVsActualModel.SMV = smv.ToString();

                    _TargetVsActualModel.PlannedSAH = Convert.ToInt32(_TargetVsActualModel.PlannedOutput * smv / 60);
                    _TargetVsActualModel.ActualSAH = Convert.ToInt32(_TargetVsActualModel.ActualOutput * smv / 60);
                    _TargetVsActualModel.SAHVariation = _TargetVsActualModel.PlannedSAH - _TargetVsActualModel.ActualSAH;

                    int plannEffCount = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID &&
                                                        x.Line == v.Line).Count();
                    double plannEffSum = _context.tbl_LineTarget.Where(x => x.Date.Date == v.QCDate.Date && x.PONo == v.PONo && x.FactoryID == _UserTokenInfo.FactoryID &&
                                                       x.Line == v.Line).Sum(x => x.PlannedEffeciency);
                    if (plannEffCount > 0)
                    {
                        _TargetVsActualModel.PlannedEff = Math.Ceiling(plannEffSum / plannEffCount);
                    }
                    else
                    {
                        _TargetVsActualModel.PlannedEff = 0;

                    }
                    if (_TargetVsActualModel.PlannedEff > 0)
                    {
                        var target100 = _TargetVsActualModel.PlannedOutput * 100 / _TargetVsActualModel.PlannedEff;
                        _TargetVsActualModel.ActualEff = Math.Ceiling(_TargetVsActualModel.ActualOutput * 100 / target100);
                    }
                    _TargetVsActualModel.EffVariation = _TargetVsActualModel.PlannedEff - _TargetVsActualModel.ActualEff;

                    _TargetVsActualModel.PlannedManHours = 0;
                    _TargetVsActualModel.ActualManHours = 0;
                    _TargetVsActualModel.HrsVariation = 0;

                    data.Add(_TargetVsActualModel);
                }

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //HourlyProduction
        [HttpPost]
        [Route("HourlyProduction")]
        public ActionResult HourlyProduction([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date, UserFirstName = x.UserFirstName, Hrs = x.QCDate.Hour })
                                   .Select(g => new
                                   {
                                       QCDate = g.Key.QCDate,
                                       Module = g.Key.Module,
                                       ProcessName = g.Key.ProcessName,
                                       Line = g.Key.Line,
                                       SONo = g.Key.SONo,
                                       PONo = g.Key.PONo,
                                       Customer = g.Key.Customer,
                                       Product = g.Key.Product,
                                       Style = g.Key.Style,
                                       UserFirstName = g.Key.UserFirstName,
                                       Hrs = g.Key.Hrs,
                                       PiecesChecked = g.Sum(x => x.Qty)
                                   }).ToList().OrderBy(x => x.QCDate);

                IQueryable<tbl_QCMaster> qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);

                List<HourlyProduction> data = new List<HourlyProduction>();
                HourlyProduction _HourlyProduction = new HourlyProduction();
                foreach (var v in result)
                {
                    _HourlyProduction = new HourlyProduction();
                    _HourlyProduction.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _HourlyProduction.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _HourlyProduction.Module = v.Module;
                    _HourlyProduction.ProcessName = v.ProcessName;
                    _HourlyProduction.Line = v.Line;
                    _HourlyProduction.QC = v.UserFirstName;
                    _HourlyProduction.StartHour = v.Hrs.ToString();
                    _HourlyProduction.EndHour = (v.Hrs + 1).ToString();
                    _HourlyProduction.SO = v.SONo;
                    _HourlyProduction.PO = v.PONo;
                    _HourlyProduction.Style = v.Style;
                    _HourlyProduction.PiecesChecked = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCDate.Month == v.QCDate.Month && x.QCDate.Day == v.QCDate.Day && x.QCDate.Hour == v.Hrs && x.TypeOfWork == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Sum(x => x.Qty);
                    _HourlyProduction.DefectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 2 && x.QCDate.Hour == v.Hrs && x.TypeOfWork == 1 && x.Line == v.Line).Sum(x => x.Qty);
                    _HourlyProduction.ReworkedPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.TypeOfWork == 2 && x.QCDate.Hour == v.Hrs && x.Line == v.Line).Sum(x => x.Qty);
                    _HourlyProduction.RejectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 3 && x.QCDate.Hour == v.Hrs && x.Line == v.Line).Sum(x => x.Qty);

                    _HourlyProduction.ActualOutput = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.QCDate.Hour == v.Hrs && x.Line == v.Line).Sum(x => x.Qty);

                    if (_HourlyProduction.PiecesChecked > 0)
                    {
                        _HourlyProduction.ReworkRate = (_HourlyProduction.ReworkedPieces * 100) / _HourlyProduction.PiecesChecked;
                        _HourlyProduction.RejectRate = (_HourlyProduction.RejectPieces * 100) / _HourlyProduction.PiecesChecked;
                    }
                    else
                    {
                        _HourlyProduction.ReworkRate = 0;
                        _HourlyProduction.RejectRate = 0;
                    }
                        

                    data.Add(_HourlyProduction);
                }

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //QualityPerfromance
        [HttpPost]
        [Route("QualityPerfromance")]
        public ActionResult QualityPerfromance([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date, UserFirstName = x.UserFirstName })
                                   .Select(g => new
                                   {
                                       QCDate = g.Key.QCDate,
                                       Module = g.Key.Module,
                                       ProcessName = g.Key.ProcessName,
                                       Line = g.Key.Line,
                                       SONo = g.Key.SONo,
                                       PONo = g.Key.PONo,
                                       Customer = g.Key.Customer,
                                       Product = g.Key.Product,
                                       Style = g.Key.Style,
                                       UserFirstName = g.Key.UserFirstName,
                                       PiecesChecked = g.Sum(x => x.Qty)
                                   }).ToList().OrderBy(x => x.QCDate);

                IQueryable<tbl_QCMaster> qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcmaster = qcmaster.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcmaster = qcmaster.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcmaster = qcmaster.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcmaster = qcmaster.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcmaster = qcmaster.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);

                List<QualityPerfromanceModel> data = new List<QualityPerfromanceModel>();
                QualityPerfromanceModel _QualityPerfromanceModel = new QualityPerfromanceModel();
                foreach (var v in result)
                {
                    _QualityPerfromanceModel = new QualityPerfromanceModel();
                    _QualityPerfromanceModel.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _QualityPerfromanceModel.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    _QualityPerfromanceModel.Module = v.Module;
                    _QualityPerfromanceModel.ProcessName = v.ProcessName;
                    _QualityPerfromanceModel.Line = v.Line;
                    _QualityPerfromanceModel.QC = v.UserFirstName;
                    _QualityPerfromanceModel.SO = v.SONo;
                    _QualityPerfromanceModel.PO = v.PONo;
                    _QualityPerfromanceModel.Style = v.Style;
                    _QualityPerfromanceModel.PiecesChecked = _context.tbl_QCMaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCDate.Month == v.QCDate.Month && x.QCDate.Day == v.QCDate.Day && x.TypeOfWork == 1 && x.FactoryID == _UserTokenInfo.FactoryID && x.Line == v.Line).Sum(x => x.Qty);
                    _QualityPerfromanceModel.DefectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 2 && x.Line == v.Line && x.TypeOfWork == 1).Sum(x => x.Qty);
                    _QualityPerfromanceModel.ReworkedPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.TypeOfWork == 2 && x.Line == v.Line).Sum(x => x.Qty);
                    _QualityPerfromanceModel.RejectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 3 && x.Line == v.Line).Sum(x => x.Qty);
                    _QualityPerfromanceModel.ActualOutput = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.Line == v.Line).Sum(x => x.Qty);
                    if (_QualityPerfromanceModel.PiecesChecked > 0)
                    {
                        _QualityPerfromanceModel.ReworkRate = _QualityPerfromanceModel.ReworkedPieces * 100 / _QualityPerfromanceModel.PiecesChecked;
                        _QualityPerfromanceModel.RejectRate = _QualityPerfromanceModel.RejectPieces * 100 / _QualityPerfromanceModel.PiecesChecked;
                        var DefectData = from p in qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus != 1 && x.Line == v.Line)
                                         join s in _context.tbl_QCDefectDetails
                                           on new { p.QCMasterId } equals new { s.QCMasterId }
                                         select new { s.QCDefectDetailsId };
                        double DefectCount = DefectData.ToList().Count;
                        _QualityPerfromanceModel.DHU = Math.Floor(DefectCount * 100 / _QualityPerfromanceModel.PiecesChecked);
                    }
                    else
                    {
                        _QualityPerfromanceModel.ReworkRate = 0;
                        _QualityPerfromanceModel.RejectRate = 0;
                        _QualityPerfromanceModel.DHU = 0;
                    }
                    

                    data.Add(_QualityPerfromanceModel);
                }

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DefectAnalysis
        [HttpPost]
        [Route("DefectAnalysis")]
        public ActionResult DefectAnalysis([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_QCMaster> qcquery = _context.vw_QCMaster.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcquery = qcquery.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                var result = qcquery.GroupBy(x => new { FactoryID = x.FactoryID, Module = x.Module, ProcessName = x.ProcessName, Line = x.Line, SONo = x.SONo, PONo = x.PONo, Color = x.Color, Style = x.Style, Product = x.Product, Customer = x.Customer, QCDate = x.QCDate.Date })
                    .Select(g => new
                    {
                        QCDate = g.Key.QCDate,
                        Module = g.Key.Module,
                        ProcessName = g.Key.ProcessName,
                        Line = g.Key.Line,
                        SONo = g.Key.SONo,
                        PONo = g.Key.PONo,
                        Customer = g.Key.Customer,
                        Product = g.Key.Product,
                        Style = g.Key.Style,
                        Color = g.Key.Color,
                        PiecesChecked = g.Sum(x => x.Qty)
                    }).ToList().OrderBy(x => x.QCDate);

                IQueryable<vw_QC> qcchild = _context.vw_QC.Where(x => x.QCDate.Date >= _obj.StartDate && x.QCDate.Date <= _obj.EndDate && x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcchild = qcchild.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcchild = qcchild.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    qcchild = qcchild.Where(x => x.ProcessName == _obj.ProcessName);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcchild = qcchild.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                IQueryable<tbl_QCMaster> qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    qcquery = qcquery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.Module))
                    qcquery = qcquery.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.Line))
                    qcquery = qcquery.Where(x => x.Line == _obj.Line);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    qcquery = qcquery.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.Style))
                    qcquery = qcquery.Where(x => x.Style == _obj.Style);
                dynamic data = new List<ExpandoObject>();
                var oplist = from p in qcmaster
                             join s in _context.tbl_QCDefectDetails
                                 on new { p.QCMasterId } equals new { s.QCMasterId }
                             select new { s.OperationName }.OperationName;

                foreach (var v in result)
                {
                    
                    dynamic dynamicObj = new ExpandoObject();
                    var defectresult = qcchild.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.Color == v.Color && x.Line == v.Line && x.QCDefectDetailsId > 0 )
                                        .GroupBy(x => new { DefectName = x.DefectName})                           
                                        .Select(g => new
                                        {
                                            DefectName = g.Key.DefectName,        
                                            DefectCount = g.Count(),
                                        }).Distinct().ToList().OrderBy(x => x.DefectName);
 
                    dynamicObj.StartDate = v.QCDate.ToString("dd/MMM/yyyy");
                    dynamicObj.EndDate = v.QCDate.ToString("dd/MMM/yyyy");
                    dynamicObj.Module = v.Module;
                    dynamicObj.ProcessName = v.ProcessName;
                    dynamicObj.Customer = v.Customer;
                    dynamicObj.Line = v.Line;
                    dynamicObj.Style = v.Style;
                    dynamicObj.Product = v.Product;
                    dynamicObj.SO = v.SONo;
                    dynamicObj.PO = v.PONo;
                    dynamicObj.Color = v.Color;

                    if (defectresult.Count() > 0)
                    {

                        foreach (var defectrow in defectresult)
                        {
                            dynamic dynamicObj1 = new ExpandoObject();
                            dynamicObj1.PiecesChecked = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.Color == v.Color && x.Line == v.Line
                                && x.FactoryID == _UserTokenInfo.FactoryID && x.TypeOfWork == 1).Sum(x => x.Qty);
                          
                            dynamicObj1.DefectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 2 && x.Color == v.Color
                                && x.Line == v.Line && x.TypeOfWork == 1).Sum(x => x.Qty);
                            if (dynamicObj1.PiecesChecked > 0)
                            {
                                dynamicObj1.DefectRate = (dynamicObj1.DefectPieces * 100) / dynamicObj1.PiecesChecked;

                                dynamicObj1.ReworkedPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.TypeOfWork == 2 && x.Color == v.Color
                                    && x.Line == v.Line).Sum(x => x.Qty);
                                dynamicObj1.RejectPieces = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 3 && x.Color == v.Color && x.Line == v.Line).Sum(x => x.Qty);
                                dynamicObj1.RejectRate = (dynamicObj1.RejectPieces * 100) / dynamicObj1.PiecesChecked;

                                dynamicObj1.ActualOutput = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.Color == v.Color && x.Line == v.Line).Sum(x => x.Qty);

                                var DefectData = from p in qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus != 1 && x.Line == v.Line && x.Color == v.Color)
                                                 join s in _context.tbl_QCDefectDetails
                                                   on new { p.QCMasterId } equals new { s.QCMasterId }
                                                 select new { s.QCDefectDetailsId };
                                double DefectCount = DefectData.ToList().Count;
                                dynamicObj1.DHU = Math.Floor(DefectCount * 100 / dynamicObj1.PiecesChecked);
                                dynamicObj1.DefectsFound = defectrow.DefectName;
                            }
                            else
                            {
                                dynamicObj1.DefectRate = 0;
                                dynamicObj1.ReworkedPieces = 0;
                                dynamicObj1.RejectPieces = 0;
                                dynamicObj1.RejectRate = 0;
                                dynamicObj1.ActualOutput = 0;
                                dynamicObj1.DHU = 0;
                                dynamicObj1.DefectsFound = '-';
                            }

                            foreach (var op in oplist)
                            {
                                int defectFound = qcchild.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.Color == v.Color
                                           && x.Line == v.Line && x.QCDefectDetailsId > 0 && x.DefectName == defectrow.DefectName
                                           && x.OperationName == op && x.QCStatus >1)
                                       .Count();

                                AddProperty(dynamicObj1, op, defectFound);
                            }
                            dynamicObj = Merge(dynamicObj, dynamicObj1);
                            data.Add(dynamicObj);
                        }
                    }
                    else
                    {
                        dynamicObj.PiecesChecked = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.Color == v.Color && x.Line == v.Line
                        && x.FactoryID == _UserTokenInfo.FactoryID && x.TypeOfWork == 1).Sum(x => x.Qty);
                        dynamicObj.DefectPieces = 0;
                        dynamicObj.DefectRate = 0;
                        dynamicObj.ReworkedPieces = 0;
                        dynamicObj.RejectPieces = 0;
                        dynamicObj.RejectRate = 0;
                        dynamicObj.ActualOutput = qcmaster.Where(x => x.QCDate.Date == v.QCDate.Date && x.PONo == v.PONo && x.QCStatus == 1 && x.Color == v.Color && x.Line == v.Line).Sum(x => x.Qty);
                        dynamicObj.DHU = 0;
                        dynamicObj.DefectsFound = '-';
                        data.Add(dynamicObj);
                    }
                }
                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private dynamic Merge(object item1, object item2)
        {
            IDictionary<string, object> result = new ExpandoObject();

            foreach (var property in (IDictionary<string,object>) item1)
            {
                result[property.Key] = property.Value;
            }

            foreach (var property in (IDictionary<string, object>)item2)
            {
                result[property.Key] = property.Value;
            }

            return result;
        }

        [HttpGet]
        [Route("expando")]
        public IActionResult expando()
        {
            try
            {
                string[] str = { "S", "M", "L", "XL" };
                List<ExpandoObject> _lst = new List<ExpandoObject>();
                for (int i = 0; i < str.Length; i++)
                {
                    dynamic dynamicObj = new ExpandoObject();
                    dynamicObj.FirstName = "Ratnesh";
                    dynamicObj.LastName = "Rai";
                    dynamicObj.Age = 36;
                    dynamicObj.Office = "WTLLP";
                    var s = str[i].ToString();
                    AddProperty(dynamicObj, s, i * 600);
                    //dynamicObj.str[i] = i / 600;
                    _lst.Add(dynamicObj);
                }
                return Ok(_lst);
            }
            catch (Exception ex)
            {
                return Ok(ex.InnerException);
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

    }
    public class ProductionSummaryModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Customer { get; set; }
        public string Line { get; set; }
        public string Style { get; set; }
        public string Product { get; set; }
        public string SO { get; set; }
        public string PO { get; set; }
        public string SMV { get; set; }
        public string Color { get; set; }

        public int PiecesChecked { get; set; }
        public int PlannedOutput { get; set; }
        public int ActualOutput { get; set; }
        public int PlannedSAH { get; set; }
        public int ActualSAH { get; set; }
        public double PlannedEff { get; set; }
        public double ActualEff { get; set; }
        public int PlannedManHours { get; set; }
        public int ActualManHours { get; set; }
    }
    public class EndlineQCSummaryModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Line { get; set; }
        public string QC { get; set; }
        public string ShiftTimings { get; set; }
        public string SO { get; set; }
        public string PO { get; set; }
        public string Style { get; set; }
        public int PiecesChecked { get; set; }
        public int DefectPieces { get; set; }
        public int ReworkedPieces { get; set; }
        public int RejectPieces { get; set; }
        public int ActualOutput { get; set; }
        public int ReworkRate { get; set; }
        public int RejectRate { get; set; }
    }
    public class TargetVsActualModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Customer { get; set; }
        public string Line { get; set; }
        public string Style { get; set; }
        public string Product { get; set; }
        public string SO { get; set; }
        public string PO { get; set; }
        public string SMV { get; set; }
        public int PiecesChecked { get; set; }
        public int PlannedOutput { get; set; }
        public int ActualOutput { get; set; }
        public int OutPutVariation { get; set; }
        public int PlannedSAH { get; set; }
        public int ActualSAH { get; set; }
        public int SAHVariation { get; set; }
        public double PlannedEff { get; set; }
        public double ActualEff { get; set; }
        public double EffVariation { get; set; }
        public int PlannedManHours { get; set; }
        public int ActualManHours { get; set; }
        public int HrsVariation { get; set; }
    }
    public class HourlyProduction
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Line { get; set; }
        public string QC { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public string SO { get; set; }
        public string PO { get; set; }
        public string Style { get; set; }
        public int PiecesChecked { get; set; }
        public int DefectPieces { get; set; }
        public int ReworkedPieces { get; set; }
        public int RejectPieces { get; set; }
        public int ActualOutput { get; set; }
        public int ReworkRate { get; set; }
        public int RejectRate { get; set; }
    }
    public class QualityPerfromanceModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Module { get; set; }
        public string ProcessName { get; set; }
        public string Line { get; set; }
        public string QC { get; set; }
        public string SO { get; set; }
        public string PO { get; set; }
        public string Style { get; set; }
        public int PiecesChecked { get; set; }
        public double DefectPieces { get; set; }
        public int ReworkedPieces { get; set; }
        public int RejectPieces { get; set; }
        public int ActualOutput { get; set; }
        public int ReworkRate { get; set; }
        public int RejectRate { get; set; }
        public double DHU { get; set; }
    }

}
