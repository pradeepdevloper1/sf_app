using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using Xamarin.Forms;
using System.Data;
using Xamarin.Essentials;

namespace XF.APP.BAL
{
    public class ProcessPageViewModel : BaseViewModel, IProcessPageViewModel
    {
        public ICommand CloseCommand1 { get; set; }
        public ICommand DropDownCommand { set; get; }
        public ICommand IssueQtyCommand { set; get; }
        
        private ITransactionService transactionService;

        ProcessDataViewModel processModel;
        public ProcessDataViewModel ProcessModel
        {
            get { return processModel; }
            set { SetProperty(ref processModel, value); }
        }

        string selectedProductionOrderNo;
        public string SelectedProductionOrderNo
        {
            get { return selectedProductionOrderNo; }
            set { SetProperty(ref selectedProductionOrderNo, value); }
        }

        string selectedWorkOrderNo;
        public string SelectedWorkOrderNo
        {
            get { return selectedWorkOrderNo; }
            set { SetProperty(ref selectedWorkOrderNo, value); }
        }

        string selectedParts;
        public string SelectedParts
        {
            get { return selectedParts; }
            set { SetProperty(ref selectedParts, value); }
        }

        string selectedColors;
        public string SelectedColors
        {
            get { return selectedColors; }
            set { SetProperty(ref selectedColors, value); }
        }

        string selectedColorsCode;
        public string SelectedColorsCode
        {
            get { return selectedColorsCode; }
            set { SetProperty(ref selectedColorsCode, value); }
        }

        string selectedIssueToWorkOrderNo;
        public string SelectedIssueToWorkOrderNo
        {
            get { return selectedIssueToWorkOrderNo; }
            set { SetProperty(ref selectedIssueToWorkOrderNo, value); }
        }

        string selectedIssueToLineNo;
        public string SelectedIssueToLineNo
        {
            get { return selectedIssueToLineNo; }
            set { SetProperty(ref selectedIssueToLineNo, value); }
        }
        string selectedSizes;
        public string SelectedSizes
        {
            get { return selectedSizes; }
            set { SetProperty(ref selectedSizes, value); }
        }
        ProcessDataDto processDatadto;
        public ProcessDataDto ProcessDatadto
        {
            get { return processDatadto; }
            set { SetProperty(ref processDatadto, value); }
        }


        public ProcessPageViewModel()
        {
            InitilizeData();
        }

        public void InitilizeData()
        {
            CloseCommand1 = new BaseCommand((object obj) => CloseCommand1Clicked());
            DropDownCommand = new BaseCommand((object obj) => DropDownCommandClickedAsync(obj));
            IssueQtyCommand = new BaseCommand((object obj) => IssueQtyCommandClickedAsync());
            
            transactionService = ServiceLocator.Resolve<ITransactionService>();
            SelectedParts = string.Empty;
            selectedColors = string.Empty;
            SelectedProductionOrderNo = string.Empty;
            SelectedWorkOrderNo = string.Empty;
            SelectedIssueToWorkOrderNo = string.Empty;
            SelectedIssueToLineNo = string.Empty;
            SelectedColorsCode = string.Empty;
 
        }

       

        private async Task DropDownCommandClickedAsync(object sender)
        {
            var existRecords = await transactionService.GetAllPoAsync();

            string selectedValue;
           
            switch (sender)
            {

                case "ProductionOrderNo":
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel" ,null,null, processModel.ProductionOrderNo.ToArray());
                    SelectedProductionOrderNo = selectedValue.Equals("Cancel") ? SelectedProductionOrderNo : selectedValue;
                    break;

                case "WorkOrderNo":
                    var workOrderNoList = existRecords.Where(x => x.SoNo == SelectedProductionOrderNo).Select(x=> x.PoNo).Distinct().ToList();
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, workOrderNoList.ToArray());
                    SelectedWorkOrderNo = selectedValue.Equals("Cancel") ? SelectedWorkOrderNo : selectedValue;
                    var userService = ServiceLocator.Resolve<IUserWebService>();
                    var result = await userService.GetIssueToWOData<PONoDto>(new PostQCInputDto()
                    {
                        PONo = selectedWorkOrderNo,
                        SONo = SelectedProductionOrderNo
                    });
                    processModel.IssueToWorkOrderNo = result.PONo;
                    processModel.IssueToLineNo = result.Line;
                    break;

                case "Parts":
                    var PartsList = existRecords.Where(x => x.PoNo == selectedWorkOrderNo).Select(x => x.Part).Distinct().ToList();
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, PartsList.ToArray());
                    SelectedParts = selectedValue.Equals("Cancel") ? SelectedParts : selectedValue;
                    break;

                case "Colors":

                    var Colours = existRecords.Where(x => x.PoNo == selectedWorkOrderNo).Select(x => new { x.Color, x.Hexcode,x.SizeList }).Distinct().ToList();
                    foreach (var c in Colours)
                        if (!processModel.Colors.ContainsKey(c.Color))
                            processModel.Colors.Add(c.Color, Color.FromHex(c.Hexcode).ToHex());
                    //var ColorsList = existRecords.Where(x => x.PoNo == selectedWorkOrderNo).Select(x => x.Color).Distinct().ToList();
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, processModel.Colors.Keys.ToArray());
                    SelectedColors = selectedValue.Equals("Cancel") ? SelectedColors : selectedValue;
                    SelectedColorsCode = selectedValue.Equals("Cancel") ? SelectedColorsCode : processModel.Colors.FirstOrDefault(x => x.Key == selectedValue).Value;
                    //SelectedSizes = Colours.Where(x => x.Color == SelectedColors).Select(x=> x.SizeList).FirstOrDefault();
                    BindGridData();
                    break;

                case "IssueToWorkOrderNo":
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, processModel.IssueToWorkOrderNo.ToArray());
                    SelectedIssueToWorkOrderNo = selectedValue.Equals("Cancel") ? SelectedIssueToWorkOrderNo : selectedValue;
                    break;

                case "IssuetToLineNo":                    
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, processModel.IssueToLineNo.ToArray());
                    SelectedIssueToLineNo = selectedValue.Equals("Cancel") ? SelectedIssueToLineNo : selectedValue;
                    break;
            }
                   
        }

        private async Task IssueQtyCommandClickedAsync()
        {
            bool isConfirmed = await UserDialogs.Instance.ConfirmAsync("Are you sure you want to Submit?", "Confirmation", "Yes", "No");

            if (isConfirmed)
            {
                var dataset = processDatadto.data;
                List<OrderIssue> orderIssueData = new List<OrderIssue>();
                foreach (var onerow in dataset)
                {
                    if (onerow.issueColorQty > 0)
                    {
                        OrderIssue obj = new OrderIssue();
                        obj.SONo = SelectedProductionOrderNo;
                        obj.PONo = SelectedWorkOrderNo;
                        obj.Part = selectedParts;
                        obj.Color = onerow.color;
                        obj.Size = onerow.size;
                        obj.Qty = onerow.issueColorQty;
                        obj.TabletID = Preferences.Get("TabletID", string.Empty);
                        obj.WFXColorCode = onerow.wfxColorCode;
                        obj.WFXColorName = onerow.wfxColorName;
                        //obj.Line = Sele
                        obj.ToPONo = SelectedIssueToWorkOrderNo;
                        obj.ToLine = SelectedIssueToLineNo;
                        orderIssueData.Add(obj);
                    }

                }
                if (orderIssueData.Count() > 0)
                {
                    
                    var userService = ServiceLocator.Resolve<IUserWebService>();
                    var result = await userService.PostOrderIssue<BaseResponseDto>(orderIssueData);

                    if (result.status == System.Net.HttpStatusCode.OK)
                    {
                            MoveToDashBoard();
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Not able to submit data, Please try again later.");
                    }
                }
            }

        }
        protected void MoveToDashBoard()
        {
           
            SelectedParts = string.Empty;
            selectedColors = string.Empty;
            SelectedProductionOrderNo = string.Empty;
            SelectedWorkOrderNo = string.Empty;
            SelectedIssueToWorkOrderNo = string.Empty;
            SelectedIssueToLineNo = string.Empty;
            SelectedColorsCode = string.Empty;
            ProcessDatadto.data.Clear();
            SalesOrdersPageViewModel.ChangeToSoView = true;
            NativeService.NavigationService.SetRootPage("MasterPage");
        }

        public async void BindGridData()
        {
            var userService = ServiceLocator.Resolve<IUserWebService>();
            var result = await userService.GetIssueToNextProcessData<ProcessDataDto>(new PostQCInputDto()
            {
                PONo = selectedWorkOrderNo,
                SONo = SelectedProductionOrderNo,
                Part = selectedParts,
                Color = SelectedColors
                
            });
            ProcessDatadto = result;
            ProcessDatadto.Totalqty = result.data.Sum(x => x.issueColorQty);
        }

        public void OnScreenAppearing()
        {
            Task.Run(async () => { await FillSO(); });
            ProcessModel = new ProcessDataViewModel();
            processModel.Colors = new Dictionary<string, string>();

        }
        private void CloseCommand1Clicked()
        {
            SelectedParts = string.Empty;
            selectedColors = string.Empty;
            SelectedProductionOrderNo = string.Empty;
            SelectedWorkOrderNo = string.Empty;
            SelectedIssueToWorkOrderNo = string.Empty;
            SelectedIssueToLineNo = string.Empty;
            SelectedColorsCode = string.Empty;
            ProcessDatadto.data.Clear();
            NativeService.NavigationService.GoBack();
        }
        public async Task FillSO() {
            if (IsBusy || InternetConnectivity.IsNetworkNotAvailable())
                return;

            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);
                var existRecords = await transactionService.GetAllPoAsync();
                if (existRecords != null)
                {
                    //var POList = existRecords.Where();
                    //var POList1 = existRecords.Where(x => x.);
                    ProcessModel.ProductionOrderNo = existRecords.Select(x => x.SoNo).Distinct().ToList();
                }
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex) {

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}




