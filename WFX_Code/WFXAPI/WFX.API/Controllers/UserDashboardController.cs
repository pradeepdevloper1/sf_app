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
    public class UserDashboardController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<UserDashboardController> _logger;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public UserDashboardController(ILogger<UserDashboardController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("GetUserDashboard")]
        public ActionResult GetUserDashboard([FromBody] SearchModel _obj)
        {
            _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

            var lines = _context.tbl_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID).Select(x => x.LineName);
            double maxeff = 0;
            double mineff = 0;
            int qcgoodpcs = 0;
            int iterationno = 0;
            double TotalLinesEff = 0;
            int totaldelayedOrders = 0;
            List<DailyEff> maxefflst = new List<DailyEff>();
            List<DailyEff> minefflst = new List<DailyEff>();


            UserDashboardModel data = new UserDashboardModel();
            IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PlanStDt.Date < DateTime.Now.Date);
            if (!string.IsNullOrEmpty(_obj.ProcessCode))
                query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
            var orders = query.Select(x => new { PONo = x.PONo, SONo = x.SONo, PlanStDt = x.PlanStDt, Style = x.Style }).Distinct().OrderBy(x => x.PlanStDt).ToList();
           
            data.RunningOrders = _context.vw_OrderList.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.OrderStatus == 1).Distinct().Count();
            data.CompletedOrders = _context.vw_OrderList.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.OrderStatus == 2).Distinct().Count();
            foreach(var i in orders)
            {
                
                long podays = 0;
                long avrageprd = 0;
                IQueryable<tbl_LineTarget> q = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == i.PONo );
                if (q.Count() > 1)
                {
                    IQueryable<tbl_QCMaster> _qcmaster = _context.tbl_QCMaster.Where(x => x.PONo == i.PONo && x.FactoryID == _UserTokenInfo.FactoryID);
                    if (_qcmaster.Count() > 1)
                    {
                        var completedOrder = _qcmaster.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                        var PlannedQty = q.Sum(x => x.PlannedTarget);
                        if (q.Count() > 1)
                            avrageprd = completedOrder / q.Count();
                        if (avrageprd > 0)
                            podays = PlannedQty / avrageprd;
                        var PlanStartDate = q.Min(x => x.Date);
                        var PlanEndDate = q.Max(x => x.Date);
                        var expdt = PlanStartDate.AddDays(podays);
                        if (expdt.Date > PlanEndDate.Date)
                            totaldelayedOrders = totaldelayedOrders + 1;
                    }
                }


            }
            data.DelayedOrders = totaldelayedOrders;
            tbl_Factory _ofactory = _context.tbl_Factory.Where(x => x.FactoryID == _UserTokenInfo.FactoryID).FirstOrDefault();
            data.Compliance = _ofactory.SmartLines * 100 / _ofactory.NoOfLine;

            //int noofline_created = _context.tbl_Lines.Count();
            //int noofline_used = _context.tbl_LineTarget.Select(x => x.Line).Distinct().Count();
            //data.Compliance = noofline_used * 100 / noofline_created;

            data.PicesChecked = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.TypeOfWork == 1 && x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date).Sum(x => x.Qty);

            var GarmentsRejected = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCStatus == 3 && x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date).Sum(x => x.Qty);
            if (GarmentsRejected > 0)
                data.RejectionRate = GarmentsRejected * 100 / data.PicesChecked;
            else
                data.RejectionRate = 0;


            var TotalDefects = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCDefectDetailsId > 0 && x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date).Count();
            if (TotalDefects > 0)
                data.CurrentDHU = (TotalDefects * 100) / data.PicesChecked;
            else
                data.CurrentDHU = 0;

            var CountOfDefectsInReject = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCStatus == 3).Count();
            data.QualityBottlenecks = CountOfDefectsInReject;

            DateTime startdt = _obj.StartDate;
            DateTime senddt = _obj.EndDate;
            DailyEff _oDailyEff;
            //Eff Graph

            foreach (var line in lines)
            {
                startdt = _obj.StartDate;

                List<DailyEff> efflst = new List<DailyEff>();

                while (startdt <= senddt)
                {
                    _oDailyEff = new DailyEff();
                    _oDailyEff.QCDate = startdt;
                    _oDailyEff.QCDay = startdt.Day.ToString();
                    _oDailyEff.QCMonth = startdt.Month;
                    var pln = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == line && x.Date.Date == startdt.Date).ToList();
                    if (pln == null)
                    {
                        _oDailyEff.QCEff = 0;
                        _oDailyEff.QCQty = 0;
                    }
                    else
                    {
                        double actualeff = 0;
                        double Totalactualeff = 0;
                        foreach (var i in pln)
                        {
                            int planqty = i.PlannedTarget;
                            double target100per = i.PlannedTarget * 100 / i.PlannedEffeciency;
                           
                             qcgoodpcs = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == line && x.QCDate.Date == startdt.Date && x.QCStatus == 1 && x.Color == i.Color).Sum(x => x.Qty);
                            if (target100per > 0)
                                actualeff = (double)qcgoodpcs * (double)i.PlannedEffeciency / (double)planqty;
                            Totalactualeff = Totalactualeff + actualeff;
                        }
                        if(pln.Count()>0)
                        _oDailyEff.QCEff = Math.Ceiling((double)Totalactualeff /(double)pln.Count());
                        _oDailyEff.QCQty = qcgoodpcs;
                    }
                    efflst.Add(_oDailyEff);
                    startdt = startdt.AddDays(1);


                }

                double sumofeff = 0;
                double effofline = 0;
                foreach (var eff in efflst)
                {

                    sumofeff = sumofeff + eff.QCEff;

                }
                iterationno = iterationno + 1;
                if(efflst.Count()>0)
                effofline = Math.Ceiling(sumofeff / efflst.Count);
                TotalLinesEff = TotalLinesEff + effofline;
                if (effofline >= maxeff)
                {


                    maxeff =  effofline;
                    if (iterationno == 1)
                    {
                        mineff =  effofline;
                        minefflst = efflst;
                    }
                    maxefflst = efflst;
                }
                if (effofline < mineff)
                {
                    mineff =  effofline;
                    minefflst = efflst;

                }
            }
            data.LowestLlineEff = mineff;
            if(lines.Count()>0)
            data.AveragetLlineEff = Math.Ceiling(TotalLinesEff / lines.Count());
            if (data.AveragetLlineEff < 0)
                data.AveragetLlineEff = 0;
            data.HighestLlineEff = maxeff;
            if (data.HighestLlineEff < 0)
                data.HighestLlineEff = 0;
            if (data.LowestLlineEff < 0)
                data.LowestLlineEff = 0;

                //Defect Tally Graph
                List<TopDefect> topdefectlist = new List<TopDefect>();
            TopDefect _oTopDefect;
            IQueryable<vw_QC> top5defect_query = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCDate.Date >= _obj.StartDate.Date && x.QCDate.Date <= _obj.EndDate.Date);
            if (!string.IsNullOrEmpty(_obj.Module))
                top5defect_query = top5defect_query.Where(x => x.Module == _obj.Module);
            if (!string.IsNullOrEmpty(_obj.ProcessCode))
                top5defect_query = top5defect_query.Where(x => x.ProcessCode == _obj.ProcessCode);
            int totaldefect = top5defect_query.Where(x => x.QCDefectDetailsId > 0).Count();

            if (totaldefect > 0)
            {
                var defect_list = top5defect_query.Where(x => x.QCDefectDetailsId > 0).GroupBy(x => new { DefectName = x.DefectName })
                                       .Select(g => new
                                       {
                                           Count = g.Count(),
                                           DefectName = g.Key.DefectName,
                                           Per = g.Count() * 100 / totaldefect
                                       }).ToList().OrderByDescending(x => x.Count).Take(5);
                var total_of_5defectper = 0;
                foreach (var v in defect_list)
                {
                    _oTopDefect = new TopDefect();
                    _oTopDefect.Count = v.Count;
                    _oTopDefect.DefectName = v.DefectName;
                    _oTopDefect.Per = v.Per;
                    topdefectlist.Add(_oTopDefect);
                    total_of_5defectper += v.Per;
                }
                _oTopDefect = new TopDefect();
                _oTopDefect.Count = 0;
                _oTopDefect.DefectName = "Others";
                _oTopDefect.Per = 100 - total_of_5defectper;
                topdefectlist.Add(_oTopDefect);
            }
            else
            {
                _oTopDefect = new TopDefect();
                _oTopDefect.Count = 0;
                _oTopDefect.DefectName = "No Defects";
                _oTopDefect.Per = 100;
                topdefectlist.Add(_oTopDefect);
            }
            return Ok(new { status = 200, message = "Success", data, topdefectlist, maxefflst, minefflst });
        }

        public class DailyEff
        {
            public int QCQty { get; set; }
            public int PlanQty { get; set; }
            public double QCEff { get; set; }
            public DateTime QCDate { get; set; }
            public string QCDay { get; set; }
            public int QCMonth { get; set; }

        }
    }
}
