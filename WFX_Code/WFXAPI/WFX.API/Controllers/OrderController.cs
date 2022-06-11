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
using WFX.API.WebService;
using System.Threading.Tasks;
using WFX.API.Model;

namespace WFX.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {

        DBContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        private readonly IUserWebService _userWebService;

        public OrderController(IWebHostEnvironment env, ILogger<OrderController> logger, DBContext context, IConfiguration configuration, IUserWebService userWebService)
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
            return Ok("Order API Call");
        }

        [HttpPost]
        [Route("PostOrder")]
        public ActionResult PostOrder([FromBody] List<tbl_Orders> OrderList)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                string source = OrderList.FirstOrDefault().Source;
                if (source == "ERPApp")
                {
                    // to check blocking of revision if any transaction has been done
                    var recordOrder = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == OrderList.FirstOrDefault().PONo).ToList();
                    if (recordOrder != null && recordOrder.Count() > 0)
                    {
                        _context.tbl_Orders.RemoveRange(recordOrder);
                    }
                    OrderList.ForEach(x => x.FactoryID = _UserTokenInfo.FactoryID);
                }

                long orderid = 0;
                var lastorder = _context.tbl_Orders.OrderBy(x => x.OrderID).LastOrDefault();
                orderid = (lastorder == null ? 0 : lastorder.OrderID) + 1;
                foreach (tbl_Orders onerow in OrderList)
                {
                    if (source != "ERPApp")
                    {
                        onerow.WFXColorCode = "";
                        onerow.WFXColorName = "";
                    }
                    if (onerow.FulfillmentType == "Outsourced")
                    {
                        onerow.OrderStatus = 3;
                    }
                    onerow.OrderID = orderid;
                    onerow.EntryDate = System.DateTime.Now.Date;
                    onerow.LastSyncedAt = System.DateTime.Now;
                    orderid++;
                }
                _context.tbl_Orders.AddRange(OrderList);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
                //return Ok("Order Save");
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ERPOrderIntegration")]
        public ActionResult ERPOrderIntegration([FromBody] ERPOrderIntegration OrderList)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                PostOrder(OrderList.OrderData);
                var recordOrder = _context.tbl_OrderProcess.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == OrderList.PDData.FirstOrDefault().SONo).ToList();
                if (recordOrder != null && recordOrder.Count() > 0)
                {
                    _context.tbl_OrderProcess.RemoveRange(recordOrder);
                }
                PostProcessTemplate(OrderList.PDData);
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("PostProcessTemplate")]
        public ActionResult PostProcessTemplate([FromBody] List<tbl_OrderProcess> orderProcesses)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                long OrderProcessID = 0;
                var lastorder = _context.tbl_OrderProcess.OrderBy(x => x.OrderProcessID).LastOrDefault();
                OrderProcessID = (lastorder == null ? 0 : lastorder.OrderProcessID) + 1;
                foreach (tbl_OrderProcess onerow in orderProcesses)
                {
                    onerow.OrderProcessID = OrderProcessID;
                    onerow.FactoryID = _UserTokenInfo.FactoryID;
                    onerow.UserID = _UserTokenInfo.UserID;
                    onerow.ProcessEnabled = true;
                    onerow.CreatedOn = System.DateTime.Now;
                    OrderProcessID++;
                }
                _context.tbl_OrderProcess.AddRange(orderProcesses);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("OrderList")]
        public ActionResult OrderList([FromBody] tbl_Orders _obj)
        {
            List<vw_OrderList> data = new List<vw_OrderList>();
            var status = 200;
            var message = "";
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_OrderList> query = _context.vw_OrderList.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo.Contains(_obj.PONo));
                if (_obj.OrderStatus > -1)
                    query = query.Where(x => x.OrderStatus == _obj.OrderStatus);
                if (!string.IsNullOrEmpty(_obj.ProcessName))
                    query = query.Where(x => x.ProcessName == _obj.ProcessName);
                data = query.OrderByDescending(x => x.OrderID).ToList();
                if (data.Count > 0)
                {
                    foreach (vw_OrderList onerow in data)
                    {

                        onerow.CompletedQty = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == onerow.PONo && x.QCStatus == 1).Sum(x => x.Qty);
                        if (onerow.POQty > 0)
                        {
                            onerow.CompletedPer = onerow.CompletedQty * 100 / onerow.POQty;
                        }
                        else
                        {
                            onerow.CompletedPer = 0;
                        }

                    }
                    status = 200;
                    message = "Success";
                }
                else
                {
                    status = 204;
                    message = "No record found.";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //return BadRequest(ex.Message);
                return Ok(new { status = status, message = ex.Message });
            }
            return Ok(new {status,message, data });
        }

        [HttpPost]
        [Route("OrderListForUserDashboard")]
        public ActionResult OrderListForUserDashboard([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_OrderList> query = _context.vw_OrderList.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.Module))
                    query = query.Where(x => x.Module == _obj.Module);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                query = query.Where(x => x.EntryDate.Date >= _obj.StartDate.Date && x.EntryDate.Date <= _obj.EndDate.Date);
                var data = query.OrderByDescending(x => x.EntryDate).ToList();
                if (data.Count <= 0) { return Ok(new { status = 400, message = "No record found." }); }
                foreach (vw_OrderList onerow in data)
                {
                    if (onerow.OrderStatus == 1)
                        onerow.OrderRemark = "Running";
                    else if (onerow.OrderStatus == 2)
                        onerow.OrderRemark = "Completed";
                    else
                        onerow.OrderRemark = "NA";
                }
                { return Ok(new { status = 200, message = "Success", data }); }
            }
            catch (Exception ex)
            {

                return Ok(new { status = 400, message = ex.Message });
            }
        }
        //returns based on search criteria for SOView
        [HttpPost]
        [Route("SOView")]
        public ActionResult SOView([FromBody] vw_SOView _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                SOViewModel data = new SOViewModel();
                IQueryable<tbl_LineTarget> _linetarget = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID &&
                                                            x.SONo == _obj.SONo);

                IQueryable<vw_SOView> query = _context.vw_SOView;
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);

                var soresult = query.FirstOrDefault();
                if (soresult == null)
                    return Ok(new { status = 400, message = "No record found." });

                data.Product = soresult.Product;
                data.Fit = soresult.Fit;
                data.Style = soresult.Style;
                data.Fit = soresult.Fit;
                data.Season = soresult.Season;
                data.Customer = soresult.Customer;
                data.Module = soresult.Module;
                data.SONo = soresult.SONo;
                data.NoOfPO = soresult.NoOfPO;
                data.FactoryID = soresult.FactoryID;
                data.SOQty = soresult.SOQty;

                data.CompletedPO = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo && x.PrimaryPart == 1 && x.OrderStatus == 2).Select(x => new { PONo = x.PONo }).Distinct().Count();
                data.HoldPO = 0;
                data.DelayedPO = 0;
                data.OnTimePO = data.NoOfPO - data.DelayedPO - data.HoldPO;

                var polist = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo).Select(x => new { PONo = x.PONo }).Distinct().ToList();
                var socompletedqty = 0;
                var pocompletedqty = 0;
                var sodefectedqty = 0;
                var podefectedqty = 0;
                var sorejectedqty = 0;
                var porejectedqty = 0;
                
                foreach (var onerow in polist)
                {
                    var checkedpcs = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == onerow.PONo).Select(x => new { x.QCStatus, x.Qty, x.TypeOfWork }).ToList();
                    pocompletedqty = checkedpcs.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                    podefectedqty = checkedpcs.Where(x => x.QCStatus == 2 && x.TypeOfWork == 1).Sum(x => x.Qty);
                    porejectedqty = checkedpcs.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                    socompletedqty += pocompletedqty;
                    sodefectedqty += podefectedqty;
                    sorejectedqty += porejectedqty;

                    IQueryable<tbl_QCMaster> _qc = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == onerow.PONo);
                    var targetlist = _linetarget.Where(x => x.Date.Date <= DateTime.Now.Date && x.PONo == onerow.PONo).ToList();

                    double total_eff = 0;
                    int daycount = 0;
                    int total_bal = 0;

                    int day_plan_eff = 0;
                    double daytarget100per = 0;
                    double todayPlannedEfficiency = 0;
                    int day_target = 0;
                    int day_goodpcs = 0;
                    double day_eff = 0;
                    double day_actual_eff = 0;
                    int todayPassPcs = 0;
                    int pendingTarget = 0;
                    int todayPlannedTarget = 0;
                    int totalPlannedTarget = 0;
                    double PlannedEfficiency = 0;
                    double day_target_100per = 0;
                    int PassPcs = 0;

                    foreach (tbl_LineTarget oneday in targetlist)
                    {
                        day_target = oneday.PlannedTarget;
                        day_plan_eff = Convert.ToInt16(oneday.PlannedEffeciency);
                        day_target_100per = (double)day_target * 100 / (double)day_plan_eff;
                        day_goodpcs = _qc.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCStatus == 1 && x.QCDate.Date == oneday.Date.Date && x.Color == oneday.Color).Sum(x => x.Qty);
                        total_bal += day_target - day_goodpcs;
                        day_eff = (double)day_goodpcs * (double)day_plan_eff / (double)day_target;

                        total_eff += day_eff;

                        daycount++;
                    }

                    if (daycount > 0)
                        data.EfficiencyActualPer = Math.Ceiling((double)total_eff / (double)daycount);
                    else
                        data.EfficiencyActualPer = 0;
                    total_bal = soresult.SOQty - socompletedqty;
                    //data.EfficiencyRequiredPer = total_bal * 100 / data.EfficiencyActualPer;
                    if (total_bal > 0 && day_target_100per > 0)
                        data.EfficiencyRequiredPer = Math.Ceiling(total_bal * 100 / day_target_100per);
                    else
                        data.EfficiencyRequiredPer = 0;

                    IQueryable<vw_QC> _viewqc = _context.vw_QC.Where(x => x.PONo == onerow.PONo);

                    data.PicesChecked = _qc.Where(x => x.TypeOfWork == 1).Sum(x => x.Qty);
                    var GarmentsRejected = _qc.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                    if (data.PicesChecked > 0)
                        data.RejectionRate = GarmentsRejected * 100 / data.PicesChecked;
                    else
                        data.RejectionRate = 0;

                    var QCMasterIds = _qc.Select(x => x.QCMasterId).ToList();
                    var TotalDefects = _viewqc.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCDefectDetailsId > 0 && QCMasterIds.Contains(x.QCMasterId)).Count();
                    if (data.PicesChecked > 0)
                        data.CurrentDHU = (TotalDefects * 100) / data.PicesChecked;
                    else
                        data.CurrentDHU = 0;
                }

                data.CompletedQty = socompletedqty;
                data.DefectedQty = sodefectedqty;
                data.RejectedQty = sorejectedqty;
                if (socompletedqty > 0)
                    data.CompletedPer = data.CompletedQty * 100 / data.SOQty;
                else
                    data.CompletedPer = 0;

                if (_linetarget.ToList().Count > 0)
                {
                    data.PlannedSAH = Math.Ceiling(_linetarget.Average(x => x.PlannedTarget * x.SMV / 60));
                    data.ActualSAH = Math.Ceiling(data.CompletedQty * _linetarget.FirstOrDefault().SMV / 60);
                    data.EfficiencyPlanedPer = Math.Ceiling(_linetarget.Average(x => x.PlannedEffeciency));
                }
                else
                {
                    data.PlannedSAH = 0;
                    data.ActualSAH = 0;
                    data.EfficiencyPlanedPer = 0;
                }
                //PO Images
                List<POImage> imagelist = new List<POImage>();
                var imglist = _context.tbl_POImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == polist.FirstOrDefault().PONo).ToList();
                POImage _poimage = new POImage();
                foreach (var v in imglist)
                {
                    _poimage = new POImage();
                    _poimage.ImagePath = v.ImagePath;
                    _poimage.ImageName = v.ImageName;
                    _poimage.PONo = v.PONo;
                    imagelist.Add(_poimage);
                }
                return Ok(new { status = 200, message = "Success", data, imagelist });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("POColorSizeList")]
        public ActionResult POColorSizeList([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);

                var data = query.ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                List<color> _lstcolor = new List<color>();
                List<size> _lstsize = new List<size>();
                color _onecolor;
                size _onesize;
                Dictionary<string, string> allSizeCollection = new Dictionary<string, string>();
                Dictionary<string, int> zeroSizeCollection = new Dictionary<string, int>();
                int TotalOrderQty = 0;
                int TotalPlannedQty = 0;
                int TotalYetToPlanQty = 0;
                int TotalCompletedQty = 0;
                int index = 0;
                foreach (tbl_Orders orderData in data)
                {
                    index = index + 1;
                    if (orderData.SizeList != "")
                    {
                        string[] sizelist = orderData.SizeList.Split(',');
                        //Array.Sort(sizelist);
                        foreach (string s in sizelist)
                        {
                            string size = s.Split('-')[0];

                            int qty = Convert.ToInt32(s.Split('-')[1]);
                            if (!allSizeCollection.ContainsKey(size))
                                allSizeCollection[size] = "";

                            if (!zeroSizeCollection.ContainsKey(size) && qty == 0 && index == 1)
                                zeroSizeCollection[size] = 0;
                            else if (zeroSizeCollection.ContainsKey(size) && qty != 0)
                                zeroSizeCollection.Remove(size);

                        }
                    }
                }

                foreach (string size in zeroSizeCollection.Keys)
                {
                    if (allSizeCollection.ContainsKey(size))
                        allSizeCollection.Remove(size);
                }


                foreach (tbl_Orders _onecolorpo in data)
                {

                    _onecolor = new color();
                    _onecolor.colorname = _onecolorpo.Color;
                    string[] sizelist = _onecolorpo.SizeList.Split(',');
                    _lstsize = new List<size>();
                    int ColorTotalOrderQty = 0;
                    int ColorTotalPlannedQty = 0;
                    int ColorTotalYetToPlanQty = 0;
                    int ColorTotalCompletedQty = 0;
                    int ColorTotalBalanceQty = 0;
                    int ColorTotalComplPer = 0;
                    foreach (string s in allSizeCollection.Keys)
                    {
                        _onesize = new size();
                        _onesize.OrderSizeName = s;
                        string sizeDetail = sizelist.Where(x => x.Split('-')[0] == s).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sizeDetail))
                        {
                            _onesize.OrderQty = Convert.ToInt16(sizeDetail.Split('-').Last());
                            if (_onesize.OrderQty > 0)
                            {
                                IQueryable<tbl_LineTarget> targetquery = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo && x.Color == _onecolorpo.Color);
                                var targetresult = targetquery.ToList();
                                bool h=true;
                                foreach (tbl_LineTarget target in targetquery)
                                    {
                                        string[] targetsizelist = target.SizeList.Split(',');
                                  
                                        if (targetsizelist[0] != "" && targetsizelist.Count() > 0)
                                        {
                                            foreach (string targetsize in targetsizelist)
                                            {
                                            h = true;
                                                string plansizename = targetsize.Split('-').First();
                                                int planqty = Convert.ToInt16(targetsize.Split('-').Last());
                                                if (plansizename == _onesize.OrderSizeName)
                                                    _onesize.PlannedQty += planqty;
                                            }
                                        }
                                        else
                                        {
                                        h = false;
                                            _onesize.PlannedQty = 0;
                                            _onesize.YetToPlanQty = 0;

                                        }
                                    }
                                    if(h == true)
                                     _onesize.YetToPlanQty = _onesize.OrderQty - _onesize.PlannedQty;              
                                _onesize.CompletedQty = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _onecolorpo.PONo && x.Color == _onecolorpo.Color && x.Size == _onesize.OrderSizeName && x.QCStatus == 1).Sum(x => x.Qty);
                                _onesize.BalanceQty = _onesize.OrderQty - _onesize.CompletedQty;
                                _onesize.ComplPer = (_onesize.CompletedQty * 100) / _onesize.OrderQty;

                                TotalOrderQty += _onesize.OrderQty;
                                TotalPlannedQty += _onesize.PlannedQty;
                                TotalYetToPlanQty += _onesize.YetToPlanQty;
                                TotalCompletedQty += _onesize.CompletedQty;
                            }
                            ColorTotalOrderQty += _onesize.OrderQty;
                            ColorTotalPlannedQty += _onesize.PlannedQty;
                            ColorTotalYetToPlanQty += _onesize.YetToPlanQty;
                            ColorTotalCompletedQty += _onesize.CompletedQty;
                            ColorTotalBalanceQty += _onesize.BalanceQty;
                            if (ColorTotalOrderQty > 0)
                                ColorTotalComplPer = ColorTotalCompletedQty * 100 / ColorTotalOrderQty;
                            else
                                ColorTotalComplPer = 0;
                        }
                        else
                        {
                            _onesize.OrderQty = 0;
                        }
                        _lstsize.Add(_onesize);
                    }
                    _onesize = new size();
                    _onesize.OrderSizeName = "Total";
                    _onesize.OrderQty = ColorTotalOrderQty;
                    _onesize.PlannedQty = ColorTotalPlannedQty;
                    _onesize.YetToPlanQty = ColorTotalYetToPlanQty;
                    _onesize.CompletedQty = ColorTotalCompletedQty;
                    _onesize.BalanceQty = ColorTotalBalanceQty;
                    _onesize.ComplPer = ColorTotalComplPer;
                    _lstsize.Add(_onesize);

                    _onecolor.sizelist = _lstsize;
                    _lstcolor.Add(_onecolor);
                }
                return Ok(new { status = 200, message = "Success", _lstcolor });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //Returns list of tbl_Orders with main part only part =1
        //based on SO search criteria
        //For Mobile App
        [HttpPost]
        [Route("SOList")]
        public ActionResult SOList([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_SOView> query = _context.vw_SOView.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.OrderStatus != 2);

                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);
                if (!(_obj.FactoryID == 0))
                    query = query.Where(x => x.FactoryID == _obj.FactoryID);


                var data = query.ToList();
                if (data == null)
                    return Ok(new { status = 401, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

     
        //Returns list of tbl_Orders with main part only part =1
        //based on PO search criteria
        //For Mobile App
        [HttpPost]
        [Route("POList")]
        public ActionResult POList([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var POList = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line).Select(x => x.PONo).ToList();

                IQueryable<vw_POList> poQuery = _context.vw_POList.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && POList.Contains(x.PONo) && x.OrderStatus != 2);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    poQuery = poQuery.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    poQuery = poQuery.Where(x => x.SONo == _obj.SONo);

                IQueryable<vw_POList> query =
                      from p in poQuery.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && POList.Contains(x.PONo))
                      join s in _context.tbl_POImages
                        on new { p.FactoryID, p.SONo } equals new { s.FactoryID, s.SONo }
                      join t in _context.tbl_OB
                        on new
                        {
                            p.FactoryID,
                            p.PONo
                        } equals new
                        {
                            t.FactoryID,
                            t.PONo
                        }
                      group p by new
                      {
                          OrderID = p.OrderID,
                          Module = p.Module,
                          SONo = p.SONo,
                          PONo = p.PONo,
                          Style = p.Style,
                          Fit = p.Fit,
                          Product = p.Product,
                          Season = p.Season,
                          Customer = p.Customer,
                          PlanStDt = p.PlanStDt,
                          ExFactory = p.ExFactory,
                          PrimaryPart = p.PrimaryPart,
                          Part = p.Part,
                          Color = p.Color,
                          Hexcode = p.Hexcode,
                          Fabric = p.Fabric,
                          SizeList = p.SizeList,
                          Line = _obj.Line,
                          OrderRemark = p.OrderRemark,
                          IsSizeRun = p.IsSizeRun,
                          POQty = p.POQty,
                          OrderStatus = p.OrderStatus,
                          UserID = p.UserID,
                          FactoryID = p.FactoryID,
                          EntryDate = p.EntryDate,
                          WFXColorCode = p.WFXColorCode,
                          WFXColorName = p.WFXColorName,
                          ProcessCode = p.ProcessCode,
                          ProcessName = p.ProcessName,
                          FulfillmentType = p.FulfillmentType
                      } into g
                      select new vw_POList
                      {
                          OrderID = g.Key.OrderID,
                          Module = g.Key.Module,
                          SONo = g.Key.SONo,
                          PONo = g.Key.PONo,
                          Style = g.Key.Style,
                          Fit = g.Key.Fit,
                          Product = g.Key.Product,
                          Season = g.Key.Season,
                          Customer = g.Key.Customer,
                          PlanStDt = g.Key.PlanStDt,
                          ExFactory = g.Key.ExFactory,
                          PrimaryPart = g.Key.PrimaryPart,
                          Part = g.Key.Part,
                          Color = g.Key.Color,
                          Hexcode = g.Key.Hexcode,
                          Fabric = g.Key.Fabric,
                          SizeList = g.Key.SizeList,
                          Line = g.Key.Line,
                          OrderRemark = g.Key.OrderRemark,
                          IsSizeRun = g.Key.IsSizeRun,
                          POQty = g.Key.POQty,
                          OrderStatus = g.Key.OrderStatus,
                          UserID = g.Key.UserID,
                          FactoryID = g.Key.FactoryID,
                          EntryDate = g.Key.EntryDate,
                          WFXColorCode = g.Key.WFXColorCode,
                          WFXColorName = g.Key.WFXColorName,
                          ProcessCode = g.Key.ProcessCode,
                          ProcessName = g.Key.ProcessName,
                          FulfillmentType = g.Key.FulfillmentType
                          //Source = g.Key.Source
                      };
                //.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && POList.Contains(x.PONo))
                var data = query.ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 401, message = "No record found." });

                foreach (var order in data)
                {

                    string[] sizelist = order.SizeList.Split(',');
                    string SizeDetails = "";
                    foreach (string s in sizelist)
                    {
                        var sizename = s.Split('-').First();
                        var sizeqty = Convert.ToInt16(s.Split('-').Last());
                        if (sizeqty > 0)
                            SizeDetails += sizename + "-" + sizeqty + ",";
                    }

                    if (SizeDetails != "")
                        SizeDetails = SizeDetails.Remove(SizeDetails.Length - 1);
                    order.SizeList = SizeDetails;
                }

                IQueryable<tbl_POImages> image_query = _context.tbl_POImages;
                image_query = image_query.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && data.Select(x => x.PONo).Contains(x.PONo));
                var imagelist = image_query.ToList();

                IQueryable<tbl_POShiftImages> shiftimage_query = _context.tbl_POShiftImages;
                shiftimage_query = shiftimage_query.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && data.Select(x => x.PONo).Contains(x.PONo));
                var shiftimagelist = shiftimage_query.ToList();

                IQueryable<tbl_QCMaster> qcmaster_query = _context.tbl_QCMaster;
                qcmaster_query = qcmaster_query.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.TabletID == _obj.TabletID
                                                      && data.Select(x => x.SONo).Contains(x.SONo)
                                                       && data.Select(x => x.PONo).Contains(x.PONo));
                var qcmasterlist = qcmaster_query.ToList();


                return Ok(new { status = 200, message = "Success", data, imagelist, shiftimagelist, qcmasterlist });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //For Mobile App
        [HttpPost]
        [Route("POForMApp")]
        public ActionResult POForMApp([FromBody] tbl_Orders _obj)
        {
            var status = 200;
            var message = "Success";
            List<POModelForMApp> data = new List<POModelForMApp>();
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_POForMApp> query = _context.vw_POForMApp.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);

                var result = query.ToList();
                if (result.Count <= 0)
                {
                    status = 204;
                    message = "No record found.";
                }
                foreach (vw_POForMApp order in result)
                {
                    POModelForMApp onepo = new POModelForMApp();
                    onepo.Module = order.Module;
                    onepo.SONo = order.SONo;
                    onepo.PONo = order.PONo;
                    onepo.Style = order.Style;
                    onepo.Fit = order.Fit;
                    onepo.Product = order.Product;
                    onepo.Season = order.Season;
                    onepo.Customer = order.Customer;
                    onepo.PlanStDt = order.PlanStDt;
                    onepo.ExFactory = order.ExFactory;
                    onepo.PrimaryPart = order.PrimaryPart;
                    onepo.Hexcode = order.Hexcode;
                    onepo.Fabric = order.Fabric;
                    onepo.POQty = order.POQty;
                    onepo.OrderStatus = order.OrderStatus;
                    onepo.UserID = order.UserID;
                    onepo.FactoryID = order.FactoryID;
                    onepo.EntryDate = order.EntryDate;
                    onepo.Line = "NA";
                    var plan = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == order.PONo).FirstOrDefault();
                    if (plan != null)
                    {
                        onepo.Line = plan.Line;
                    }

                    // color list block

                    List<string> colorlist = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && x.PONo == order.PONo).Select(x => x.Color).ToList();
                    onepo.colorlist = colorlist;

                    //List<POColorList> colorlist = new List<POColorList>();
                    //POColorList _onecolor = new POColorList();
                    //var color = _context.tbl_Orders.Where(x => x.PrimaryPart == 1 && x.PONo == order.PONo).Select(x => new { PONo = x.PONo, Color = x.Color }).ToList();
                    //foreach (var v in color)
                    //{
                    //    _onecolor = new POColorList();
                    //    //_onecolor.PONo = v.PONo;
                    //    _onecolor.Color = v.Color;
                    //    colorlist.Add(_onecolor);
                    //}
                    //onepo.colorlist = colorlist;


                    // image list block
                    var imagelist = _context.tbl_POImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == order.PONo).ToList();
                    onepo.imagelist = imagelist;

                    // shiftimage list block
                    var shiftimagelist = _context.tbl_POShiftImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == order.PONo).ToList();
                    onepo.shiftimagelist = shiftimagelist;

                    data.Add(onepo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(new { status = status, message = message, data });
        }

        //Returns list of tbl_Orders (all part 0 and 1)
        //based on PO search criteria
        [HttpPost]
        [Route("POPartsList")]
        public ActionResult POPartsList([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                var data = query.Select(x => new { Part = x.Part, PONo = x.PONo, PrimaryPart = x.PrimaryPart }).Distinct().OrderByDescending(x => x.PrimaryPart).ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [Route("PostIsPOExist")]
        [HttpPost]
        public ActionResult PostIsPOExist([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

                var data = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).Select(x => new { x.OrderStatus, x.PONo }).FirstOrDefault();

                if (data == null)
                {
                    return Ok(new { status = 401, message = "Not Found" });
                }
                else if (data.OrderStatus == 2 && _obj.FromPage == "LineTarget")
                {
                    return Ok(new { status = 402, message = "Order Already Completed" });
                }
                else
                {
                    return Ok(new { status = 200, message = "Found" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [Route("PostIsShiftExist")]
        [HttpPost]
        public ActionResult PostIsShiftExist([FromBody] tbl_Shift _obj)
        {
            try
            {
                var data = _context.tbl_Shift.Where(x => x.FactoryID == _obj.FactoryID && x.ShiftName == _obj.ShiftName).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [Route("PostIsSOExist")]
        [HttpPost]
        public ActionResult PostIsSOExist([FromBody] tbl_Orders _obj)
        {
            try
            {
                var data = _context.tbl_Orders.Where(x => x.FactoryID == _obj.FactoryID && x.SONo == _obj.SONo).FirstOrDefault();
                if (data == null)
                {
                    return Ok(new { status = 401, message = "Not Found" });
                }
                else
                {
                    return Ok(new { status = 200, message = "Found", data = data });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [Route("PostIsStyleExist")]
        [HttpPost]
        public ActionResult PostIsStyleExist([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo && x.SONo == _obj.SONo && x.Style == _obj.Style).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostIsSizeExist")]
        [HttpPost]
        public ActionResult PostIsSizeExist([FromBody] tbl_Orders _obj)
        {
            try
            {
                var data = _context.tbl_Orders.Where(x => x.FactoryID == _obj.FactoryID && x.PONo == _obj.PONo && x.Color == _obj.Color).Select(x => x.SizeList).FirstOrDefault();
                if (data == null)
                {
                    return Ok(new { status = 400, message = "Not Found" });
                }
                else
                {
                    string[] sizelist = data.Split(',');
                    List<string> sizeNotExists = new List<string>();
                    string sizeList = "";
                    int recFound = 0;
                    foreach (string sizerec in _obj.SizeList.Split(','))
                    {
                        recFound = 0;
                        foreach (string s in sizelist)
                        {
                            string size = s.Split('-')[0];
                            int sizeqty = Convert.ToInt32(s.Split('-')[1]);
                            if (sizerec == size && sizeqty > 0)
                            {
                                recFound = 1;
                                break;
                            }
                        }
                        if (recFound == 0)
                            sizeNotExists.Add(sizerec);
                    }

                    foreach (string sizes in sizeNotExists)
                    {
                        sizeList += sizes + ',';
                    }

                    if (sizeList != "")
                    {
                        sizeList = sizeList.Remove(sizeList.Length - 1);
                        return Ok(new { status = 401, message = "Not Found", sizes = sizeList });

                    }
                    else
                    {
                        return Ok(new { status = 200, message = "Found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostIsColorExist")]
        [HttpPost]
        public ActionResult PostIsColorExist([FromBody] tbl_Orders _obj)
        {
            try
            {
                var data = _context.tbl_Orders.Where(x => x.FactoryID == _obj.FactoryID && x.PONo == _obj.PONo && x.Color == _obj.Color).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPOListOfSO")]
        public ActionResult GetPOListOfSO(string SONo)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && x.SONo == SONo);
                var data = query.Select(x => new { Id = x.PONo, Value = x.PONo }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("POListOfSO")]
        public ActionResult POListOfSO([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                var sono = _obj.SONo;
                if (!string.IsNullOrEmpty(_obj.PONo))
                {
                    sono = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1 && x.PONo == _obj.PONo).Select(x => x.SONo).FirstOrDefault();
                    query = query.Where(x => x.SONo == sono);
                }
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                if (!string.IsNullOrEmpty(_obj.POSearchText))

                    query = query.Where(x => x.PONo.Contains(_obj.POSearchText));


                //var data = query.Select(x => new { Id = x.PONo, Value = x.PONo, OrderID = x.OrderID }).Distinct().OrderByDescending(x => x.OrderID).ToList();
                var data = query.Select(x => new { Id = x.PONo, Value = x.PONo, EntryDate = x.EntryDate, ProcessCode = x.ProcessCode, ProcessName = x.ProcessName }).Distinct().OrderByDescending(x => x.EntryDate).ToList();

                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data, sono });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetOrderStatusList")]
        public ActionResult GetOrderStatusList()
        {
            try
            {
                IQueryable<tbl_OrderStatus> query = _context.tbl_OrderStatus;
                var data = query.Select(x => new { Id = x.OrdrStatusID, Text = x.OrderStatusName }).ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeletePO")]
        public ActionResult DeletePO([FromBody] tbl_Orders _obj)
        {
            var status = 200;
            var message = "";
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                List<tbl_Orders> _po = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                List<tbl_POImages> _poimage = _context.tbl_POImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                List<tbl_OB> _ob = _context.tbl_OB.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();

                List<tbl_LineTarget> _target = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                List<tbl_QCMaster> _qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();
                List<tbl_POShiftImages> _poshiftimage = _context.tbl_POShiftImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo).ToList();


                if (_po.Count <= 0)
                {
                    status = 204;
                    message = "No record found.";
                }
                else
                {
                    _context.tbl_Orders.RemoveRange(_po);
                    if (_poimage.Count > 0)
                    {
                        _context.tbl_POImages.RemoveRange(_poimage);
                    }
                    if (_ob.Count > 0)
                    {
                        _context.tbl_OB.RemoveRange(_ob);
                    }

                    if (_target.Count > 0)
                    {
                        _context.tbl_LineTarget.RemoveRange(_target);
                    }
                    if (_qcmaster.Count > 0)
                    {
                        var qcidlist = _qcmaster.Select(x => x.QCMasterId).ToList();
                        List<tbl_QCDefectDetails> _qcdefects = _context.tbl_QCDefectDetails.Where(x => qcidlist.Contains(x.QCMasterId)).ToList();
                        if (_qcdefects.Count <= 0)
                        {
                            _context.tbl_QCDefectDetails.RemoveRange(_qcdefects);
                        }
                        _context.tbl_QCMaster.RemoveRange(_qcmaster);
                    }
                    if (_poshiftimage.Count > 0)
                    {
                        _context.tbl_POShiftImages.RemoveRange(_poshiftimage);
                    }

                    _context.SaveChanges();
                    message = "Delete Sucessfully";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //return BadRequest(ex.Message);
                return Ok(new { status = status, message = ex.Message });
            }
            return Ok(new { status = status, message = message });
        }

        [HttpPost]
        [Route("CompletePO")]
        public ActionResult CompletePO([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo);
                var data = query.ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });
                foreach (tbl_Orders onerow in data)
                {
                    onerow.OrderStatus = 2; // complete
                }
                _context.tbl_Orders.UpdateRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //Mobile App API
        [HttpPost]
        [Route("PostOrderPlay")]
        public ActionResult PostOrderPlay([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo);
                var data = query.ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });
                foreach (tbl_Orders onerow in data)
                {
                    onerow.OrderStatus = 1; // Running
                }
                _context.tbl_Orders.UpdateRange(data);
                _context.SaveChanges();
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPOLineList")]
        public ActionResult GetPOLineList(string PONo)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_LineTarget> query = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == PONo);
                var data = query.Select(x => new { Id = x.Line, Text = x.Line }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPOModuleList")]
        public ActionResult GetPOModuleList(string PONo)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_LineTarget> query = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == PONo);
                var data = query.Select(x => new { Id = x.Module, Text = x.Module }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("FillRunningPO")]
        public ActionResult FillRunningPO()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                var data = query.Select(x => new { Id = x.PONo, Text = x.PONo }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("FillSO")]
        public ActionResult FillSO()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.SONo, Text = x.SONo }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("FillSO")]
        public ActionResult FillSO(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo != null && x.SONo != "");
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
                    dataQuery = (IQueryable<tbl_Orders>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          SONo = p.SONo
                      }
                      into g
                      select new tbl_Orders
                      {
                          SONo = g.Key.SONo,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Orders>)
                        query.
                        GroupBy(x => new
                        {
                            SONo = x.SONo
                        }).
                        Select(g => new tbl_Orders
                        {
                            SONo = g.Key.SONo
                        });
                }

                data = dataQuery.Select(x => new { ID = x.SONo, Text = x.SONo }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("FillPO")]
        public ActionResult FillPO()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.PONo, Text = x.PONo }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("FillPO")]
        public ActionResult FillPO(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo != null && x.PONo != "");
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
                    dataQuery = (IQueryable<tbl_Orders>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          PONo = p.PONo
                      }
                      into g
                      select new tbl_Orders
                      {
                          PONo = g.Key.PONo,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Orders>)
                        query.
                        GroupBy(x => new
                        {
                            PONo = x.PONo
                        }).
                        Select(g => new tbl_Orders
                        {
                            PONo = g.Key.PONo
                        });
                }

                data = dataQuery.Select(x => new { ID = x.PONo, Text = x.PONo }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("FillSeason")]
        public ActionResult FillSeason()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.Season, Text = x.Season }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("FactoryProcessList")]
        public ActionResult FactoryProcessList()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<vw_ProcessDefinition> query = _context.vw_ProcessDefinition.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.ProcessCode, Text = x.ProcessName }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("FillSeason")]
        public ActionResult FillSeason(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Season != null && x.Season != "");
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
                    dataQuery = (IQueryable<tbl_Orders>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Season = p.Season
                      }
                      into g
                      select new tbl_Orders
                      {
                          Season = g.Key.Season,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Orders>)
                        query.
                        GroupBy(x => new
                        {
                            Season = x.Season
                        }).
                        Select(g => new tbl_Orders
                        {
                            Season = g.Key.Season
                        });
                }

                data = dataQuery.Select(x => new { ID = x.Season, Text = x.Season }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("FillStyle")]
        public ActionResult FillStyle()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.Select(x => new { Id = x.Style, Text = x.Style }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("FillStyle")]
        public ActionResult FillStyle(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Style != null && x.Style != "");
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
                    dataQuery = (IQueryable<tbl_Orders>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          Style = p.Style
                      }
                      into g
                      select new tbl_Orders
                      {
                          Style = g.Key.Style,
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_Orders>)
                        query.
                        GroupBy(x => new
                        {
                            Style = x.Style
                        }).
                        Select(g => new tbl_Orders
                        {
                            Style = g.Key.Style
                        });
                }

                data = dataQuery.Select(x => new { ID = x.Style, Text = x.Style }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPOImages")]
        public ActionResult GetPOImages(string SONo)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_POImages> query = _context.tbl_POImages.Where(x => x.SONo == SONo && x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("PostPOImages")]
        public ActionResult PostPOImages([FromBody] tbl_POImages _obj)

        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

                long id = 0;
                var lastrecord = _context.tbl_POImages.OrderBy(x => x.POImageID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.POImageID) + 1;

                _obj.POImageID = id;
                _obj.EntryDate = DateTime.Now.Date;
                _obj.UserID = _UserTokenInfo.UserID;
                _obj.FactoryID = _UserTokenInfo.FactoryID;
                _context.tbl_POImages.Add(_obj);
                _context.SaveChanges();

                IQueryable<tbl_POImages> query = _context.tbl_POImages.Where(x => x.SONo == _obj.SONo && x.FactoryID == _UserTokenInfo.FactoryID);
                var data = query.ToList();

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
                //return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PutPOImages")]
        public ActionResult PutPOImages([FromBody] tbl_POImages _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                long id = 0;
                var lastrecord = _context.tbl_POImages.Where(x => x.POImageID == _obj.POImageID).FirstOrDefault();
                if (lastrecord != null)
                {
                    lastrecord.ImagePath = _obj.ImagePath;
                    lastrecord.ImageName = _obj.ImageName;
                    _context.tbl_POImages.Update(lastrecord);
                    _context.SaveChanges();

                    IQueryable<tbl_POImages> query = _context.tbl_POImages.Where(x => x.SONo == _obj.SONo && x.FactoryID == _UserTokenInfo.FactoryID);
                    var data = query.ToList();

                    return Ok(new { status = 200, message = "Success", data });
                }
                return Ok(new { status = 400, message = "No record found." });

            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("DeletePOImage")]
        public ActionResult DeletePOImage([FromBody] tbl_POImages _obj)
        {
            var status = 200;
            var message = "";
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                tbl_POImages _poimage = _context.tbl_POImages.Where(x => x.POImageID == _obj.POImageID).FirstOrDefault();
                if (_poimage == null)
                {
                    status = 204;
                    message = "No record found.";
                }
                else
                {
                    _context.tbl_POImages.Remove(_poimage);
                    _context.SaveChanges();
                    IQueryable<tbl_POImages> query = _context.tbl_POImages.Where(x => x.SONo == _obj.SONo && x.FactoryID == _UserTokenInfo.FactoryID);
                    var data = query.ToList();
                    message = "Delete Sucessfully";
                    return Ok(new { status = status, message = message, data });
                }
            }
            catch (Exception ex)
            {
                status = 400;
                message = ex.Message;
                //Console.WriteLine(ex.Message);
                return Ok(new { status = status, message = ex.Message });
            }
            return Ok(new { status = status, message = message });
        }

        [HttpPost]
        [Route("POView")]
        public ActionResult POView([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                POView data = new POView();
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    query = query.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    query = query.Where(x => x.ProcessCode == _obj.ProcessCode);
                var result = query.FirstOrDefault();

                if (result == null)
                    return Ok(new { status = 400, message = "No record found." });


                data.Product = result.Product;
                data.Style = result.Style;
                data.Fit = result.Fit;
                data.Season = result.Season;
                data.Customer = result.Customer;

                data.Module = result.Module;
                data.SONo = result.SONo;
                data.PONo = result.PONo;
                data.FactoryID = result.FactoryID;

                data.ExFactoryDate = result.ExFactory;
                data.RecvDate = result.EntryDate;
                data.POQty = query.Sum(x => x.POQty);
                data.OrderStatus = result.OrderStatus;
                data.ProcessCode = result.ProcessCode;
                data.ProcessName = result.ProcessName;

                IQueryable<tbl_LineTarget> _linetarget = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    _linetarget = _linetarget.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    _linetarget = _linetarget.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    _linetarget = _linetarget.Where(x => x.ProcessCode == _obj.ProcessCode);

                IQueryable<tbl_QCMaster> _qcmaster = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    _qcmaster = _qcmaster.Where(x => x.SONo == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    _qcmaster = _qcmaster.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    _qcmaster = _qcmaster.Where(x => x.ProcessCode == _obj.ProcessCode);

                IQueryable<vw_QC> _viewqc = _context.vw_QC.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                if (!string.IsNullOrEmpty(_obj.SONo))
                    _viewqc = _viewqc.Where(x => x.SONO == _obj.SONo);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    _viewqc = _viewqc.Where(x => x.PONo == _obj.PONo);
                if (!string.IsNullOrEmpty(_obj.ProcessCode))
                    _viewqc = _viewqc.Where(x => x.ProcessCode == _obj.ProcessCode);

                List<processDetail> processListt = new List<processDetail>();
               
                    var processes =
                    _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo).Select(x => new { x.ProcessCode, x.ProcessName, x.PONo });
                    if (!string.IsNullOrEmpty(_obj.PONo))
                        processes = processes.Where(x => x.PONo == _obj.PONo);
                    if (!string.IsNullOrEmpty(_obj.ProcessCode))
                        processes = processes.Where(x => x.ProcessCode == _obj.ProcessCode);
                    var processList = processes.ToList();
                    var ProcessesList = processList.GroupBy(x => new { ProcessCode = x.ProcessCode, ProcessName = x.ProcessName }).Where(x => x.Key.ProcessCode != null).Select(g => new
                    {
                        ProcessCode = g.Key.ProcessCode,
                        ProcessName = g.Key.ProcessName
                    }).ToList();
                if (string.IsNullOrEmpty(_obj.PONo) && string.IsNullOrEmpty(_obj.ProcessCode))
                {
                    foreach (var i in ProcessesList)
                    {
                        processDetail y = new processDetail();
                        y.ProcessName = i.ProcessName;
                        y.ProcessCode = i.ProcessCode;
                        y.PassPcs = _qcmaster.Where(x => x.QCStatus == 1 && x.ProcessCode == i.ProcessCode).Sum(x => x.Qty);
                        y.DefectPcs = _qcmaster.Where(x => x.QCStatus == 2 && x.TypeOfWork == 1 && x.ProcessCode == i.ProcessCode).Sum(x => x.Qty)
                                           - _qcmaster.Where(x => x.TypeOfWork == 2 && x.QCStatus != 2 && x.ProcessCode == i.ProcessCode).Sum(x => x.Qty);
                        y.RejectPcs = _qcmaster.Where(x => x.QCStatus == 3 && x.ProcessCode == i.ProcessCode).Sum(x => x.Qty);
                        var j = _linetarget.Where(x => x.ProcessCode == i.ProcessCode && x.Date.Date <= DateTime.Now.Date).ToList();
                        if (j.Count() > 0)
                        {
                            y.PlannedSAH = Math.Ceiling(j.Average(x => x.PlannedTarget) * j.Average(x => x.SMV) / 60);
                            y.ActualSAH = Math.Ceiling(y.PassPcs * j.Average(x => x.SMV) / 60);
                            double day_eff = 0;
                            double total_eff = 0;
                            foreach (var o in j)
                            {
                                day_eff = Math.Ceiling(_qcmaster.Where(x => x.QCStatus == 1 && x.ProcessCode == i.ProcessCode && x.QCDate.Date == o.Date.Date).Sum(x => x.Qty) * o.PlannedEffeciency / o.PlannedTarget);
                                total_eff += day_eff;
                            }
                            y.ActualEfficiency = total_eff / j.Count();
                        }
                        else
                        {
                            y.PlannedSAH = 0;
                            y.ActualSAH = 0;
                        }
                        processListt.Add(y);
                    }
                   
                }
                else
                {
                    if (_linetarget.ToList().Count > 0)
                    {
                        data.PlannedQty = _linetarget.Sum(x => x.PlannedTarget);
                        data.PlannedPer = data.PlannedQty * 100 / data.POQty;

                        //data.PlanStartDate = _linetarget.Min(x => x.Date);
                        //data.PlanEndDate = _linetarget.Max(x => x.Date);

                        data.EfficiencyPlanedPer = Math.Ceiling(_linetarget.Average(x => x.PlannedEffeciency));
                        data.PlannedSAH = Math.Ceiling(_linetarget.Average(x => x.PlannedTarget * x.SMV / 60));
                    }
                    else
                    {
                        data.PlannedQty = 0;
                        data.PlannedPer = 0;

                        data.PlanStartDate = DateTime.Now.Date;
                        data.PlanEndDate = DateTime.Now.Date;

                        data.EfficiencyPlanedPer = 0;
                        data.PlannedSAH = 0;
                    }
                    if (_qcmaster.ToList().Count > 0)
                    {
                       // data.ActualStartDate = _qcmaster.Min(x => x.QCDate);
                        data.CompletedQty = _qcmaster.Where(x => x.QCStatus == 1).Sum(x => x.Qty);
                        data.DefectedQty = _qcmaster.Where(x => x.QCStatus == 2 && x.TypeOfWork == 1).Sum(x => x.Qty)
                                           - _qcmaster.Where(x => x.TypeOfWork == 2 && x.QCStatus != 2).Sum(x => x.Qty);
                        data.RejectedQty = _qcmaster.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                        data.CompletedPer = data.CompletedQty * 100 / data.POQty;

                        var targetlist = _linetarget.Where(x => x.Date.Date <= DateTime.Now.Date).ToList();
                        double total_eff = 0;
                        int daycount = 0;
                        int total_bal = 0;
                        double total_eff_diffrence = 0;

                        int day_plan_eff = 0;
                        double day_target_100per = 0;
                        int day_target = 0;
                        int day_goodpcs = 0;
                        double day_eff = 0;
                        double day_actual_eff = 0;
                        foreach (tbl_LineTarget oneday in targetlist)
                        {
                            day_target = oneday.PlannedTarget;
                            day_plan_eff = Convert.ToInt16(oneday.PlannedEffeciency);
                            if (day_plan_eff > 0)
                                day_target_100per = (double)day_target * 100 / (double)day_plan_eff;
                            day_goodpcs = _qcmaster.Where(x => x.QCStatus == 1 && x.QCDate.Date == oneday.Date.Date && x.Color == oneday.Color).Sum(x => x.Qty);
                            total_bal += day_target - day_goodpcs;
                            if (day_target_100per > 0)
                            {
                                day_eff = (double)day_goodpcs * (double)day_plan_eff / (double)day_target;
                                day_actual_eff = (double)day_goodpcs * 100 / day_target_100per;
                            }
                            total_eff_diffrence += day_plan_eff - day_actual_eff;

                            total_eff += day_eff;
                            daycount++;
                        }
                        int todayPassPcs = 0;
                        int pendingTarget = 0;
                        int todayPlannedTarget = 0;
                        int totalPlannedTarget = 0;
                        double PlannedEfficiency = 0;
                        double todayPlannedEfficiency = 0;
                        double daytarget100per = 0;
                        int PassPcs = 0;


                        if (daycount > 0) { data.EfficiencyActualPer = Math.Ceiling((double)total_eff / (double)daycount); }
                        else { data.EfficiencyActualPer = 0; }

                        if (day_target_100per > 0)
                        {
                            if (total_bal > 0)
                                data.EfficiencyRequiredPer = Math.Ceiling(total_bal * 100 / day_target_100per);
                            else
                                data.EfficiencyRequiredPer = 0;
                        }
                        else { data.EfficiencyRequiredPer = 0; }

                        if (daycount > 0)
                        {
                            data.EfficiencyLossGain = Math.Ceiling(total_eff_diffrence / daycount);
                            if (data.EfficiencyLossGain < 0)
                                data.EfficiencyLossGain = 0;
                        }
                        else { data.EfficiencyLossGain = 0; }

                        var pobal = data.PlannedQty - data.CompletedQty;
                        long avrageprd = 0;
                        long podays = 0;
                        if (daycount > 0)
                        {
                            avrageprd = data.CompletedQty / daycount;
                            if (avrageprd > 0)
                                podays = data.PlannedQty / avrageprd;
                        }
                        var daysneed = podays - daycount;
                        var expdt = data.PlanStartDate.AddDays(podays);
                        data.ExpectedDelDate = expdt;
                        if (expdt.Date <= data.PlanEndDate.Date)
                        {
                            data.ExpectedDelay = 0;
                            data.OrderStatusText = "On Time";
                        }
                        else
                        {
                            data.ExpectedDelay = (expdt - data.PlanEndDate).Days;
                            data.OrderStatusText = "Delay";
                        }
                        if (data.CompletedQty > 0 && _linetarget.ToList().Count > 0)
                        { data.ActualSAH = Math.Ceiling(data.CompletedQty * _linetarget.FirstOrDefault().SMV / 60); }
                        else { data.ActualSAH = 0; }

                        data.PicesChecked = _qcmaster.Where(x => x.TypeOfWork == 1).Sum(x => x.Qty);
                        var GarmentsRejected = _qcmaster.Where(x => x.QCStatus == 3).Sum(x => x.Qty);
                        if (GarmentsRejected > 0 && data.PicesChecked > 0) { data.RejectionRate = GarmentsRejected * 100 / data.PicesChecked; }
                        else { data.RejectionRate = 0; }

                        var QCMasterIds = _qcmaster.Select(x => x.QCMasterId).ToList();
                        var TotalDefects = _viewqc.Where(x => x.QCDefectDetailsId > 0 && QCMasterIds.Contains(x.QCMasterId)).Count();
                        if (TotalDefects > 0 && data.PicesChecked > 0) { data.CurrentDHU = (TotalDefects * 100) / data.PicesChecked; }
                        else { data.CurrentDHU = 0; }
                    }
                    else
                    {
                        data.ActualStartDate = DateTime.Now.Date;
                        data.CompletedQty = 0;
                        data.CompletedPer = 0;
                        data.DefectedQty = 0;
                        data.RejectedQty = 0;
                        data.EfficiencyActualPer = 0;
                        data.EfficiencyRequiredPer = 0;
                        data.EfficiencyLossGain = 0;

                        data.ExpectedDelDate = DateTime.Now.Date;
                        data.ExpectedDelay = 0;
                        data.OrderStatusText = "On Time";
                        data.ActualSAH = 0;
                        data.ActualSAH = 0;
                        data.PicesChecked = 0;
                        data.RejectionRate = 0;
                        data.CurrentDHU = 0;
                    }
                }
                if (_qcmaster.ToList().Count > 0)
                {
                    data.ActualStartDate = _qcmaster.Min(x => x.QCDate);
                }
                if (_linetarget.ToList().Count > 0)
                {
                    data.PlanStartDate = _linetarget.Min(x => x.Date);
                    data.PlanEndDate = _linetarget.Max(x => x.Date);
                }
                //PO Images
                List<POImage> imagelist = new List<POImage>();
                var imglist = _context.tbl_POImages.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo ).ToList();
                POImage _poimage = new POImage();
                if (imglist.Count() > 0)
                {
                    foreach (var v in imglist)
                    {
                        _poimage = new POImage();
                        _poimage.ImagePath = v.ImagePath;
                        _poimage.ImageName = v.ImageName;
                        _poimage.PONo = v.PONo;
                        imagelist.Add(_poimage);
                    }
                }
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data, imagelist, processListt });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        //edit order 
        [HttpPost]
        [Route("PurchaseOrderView")]
        public ActionResult PurchaseOrderView([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                PurchaseOrderView data = new PurchaseOrderView();
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);
                var result = query.FirstOrDefault();

                data.Product = result.Product;
                data.Style = result.Style;
                data.Fit = result.Fit;
                data.Season = result.Season;
                data.Customer = result.Customer;
                data.Module = result.Module;
                data.SONo = result.SONo;
                data.PONo = result.PONo;
                data.FactoryID = result.FactoryID;
                data.ExFactory = result.ExFactory;
                data.PlanStDt = result.PlanStDt;
                data.OrderRemark = result.OrderRemark;
                data.OrderID = result.OrderID;
                data.OrderStatus = result.OrderStatus;
                data.Source = result.Source;
                data.EntryDate = result.EntryDate;

                IQueryable<tbl_LineTarget> _linetarget = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID
                                                                    && x.PONo == _obj.PONo);
                data.POQty = query.Sum(x => x.POQty);

                if (_linetarget.ToList().Count > 0)
                {
                    //data.PlanStDt = _linetarget.Min(x => x.Date);
                    data.PlanEndDate = _linetarget.Max(x => x.Date);
                }
                else
                {
                    //data.PlanStDt = DateTime.Now.Date;
                    data.PlanEndDate = DateTime.Now.Date;
                }

                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("PurchaseOrderColorSize/{pono}")]
        public ActionResult PurchaseOrderColorSize(string pono)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(pono))
                    query = query.Where(x => x.PONo == pono);

                var data = query.ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                List<ColorWithSize> _lstcolor = new List<ColorWithSize>();
                List<size> _lstsize = new List<size>();

                List<ExpandoObject> lstexp = new List<ExpandoObject>();
                foreach (tbl_Orders _onecolorpo in data)
                {

                    string[] sizelist = _onecolorpo.SizeList.Split(',');

                    dynamic dynamicObj = new ExpandoObject();
                    dynamicObj.Color = _onecolorpo.Color;
                    dynamicObj.Fabric = _onecolorpo.Fabric;
                    foreach (string s in sizelist)
                    {
                        var sizename = s.Split('-').First();
                        var sizeqty = Convert.ToInt16(s.Split('-').Last());

                        AddProperty(dynamicObj, sizename, sizeqty);
                    }
                    dynamicObj.TotalQty = _onecolorpo.POQty;
                    lstexp.Add(dynamicObj);
                }
                return Ok(new { lstexp });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("PurchaseOrderColorSizeList")]
        public ActionResult PurchaseOrderColorSizeList([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Orders> query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PrimaryPart == 1);
                if (!string.IsNullOrEmpty(_obj.PONo))
                    query = query.Where(x => x.PONo == _obj.PONo);

                var data = query.ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                List<ColorWithSize> _lstcolor = new List<ColorWithSize>();
                List<size> _lstsize = new List<size>();

                List<ExpandoObject> _lst = new List<ExpandoObject>();
                foreach (tbl_Orders _onecolorpo in data)
                {

                    string[] sizelist = _onecolorpo.SizeList.Split(',');

                    dynamic dynamicObj = new ExpandoObject();
                    dynamicObj.Hexcode = _onecolorpo.Hexcode;
                    dynamicObj.HexcodeValue = _onecolorpo.Hexcode;
                    dynamicObj.Color = _onecolorpo.Color;
                    dynamicObj.Fabric = _onecolorpo.Fabric;

                    foreach (string s in sizelist)
                    {
                        var sizename = s.Split('-').First();
                        var sizeqty = Convert.ToInt16(s.Split('-').Last());
                        if (sizeqty > 0)
                            AddProperty(dynamicObj, sizename, sizeqty);
                    }
                    dynamicObj.TotalQty = _onecolorpo.POQty;
                    _lst.Add(dynamicObj);
                }
                return Ok(new { _lst });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
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
        [Route("PutOrder")]
        public ActionResult PutOrder([FromBody] List<tbl_Orders> list)
        {
            // Part is use for store prv pono in case if pono modify need to remove prv po
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                if (list.Count > 0)
                {
                    long orderid = 0;
                    var lastorder = _context.tbl_Orders.OrderBy(x => x.OrderID).LastOrDefault();
                    orderid = (lastorder == null ? 0 : lastorder.OrderID) + 1;
                    var data = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == list.FirstOrDefault().Part).Select(x => new { x.Module, x.Part }).FirstOrDefault();

                    var record = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == list.FirstOrDefault().Part).ToList();
                    if (record != null)
                    {
                        _context.tbl_Orders.RemoveRange(record);
                    }

                    foreach (tbl_Orders onerow in list)
                    {
                        //var record = _context.tbl_Orders.Where(x => x.PONo == onerow.PONo && x.Color == onerow.Color).SingleOrDefault();
                        //if (record != null)
                        //{
                        //    record.PONo = onerow.PONo;
                        //    record.Style = onerow.Style;
                        //    record.Fit = onerow.Fit;
                        //    record.Product = onerow.Product;
                        //    record.SONo = onerow.SONo;
                        //    record.Season = onerow.Season;
                        //    record.Customer = onerow.Customer;
                        //    record.OrderRemark = onerow.OrderRemark;
                        //    record.ExFactory = onerow.ExFactory;
                        //    record.PlanStDt = onerow.PlanStDt;
                        //    record.Fabric = onerow.Fabric;
                        //    record.SizeList = onerow.SizeList;
                        //    record.POQty = onerow.POQty;
                        //    _context.tbl_Orders.Update(record);
                        //}

                        tbl_Orders otbl_Orders = new tbl_Orders();

                        orderid++;
                        otbl_Orders.OrderID = orderid;
                        otbl_Orders.Module = data.Module;
                        otbl_Orders.SONo = onerow.SONo;
                        otbl_Orders.PONo = onerow.PONo;
                        otbl_Orders.Style = onerow.Style;
                        otbl_Orders.Fit = onerow.Fit;
                        otbl_Orders.Product = onerow.Product;
                        otbl_Orders.Season = onerow.Season;
                        otbl_Orders.Customer = onerow.Customer;
                        otbl_Orders.PlanStDt = onerow.PlanStDt;
                        otbl_Orders.ExFactory = onerow.ExFactory;
                        otbl_Orders.PrimaryPart = 1;
                        otbl_Orders.Part = data.Part;
                        otbl_Orders.Color = onerow.Color;
                        otbl_Orders.Hexcode = onerow.Hexcode;
                        otbl_Orders.Fabric = onerow.Fabric;
                        otbl_Orders.OrderRemark = onerow.OrderRemark;
                        otbl_Orders.IsSizeRun = 1;
                        otbl_Orders.POQty = onerow.POQty;
                        otbl_Orders.SizeList = onerow.SizeList;
                        otbl_Orders.OrderStatus = 1;
                        otbl_Orders.UserID = _UserTokenInfo.UserID;
                        otbl_Orders.FactoryID = _UserTokenInfo.FactoryID;
                        otbl_Orders.EntryDate = onerow.EntryDate;
                        otbl_Orders.LastSyncedAt = System.DateTime.Now;
                        if (otbl_Orders.Source != "ERPAPP")
                        {
                            otbl_Orders.WFXColorCode = "";
                            otbl_Orders.WFXColorName = "";
                        }
                        _context.tbl_Orders.Add(otbl_Orders);
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("RequestStatusList")]
        public ActionResult RequestStatusList()
        {
            var status = 200;
            var message = "";
            List<tbl_QCRequest> data = new List<tbl_QCRequest>();

            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_QCRequest> query = _context.tbl_QCRequest.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.GRNstatus.ToLower() != "created");
                data = query.OrderByDescending(x => x.QCRequestID).ToList();
                if (data.Count > 0)
                {
                    status = 200;
                    message = "Success";
                }
                else
                {
                    status = 204;
                    message = "No record found.";
                }
            }
            catch (Exception ex)
            {
                status = 400;
                return Ok(new { status = status, message = ex.Message });
            }
            return Ok(new { status = status, message = message, data = data });
        }

        [HttpPost]
        [Route("DeleteRequestStatus")]
        public ActionResult DeleteRequestStatus([FromBody] tbl_QCRequest _obj)
        {
            var status = 200;
            var message = "";
            List<tbl_QCRequest> data = new List<tbl_QCRequest>();
            _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

            try
            {
                tbl_QCRequest _requeststatus = _context.tbl_QCRequest.Where(x => x.QCRequestID == _obj.QCRequestID).FirstOrDefault();
                IQueryable<tbl_QCMaster> _qcmasterlist = _context.tbl_QCMaster.Where(x => x.QCRequestID == _obj.QCRequestID);
                if (_requeststatus == null)
                {
                    status = 204;
                    message = "No record found.";
                }
                else
                {

                    _context.tbl_QCRequest.Remove(_requeststatus);
                    _context.tbl_QCMaster.RemoveRange(_qcmasterlist);
                    _context.SaveChanges();
                    IQueryable<tbl_QCRequest> query = _context.tbl_QCRequest.Where(x => x.FactoryID == _UserTokenInfo.FactoryID);
                    data = query.ToList();
                    message = "Delete Sucessfully";
                    return Ok(new { status = status, message = message, data = data });
                }
            }
            catch (Exception ex)
            {
                status = 400;
                message = ex.Message;
                return Ok(new { status = status, message = message });

            }
            return Ok(new { status = status, message = message });
        }

        [HttpPost]
        [Route("SyncData")]
        public async Task<ActionResult> SyncData([FromBody] List<tbl_QCRequest> _obj)
        {
            _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
            var status = 200;
            var message = "";
            try
            {
                var ERPAPIURL = _context.tbl_Factory.Where(x => x.FactoryID == _UserTokenInfo.FactoryID)
                                .Join(
                                    _context.tbl_Clusters,
                                    Factory => Factory.ClusterID,
                                    cluster => cluster.ClusterID,
                                    (Factory, cluster) => new
                                    {
                                        OrganisationID = cluster.OrganisationID
                                    }
                                )
                                .Join(
                                _context.tbl_Organisations,
                                cluster => cluster.OrganisationID,
                                Organisations => Organisations.OrganisationID,
                                (cluster, Organisations) => new
                                {
                                    OrganisationID = Organisations.OrganisationID,
                                    ERPAPIUrl = Organisations.ERPAPIURL
                                }
                            )
                            .Select(x => x.ERPAPIUrl).FirstOrDefault();
                foreach (var rec in _obj)
                {
                    if (rec.RequestType == "IssueToNextProcess")
                    {
                        var data = _context.tbl_OrderIssue.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCRequestID == rec.QCRequestID).ToList();
                        if (data.Count() == 0)
                        {
                            return Ok(new { status = 400, message = "No record found." });
                        }
                        var QCRequest = _context.tbl_QCRequest.Where(x => x.QCRequestID == rec.QCRequestID).OrderByDescending(x => x.QCRequestID).LastOrDefault();
                        data.ForEach(x => x.BatchNumber = QCRequest.TranNum);

                        var result = await _userWebService.PostIssuetoNextProcessAsync<ERPCreateGRNOutputDto>(data, ERPAPIURL);
                        if (result.status == System.Net.HttpStatusCode.OK)
                        {
                            long stockGRNID = result.data.StockGRNID;
                            data.ForEach(x => x.WFXStockGRNID = stockGRNID);
                            _context.tbl_OrderIssue.UpdateRange(data);
                            _context.SaveChanges();
                            QCRequest.StockGRNID = stockGRNID;
                            QCRequest.SyncedAt = DateTime.Now;
                            QCRequest.GRNstatus = "Created";
                            _context.tbl_QCRequest.UpdateRange(QCRequest);
                            _context.SaveChanges();

                            var POList = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == QCRequest.SONo && x.PONo == QCRequest.PONo).ToList();
                            POList.ForEach(x => x.LastSyncedAt = DateTime.Now);
                            _context.tbl_Orders.UpdateRange(POList);
                            _context.SaveChanges();
                            return Ok(new { status = 200, message = result.message });
                        }

                        else
                        {
                            //var QCRequest = _context.tbl_QCRequest.Where(x => x.QCRequestID == _obj.QCRequestID).LastOrDefault();
                            QCRequest.ErrorMessage = result.message;
                            _context.SaveChanges();
                            return Ok(new { status = 400, message = result.message });
                        }
                    }
                    else
                    {
                        var data = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.QCRequestID == rec.QCRequestID).ToList();
                        if (data.Count() == 0)
                        {
                            return Ok(new { status = 400, message = "No record found." });
                        }
                        var QCRequest = _context.tbl_QCRequest.Where(x => x.QCRequestID == rec.QCRequestID).OrderByDescending(x => x.QCRequestID).LastOrDefault();
                        data.ForEach(x => x.BatchNumber = QCRequest.TranNum);

                        var result = await _userWebService.PostEndShiftGRNAsync<ERPCreateGRNOutputDto>(data, ERPAPIURL);
                        if (result.status == System.Net.HttpStatusCode.OK)
                        {
                            long stockGRNID = result.data.StockGRNID;
                            data.ForEach(x => x.WFXStockGRNID = stockGRNID);
                            _context.tbl_QCMaster.UpdateRange(data);
                            _context.SaveChanges();
                            QCRequest.StockGRNID = stockGRNID;
                            QCRequest.SyncedAt = DateTime.Now;
                            QCRequest.GRNstatus = "Created";
                            _context.tbl_QCRequest.UpdateRange(QCRequest);
                            _context.SaveChanges();

                            var POList = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == QCRequest.SONo && x.PONo == QCRequest.PONo).ToList();
                            POList.ForEach(x => x.LastSyncedAt = DateTime.Now);
                            _context.tbl_Orders.UpdateRange(POList);
                            _context.SaveChanges();
                            return Ok(new { status = 200, message = result.message });
                        }

                        else
                        {
                            //var QCRequest = _context.tbl_QCRequest.Where(x => x.QCRequestID == _obj.QCRequestID).LastOrDefault();
                            QCRequest.ErrorMessage = result.message;
                            _context.SaveChanges();
                            return Ok(new { status = 400, message = result.message });
                        }
                    }
                }
                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                status = 400;
                message = ex.Message;
                return Ok(new { status = status, message = message });
            }
        }
        [Route("PostIsPartExist")]
        [HttpPost]
        public ActionResult PostIsPartExist([FromBody] tbl_Orders _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo && x.SONo == _obj.SONo && x.Part == _obj.Part).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("PutOB")]
        public ActionResult PutOB([FromBody] List<tbl_OB> _obj)
        {
            try
            {
                foreach (var l in _obj)
                {
                    var OBDetail = _context.tbl_OB.Where(x => x.OBID == l.OBID && x.FactoryID == l.FactoryID && x.SONo == l.SONo).ToList();
                    if (OBDetail != null)
                    {
                        _context.tbl_OB.RemoveRange(OBDetail);
                        tbl_OB otbl_tbl_OB = new tbl_OB();

                        otbl_tbl_OB.OBID = l.OBID;
                        otbl_tbl_OB.PONo = l.PONo;
                        otbl_tbl_OB.SONo = l.SONo;
                        otbl_tbl_OB.SNo = l.SNo;
                        otbl_tbl_OB.OperationCode = l.OperationCode;
                        otbl_tbl_OB.OperationName = l.OperationName;
                        otbl_tbl_OB.EntryDate = l.EntryDate;
                        otbl_tbl_OB.UserID = l.UserID;
                        otbl_tbl_OB.SMV = l.SMV;
                        otbl_tbl_OB.Section = l.Section;
                        otbl_tbl_OB.OBLocation = l.OBLocation;
                        otbl_tbl_OB.FactoryID = l.FactoryID;
                        _context.tbl_OB.Add(otbl_tbl_OB);
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
        [Route("PostIsFitExist")]
        [HttpPost]
        public ActionResult PostIsFitExist([FromBody] tbl_ProductFit _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_ProductFit.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.FitType == _obj.FitType).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostIsCustomerExist")]
        [HttpPost]
        public ActionResult PostIsCustomerExist([FromBody] tbl_Customer _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Customer.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.CustomerName == _obj.CustomerName).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostIsProductExist")]
        [HttpPost]
        public ActionResult PostIsProductExist([FromBody] tbl_Products _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Products.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.ProductName == _obj.ProductName).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostIsOrderRun")]
        [HttpPost]
        public ActionResult PostIsOrderRun([FromBody] tbl_LineTarget _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo && x.SONo == _obj.SONo && x.Color == _obj.Color && x.QCDate.Date == _obj.Date.Date).FirstOrDefault();
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
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostTotalCheckedPcs")]
        [HttpPost]
        public ActionResult PostTotalCheckedPcs([FromBody] tbl_QCMaster _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_QCMaster.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.TypeOfWork == 1 && x.Size == _obj.Size && x.Color == _obj.Color && x.PONo == _obj.PONo && x.QCDate.Date == _obj.QCDate.Date).ToList();
                if (data == null)
                {
                    return Ok(new { status = 401, message = "Not Found" });
                }
                else
                {
                    return Ok(new { status = 200, data = data.Sum(x => x.Qty) });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PostCheckLineTargetValues")]
        [HttpPost]
        public ActionResult PostCheckLineTargetValues([FromBody] tbl_LineTarget _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.PONo == _obj.PONo && x.SONo == _obj.SONo && x.Color == _obj.Color && x.Date.Date == _obj.Date.Date).ToList();
                if (data == null)
                {
                    return Ok(new { status = 401, message = "Not Found" });
                }
                else
                {
                    return Ok(new { status = 200, data = data });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("FactoryProcessList")]
        public ActionResult FactoryProcessList([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_OrderProcess> dataQuery;
                dynamic data = null;
                var query = _context.tbl_Orders.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.ProcessCode != null && x.ProcessCode != "");
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
                    dataQuery = (IQueryable<tbl_OrderProcess>)
                      from p in query
                      join s in _context.tbl_LineTarget.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.Line == _obj.Line)
                        on new { p.FactoryID, p.PONo } equals new { s.FactoryID, s.PONo }
                      group p by new
                      {
                          ProcessCode = p.ProcessCode,
                          ProcessName = p.ProcessName
                      }
                      into g
                      select new tbl_OrderProcess
                      {
                          ProcessCode = g.Key.ProcessCode,
                          ProcessName = g.Key.ProcessName
                      };
                }
                else
                {
                    dataQuery = (IQueryable<tbl_OrderProcess>)
                        query.
                        GroupBy(x => new
                        {
                            ProcessCode = x.ProcessCode,
                            ProcessName = x.ProcessName
                        }).
                        Select(g => new tbl_OrderProcess
                        {
                            ProcessCode = g.Key.ProcessCode,
                            ProcessName = g.Key.ProcessName
                        });
                }

                data = dataQuery.Select(x => new { ID = x.ProcessCode, Text = x.ProcessName + "(" + x.ProcessCode + ")" }).ToList();
                if (data == null)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [Route("GetProcessTemplate")]
        [HttpPost]
        public ActionResult GetProcessTemplate([FromBody] SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_OrderProcess.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == _obj.SONo).ToList();
                if (data == null)
                {
                    

                    return Ok(new { status = 401, message = "Not Found" });
                }
                else
                {
                    var result = data.Select(x => new
                    {
                        OrderProcessID = x.OrderProcessID,
                        ProcessCode = x.ProcessCode,
                        ProcessName = x.ProcessName,
                        AfterProcessCode = x.AfterProcessCode,
                        AfterProcessName = x.AfterProcessName + "(" + x.AfterProcessCode + ")",
                        SONo = x.SONo,
                        PONo = x.PONo,
                        FactoryID = x.FactoryID,
                        UserID = x.UserID,
                        CreatedOn = x.CreatedOn,
                        ProcessEnabled = x.ProcessEnabled
                    });
                    return Ok(new { status = 200,  result=result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [Route("PutProcessTemplate")]
        [HttpPost]
        public ActionResult PutProcessTemplate([FromBody] List<tbl_OrderProcess> _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);

                foreach (var t in _obj)
                {
                    var data = _context.tbl_OrderProcess.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.SONo == t.SONo && x.OrderProcessID == t.OrderProcessID).FirstOrDefault();
                    if (data != null)
                    {
                        //data.ProcessName = t.ProcessName;
                        //data.ProcessCode = t.ProcessCode;
                        //data.ProcessName = t.ProcessName;
                        //data.AfterProcessCode = t.AfterProcessCode;
                        //data.AfterProcessName = t.AfterProcessName;
                        data.ProcessEnabled = t.ProcessEnabled;
                        _context.tbl_OrderProcess.Update(data);
                        _context.SaveChanges();

                    }
                    else
                    {
                        return Ok(new { status = 400, message = "No record found." });

                    }
                }

                return Ok(new { status = 200, message = "Suceess" });

            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        
    }
}

public class SOViewModel
{
    public string Module { get; set; }
    public string SONo { get; set; }
    public string Style { get; set; }
    public string Fit { get; set; }
    public string Product { get; set; }
    public string Season { get; set; }
    public string Customer { get; set; }
    public int SOQty { get; set; }
    public int FactoryID { get; set; }
    public int NoOfPO { get; set; }
    public int CompletedQty { get; set; }
    public int RejectedQty { get; set; }
    public int DefectedQty { get; set; }
    public int CompletedPer { get; set; }

    public double PlannedSAH { get; set; }
    public double ActualSAH { get; set; }
    public double EfficiencyPlanedPer { get; set; }
    public double EfficiencyActualPer { get; set; }
    public double EfficiencyRequiredPer { get; set; }
    public int PicesChecked { get; set; }
    public int RejectionRate { get; set; }
    public int CurrentDHU { get; set; }

    public int DelayedPO { get; set; }
    public int CompletedPO { get; set; }
    public int HoldPO { get; set; }
    public int OnTimePO { get; set; }
}
public class POView
{
    public long OrderQty { get; set; }
    public long PlannedQty { get; set; }
    public double PlannedPer { get; set; }
    public long CompletedQty { get; set; }
    public long RejectedQty { get; set; }
    public long DefectedQty { get; set; }
    public double CompletedPer { get; set; }

    public double EfficiencyPlanedPer { get; set; }
    public double EfficiencyActualPer { get; set; }
    public double EfficiencyRequiredPer { get; set; }
    public double EfficiencyLossGain { get; set; }

    public int POBal { get; set; }

    public double PlannedSAH { get; set; }
    public double ActualSAH { get; set; }
    public double ActualSAMOfOneGarment { get; set; }
    public double PlannedManHrs { get; set; }
    public double ActualManHrs { get; set; }

    public long DHUCurrent { get; set; }
    public long TotalDefectsFound { get; set; }
    public long DHUAvg { get; set; }
    public long DHUMax { get; set; }
    public long RejCurrent { get; set; }
    public long TotalRejected { get; set; }
    public long TotalInspected { get; set; }
    public long RejAvg { get; set; }
    public long RejMax { get; set; }

    public long OrderID { get; set; }
    public string Module { get; set; }
    public string SONo { get; set; }
    public string PONo { get; set; }
    public string Style { get; set; }
    public string Fit { get; set; }
    public string Product { get; set; }
    public string Season { get; set; }
    public string Customer { get; set; }
    public int PrimaryPart { get; set; }
    public string Part { get; set; }
    public string Color { get; set; }
    public string Hexcode { get; set; }
    public string Fabric { get; set; }
    public string OrderRemark { get; set; }
    public int IsSizeRun { get; set; }
    public int POQty { get; set; }
    public string SizeList { get; set; }
    public int OrderStatus { get; set; }
    public int UserID { get; set; }
    public int FactoryID { get; set; }


    public int PicesChecked { get; set; }
    public int RejectionRate { get; set; }
    public int CurrentDHU { get; set; }

    public DateTime RecvDate { get; set; }
    public DateTime PlanStartDate { get; set; }
    public DateTime PlanEndDate { get; set; }
    public DateTime ExFactoryDate { get; set; }
    public DateTime ActualStartDate { get; set; }

    public int ExpectedDelay { get; set; }
    public DateTime ExpectedDelDate { get; set; }
    public string OrderStatusText { get; set; }
    public string ProcessCode { get; set; }
    public string ProcessName { get; set; }
}
public class color
{
    public string colorname { get; set; }
    public List<size> sizelist { get; set; }
}
public class size
{
    public string OrderSizeName { get; set; }
    public int OrderQty { get; set; }
    public int PlannedQty { get; set; }
    public int YetToPlanQty { get; set; }
    public int CompletedQty { get; set; }
    public int BalanceQty { get; set; }
    public int ComplPer { get; set; }


    public int TotalOrderQty { get; set; }
    public int TotalPlannedQty { get; set; }
    public int TotalYetToPlanQty { get; set; }
    public int TotalCompletedQty { get; set; }
    public int TotalBalanceQty { get; set; }
    public int TotalComplPer { get; set; }
}

//order edit section
public class PurchaseOrderView
{
    public long OrderQty { get; set; }

    public long OrderID { get; set; }
    public string Module { get; set; }
    public string SONo { get; set; }
    public string PONo { get; set; }
    public string Style { get; set; }
    public string Fit { get; set; }
    public string Product { get; set; }
    public string Season { get; set; }
    public string Customer { get; set; }
    public int PrimaryPart { get; set; }
    public string Part { get; set; }
    public string Color { get; set; }
    public string Hexcode { get; set; }
    public string Fabric { get; set; }
    public string OrderRemark { get; set; }

    public int POQty { get; set; }
    public string SizeList { get; set; }
    public int OrderStatus { get; set; }
    public int UserID { get; set; }
    public int FactoryID { get; set; }

    public DateTime PlanStDt { get; set; }
    public DateTime PlanEndDate { get; set; }
    public DateTime ExFactory { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime ActualStartDate { get; set; }
    public string OrderStatusText { get; set; }
    public string Source { get; set; }
}
public class ColorWithSize
{
    public string colorname { get; set; }
    public string febric { get; set; }

    public string SizeName { get; set; }
    public int SizeQty { get; set; }

    public string Size1Name { get; set; }
    public int Size1Qty { get; set; }
    public string Size2Name { get; set; }
    public int Size2Qty { get; set; }
    public string Size3Name { get; set; }
    public int Size3Qty { get; set; }
    public string Size4Name { get; set; }
    public int Size4Qty { get; set; }
    public string Size5Name { get; set; }
    public int Size5Qty { get; set; }
    public string Size6Name { get; set; }
    public int Size6Qty { get; set; }
    public int TotalQty { get; set; }
    public List<size> sizelist { get; set; }
    public Dictionary<string, object> Fields { get; set; }
}

public class ERPOrderIntegration
{
    public List<tbl_Orders> OrderData { get; set; }
    public List<tbl_OrderProcess> PDData { get; set; }
}
public class processDetail
{
    public string ProcessName { get; set; }
    public string ProcessCode { get; set; }
    public double ActualSAH { get; set; }
    public double PlannedSAH { get; set; }
    public int PassPcs { get; set; }
    public int DefectPcs { get; set; }
    public int RejectPcs { get; set; }
    public double ActualEfficiency { get; set; }



}