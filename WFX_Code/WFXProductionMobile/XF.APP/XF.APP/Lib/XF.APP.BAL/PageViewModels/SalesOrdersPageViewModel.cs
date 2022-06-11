using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Essentials;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.APP.BAL
{
    public class SalesOrdersPageViewModel : NavigationSearchPageViewModel, ISalesOrdersViewModel
    {
        public ICommand NextButtonCommand { set; get; }
        public ICommand RefreshCommand { set; get; }

        private ObservableCollection<SalesOrder> SoListOriginal;
        private ObservableCollection<PurchaseOrder1> PoListOriginal;
        private ITransactionService transactionService;

        public static bool ChangeToSoView = true;

        ObservableCollection<SalesOrder> soList;
        public ObservableCollection<SalesOrder> SoList
        {
            get { return soList; }
            set { SetProperty(ref soList, value); }
        }

        ObservableCollection<PurchaseOrder1> poList;
        public ObservableCollection<PurchaseOrder1> PoList
        {
            get { return poList; }
            set { SetProperty(ref poList, value); }
        }

        string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                FilterListData(value);
            }
        }

        SalesOrder selectedSo;
        public SalesOrder SelectedSo
        {
            get { return selectedSo; }
            set
            {
                SetProperty(ref selectedSo, value);
                if (selectedSo != null)
                    NextButtonCommand.Execute(1);
            }
        }

        PurchaseOrder1 selectedPo;
        public PurchaseOrder1 SelectedPo
        {
            get { return selectedPo; }
            set
            {
                SetProperty(ref selectedPo, value);
                if (selectedPo != null)
                    NextButtonCommand.Execute(2);
            }
        }

        bool isPoView;
        public bool IsPoView
        {
            get { return isPoView; }
            set { SetProperty(ref isPoView, value); }
        }

        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        int poQty;
        public int PoQty
        {
            get { return poQty; }
            set { SetProperty(ref poQty, value); }
        }

        public SalesOrdersPageViewModel()
        {
            NextButtonCommand = new BaseCommand(async (object obj) => await NextButtonCommandClicked(obj));
            SearchCommand = new BaseCommand((object obj) => SearchCommandClicked(obj as string));
            BackButtonCommand = new BaseCommand((object obj) => BackButtonCommandClicked());
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
            RefreshCommand = new BaseCommand((object obj) => RefreshCommandClicked());

            transactionService = ServiceLocator.Resolve<ITransactionService>();
        }

        public void OnScreenAppearing()
        {
            if (ChangeToSoView)
            {
                Task.Run(async () => { await FetchSoList(); });

                IsPoView = false;
                ChangeToSoView = false;
            }
            //Task.Run(async () => { await FetchSoList(); });
        }

        private async void RefreshCommandClicked()
        {
            IsRefreshing = true;

            if (IsPoView)
                await FetchPoList(SelectedSo.soNo);
            else
                await FetchSoList();

            IsRefreshing = false;
        }
        // Need to hit API POList only One Time, i have to write Logic
        private async Task FetchSoList()
        {
            if (IsBusy || InternetConnectivity.IsNetworkNotAvailable())
                return;

            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);

                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();
                var userFactoryId = Preferences.Get("FACTORY_ID", 0);
                string Line = Preferences.Get("LINE_ID", string.Empty);
                string RoleName = Preferences.Get("RoleName", string.Empty);
                
                var result = await userService.GetSoListAsync<SoViewOutputDto>(new SoViewInputDto() { RoleName = RoleName, Line = Line }, Preferences.Get("AUTH_KEY", null)); ;


                // Getch shift details
                if (!(InspectionPageViewModel.shiftDetails != null && InspectionPageViewModel.shiftDetails.Count > 0))
                {
                    var shiftResult = await userService.GetShiftAsync<ShiftOutputDto>();
                    if (shiftResult?.status == System.Net.HttpStatusCode.OK)
                    {
                        InspectionPageViewModel.shiftDetails = shiftResult.data;
                    }
                }

                UserDialogs.Instance.HideLoading();

                //navigate to home page
                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    SoList = new ObservableCollection<SalesOrder>();
                    // Get db values
                    int soCompletedQty = 0;
                    int completedPo = 0;
                    foreach (var so in result.data)
                    {
                        soCompletedQty = 0;
                        completedPo = 0;
                        var POData = await userService.GetPoListAsync<SoPoOutputDto>(new SoPoInputDto()
                        {
                            SONo = so.soNo,
                            Line = Line,
                            TabletID = Preferences.Get("TabletID", string.Empty)
                        }, Preferences.Get("AUTH_KEY", null));
                        List<string> poNoList = new List<string>();
                        if (POData.data != null)
                            poNoList = POData.data.Select(p => p.poNo).ToList();
                        //navigate to home page
                        if (POData.status == System.Net.HttpStatusCode.OK)
                        {
                            PoList = new ObservableCollection<PurchaseOrder1>();



                            // Update in db
                            var dbPoList = new List<PurchaseOrderDto>();



                            if (POData.data != null && POData.data.Count > 0)
                            {
                                POData.data = POData.data.Where(x => x.soNo == so.soNo).ToList();
                                var qcData = POData.qcmasterlist.ToList();
                                foreach (var po in POData.data)
                                {
                                    int completedColors = 0;
                                    int completedQty = 0;
                                    int totalColors = 0;
                                    int totalQty = 0;



                                    try
                                    {
                                        totalColors = POData.data.Where(x => x.soNo == po.soNo && x.poNo == po.poNo).Count();
                                        totalQty = POData.data.Where(x => x.soNo == po.soNo && x.poNo == po.poNo).Sum(x => x.poQty);
                                        completedColors = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo).Select(p => p.Color).Distinct().Count();
                                        completedQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 1).Sum(x => x.Qty);
                                    }
                                    catch (Exception ex)
                                    {
                                        await Logger.AddException(ex);
                                    }



                                    dbPoList.Add(new PurchaseOrderDto
                                    {
                                        UserID = po.userID,
                                        FactoryID = po.factoryID,
                                        OrderID = po.orderID,
                                        SoNo = po.soNo,
                                        PoNo = po.poNo,
                                        PoQty = po.poQty,
                                        Part = po.part,
                                        Color = po.color,
                                        Customer = po.customer,
                                        EntryDate = po.entryDate,
                                        ExFactory = po.exFactory,
                                        Fabric = po.fabric,
                                        Fit = po.fit,
                                        Hexcode = po.hexcode,
                                        IsSizeRun = po.isSizeRun,
                                        Module = po.module,
                                        OrderRemark = po.orderRemark,
                                        OrderStatus = po.orderStatus,
                                        PlanStDt = po.planStDt,
                                        PrimaryPart = po.primaryPart,
                                        Product = po.product,
                                        Season = po.season,
                                        SizeList = po.sizeList,
                                        Style = po.style,
                                        WFXColorCode = po.WFXColorCode,
                                        WFXColorName = po.WFXColorName,
                                        //ProcessCode = po.ProcessCode,
                                        //ProcessName = po.ProcessName,
                                        //FulfillmentType = po.FulfillmentType,
                                        PassQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 1 && x.Color == po.color).Sum(x => x.Qty),
                                        DefectQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 2 && x.Color == po.color
                                        && x.TypeOfWork == 1).Sum(x => x.Qty),
                                        RejectQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.Color == po.color && x.QCStatus == 3).Sum(x => x.Qty),



                                    });
                                }
                            }



                            PoQty = PoList.Select(p => p.poNo).Distinct().Count();

                            await transactionService.SaveUpdatePoAsync(dbPoList, InspectionPageViewModel.shiftDetails);
                        }
                        if (poNoList.Count() > 0)
                        {
                            if (POData.qcmasterlist.Count() > 0)
                            {
                                completedPo = POData.qcmasterlist.Where(x => x.SONo == so.soNo && poNoList.Contains(x.PONo)).Select(p => p.PONo).Distinct().Count();
                                soCompletedQty = POData.qcmasterlist.Where(x => x.SONo == so.soNo && poNoList.Contains(x.PONo) && x.QCStatus == 1).Sum(x => x.Qty);
                            }
                            SoList.Add(new SalesOrder
                            {
                                module = so.module,
                                soNo = so.soNo,
                                style = so.style,
                                fit = so.fit,
                                product = so.product,
                                season = so.season,
                                customer = so.customer,
                                soQty = so.soQty,
                                factoryID = so.factoryID,
                                noOfPO = so.noOfPO,
                                completedPo = completedPo,
                                completedQty = soCompletedQty,
                                poGraphProgress = (completedPo == 0 || so.noOfPO == 0) ? 0 : (float)completedPo / so.noOfPO,
                                qtyGraphProgress = (soCompletedQty == 0 || so.soQty == 0) ? 0 : (float)soCompletedQty / so.soQty,
                            });
                        }
                    }

                    SoListOriginal = SoList;
                }
                else
                {
                    UserDialogs.Instance.Alert("Not able to fetch orders, Please try again later.");
                }
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
                UserDialogs.Instance.HideLoading();

                Preferences.Clear();
                NativeService.NavigationService.SetRootPage("LoginPage");

            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FetchPoList(string soNo)
        {
            string Line = Preferences.Get("LINE_ID", string.Empty);
            if (IsBusy || InternetConnectivity.IsNetworkNotAvailable())
                return;

            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);

                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();
                var result = await userService.GetPoListAsync<SoPoOutputDto>(new SoPoInputDto()
                {
                    SONo = soNo,
                    Line = Line,
                    TabletID = Preferences.Get("TabletID", string.Empty)
                }, Preferences.Get("AUTH_KEY", null));

                //navigate to home page
                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    PoList = new ObservableCollection<PurchaseOrder1>();

                    // Update in db
                    var dbPoList = new List<PurchaseOrderDto>();

                    if (result.data != null && result.data.Count > 0)
                    {
                        result.data = result.data.Where(x => x.soNo == SelectedSo.soNo).ToList();
                        var qcData = result.qcmasterlist.ToList();
                        foreach (var po in result.data)
                        {
                            int completedColors = 0;
                            int completedQty = 0;
                            int totalColors = 0;
                            int totalQty = 0;

                            try
                            {
                                totalColors = result.data.Where(x => x.soNo == po.soNo && x.poNo == po.poNo).Count();
                                totalQty = result.data.Where(x => x.soNo == po.soNo && x.poNo == po.poNo).Sum(x => x.poQty);
                                completedColors = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo).Select(p => p.Color).Distinct().Count();
                                completedQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 1).Sum(x => x.Qty);
                            }
                            catch (Exception ex)
                            {
                                await Logger.AddException(ex);
                            }

                            // Supress duplicae po tiles for different colors
                            if (!PoList.Any(x => x.soNo == po.soNo && x.poNo == po.poNo))
                            {
                                PoList.Add(new PurchaseOrder1
                                {
                                    orderID = po.orderID,
                                    module = po.module,
                                    soNo = po.soNo,
                                    poNo = po.poNo,
                                    style = po.style,
                                    fit = po.fit,
                                    product = po.product,
                                    season = po.season,
                                    customer = po.customer,
                                    planStDt = po.planStDt,
                                    exFactory = po.exFactory,
                                    primaryPart = po.primaryPart,
                                    part = po.part,
                                    color = po.color,
                                    hexcode = po.hexcode,
                                    fabric = po.fabric,
                                    orderRemark = po.orderRemark,
                                    isSizeRun = po.isSizeRun,
                                    poQty = totalQty,
                                    sizeList = po.sizeList,
                                    orderStatus = po.orderStatus,
                                    userID = po.userID,
                                    factoryID = po.factoryID,
                                    entryDate = po.entryDate,
                                    line = Line,
                                    totalColors = totalColors,
                                    completedColors = completedColors,
                                    completedQty = completedQty,
                                    WFXColorCode = po.WFXColorCode,
                                    WFXColorName = po.WFXColorName,
                                    colorsGraphProgress = (completedColors == 0 || totalColors == 0) ? 0 : (float)completedColors / totalColors,
                                    qtyGraphProgress = (completedQty == 0 || totalQty == 0) ? 0 : (float)completedQty / totalQty
                                });
                            }

                            dbPoList.Add(new PurchaseOrderDto
                            {
                                UserID = po.userID,
                                FactoryID = po.factoryID,
                                OrderID = po.orderID,
                                SoNo = po.soNo,
                                PoNo = po.poNo,
                                PoQty = po.poQty,
                                Part = po.part,
                                Color = po.color,
                                Customer = po.customer,
                                EntryDate = po.entryDate,
                                ExFactory = po.exFactory,
                                Fabric = po.fabric,
                                Fit = po.fit,
                                Hexcode = po.hexcode,
                                IsSizeRun = po.isSizeRun,
                                Module = po.module,
                                OrderRemark = po.orderRemark,
                                OrderStatus = po.orderStatus,
                                PlanStDt = po.planStDt,
                                PrimaryPart = po.primaryPart,
                                Product = po.product,
                                Season = po.season,
                                SizeList = po.sizeList,
                                Style = po.style,
                                WFXColorCode = po.WFXColorCode,
                                WFXColorName = po.WFXColorName,
                                //ProcessCode = po.ProcessCode,
                                //ProcessName = po.ProcessName,
                                //FulfillmentType = po.FulfillmentType,
                                PassQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 1 && x.Color == po.color).Sum(x => x.Qty),
                                DefectQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.QCStatus == 2 && x.Color == po.color
                                                                                && x.TypeOfWork == 1).Sum(x => x.Qty),
                                RejectQty = qcData.Where(x => x.SONo == po.soNo && x.PONo == po.poNo && x.Color == po.color && x.QCStatus == 3).Sum(x => x.Qty),

                            });
                        }
                    }

                    PoQty = PoList.Select(p => p.poNo).Distinct().Count();
                    PoListOriginal = PoList;

                    await transactionService.SaveUpdatePoAsync(dbPoList, InspectionPageViewModel.shiftDetails);
                    UserDialogs.Instance.HideLoading();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Alert("Not able to fetch orders, Please try again later.");
                }
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
                UserDialogs.Instance.HideLoading();

                UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task NextButtonCommandClicked(object sender)
        {
            if (Convert.ToInt32(sender) == 1)
                IsPoView = !IsPoView;
            SearchText = string.Empty;

            if (IsPoView && Convert.ToInt32(sender) == 1)
                await FetchPoList(SelectedSo.soNo);
            else
            {
                await NativeService.NavigationService.NavigateAsync("InspectionPage", SelectedPo);
            }
        }

        private void BackButtonCommandClicked()
        {
            IsPoView = !IsPoView;
        }

        private void SearchCommandClicked(string searchText)
        {
            IsSearchBarVisible = !IsSearchBarVisible;

            FilterListData(searchText);
        }

        private void CloseCommandClicked()
        {
            IsSearchBarVisible = !IsSearchBarVisible;

            if (IsPoView)
                PoList = PoListOriginal;
            else
                SoList = SoListOriginal;

            SearchText = string.Empty;
        }

        private void FilterListData(string searchText)
        {
            if (searchText == null) return;
            else if (searchText == "")
            {
                if (IsPoView)
                    PoList = PoListOriginal;
                else
                    SoList = SoListOriginal;
            }
            else if (IsPoView)
            {
                PoList = new ObservableCollection<PurchaseOrder1>(PoListOriginal.Where(x => x.poNo.ToLower().Contains(searchText.ToLower()) ||
                x.customer.ToLower().ToLower().Contains(searchText.ToLower()) ||
                x.style.ToLower().Contains(searchText.ToLower()) ||
                x.product.ToLower().Contains(searchText.ToLower())
                ));
            }
            else
            {
                SoList = new ObservableCollection<SalesOrder>(SoListOriginal.Where(x => x.soNo.ToLower().Contains(searchText.ToLower()) ||
                x.customer.ToLower().Contains(searchText.ToLower()) ||
                x.style.ToLower().Contains(searchText.ToLower()) ||
                x.product.ToLower().Contains(searchText.ToLower())
                ));
            }
        }
    }
}
