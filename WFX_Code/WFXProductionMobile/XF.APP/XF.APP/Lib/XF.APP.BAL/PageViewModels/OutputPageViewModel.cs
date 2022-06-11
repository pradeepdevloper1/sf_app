using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Android.Content;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;


namespace XF.APP.BAL
{
    public class OutputPageViewModel : NavigationSearchPageViewModel, IOutputViewModel
    {

        public ICommand DropDownCommand { set; get; }
        public ICommand Undo5Command { set; get; }
        public ICommand PauseCommand { set; get; }
        public ICommand CompleteCommand { set; get; }
        public ICommand MultipleAddCommand { set; get; }
        public ICommand SwitchChangedCommand { set; get; }
        public ICommand AddCommand { set; get; }
        public ICommand ResumeCommand { set; get; }

        private string BtnPauseText;
        private string BtnResumeText;
        private bool isTargetDataLoaded = false;
        private ITransactionService transactionService;

        public static List<KeyValuePair<string, string>> postQCQueue;
        public static List<PurchaseOrderShift> POShiftData;

        public KeyValuePair<string, string> lastData = new KeyValuePair<string, string>();
        public static string markPoints = string.Empty;

        public static bool _Frametapped = false;



        #region Bindable Properties

        string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value); }
        }

        int switchIndex;
        public int SwitchIndex
        {
            get { return switchIndex; }
            set { SetProperty(ref switchIndex, value); }
        }

        PurchaseOrder1 selectedPo;
        public PurchaseOrder1 SelectedPo
        {
            get { return selectedPo; }
            set
            {
                SetProperty(ref selectedPo, value);


                _ = Task.Run(async () =>
                {


                    PassCount = InspectionPageViewModel.PassQtyCount == 0 ? PassCount : InspectionPageViewModel.PassQtyCount;
                    DefectCount = InspectionPageViewModel.DefectQtyCount;
                    RejectCount = InspectionPageViewModel.RejectQtyCount;

                    await PostDailyActivity(1, 0);
                    setDefectCount();
                });
            }
        }

        InspDataViewModel inspData;
        public InspDataViewModel InspData
        {
            get { return inspData; }
            set { SetProperty(ref inspData, value); }
        }

        string selectedPart;
        public string SelectedPart
        {
            get { return selectedPart; }
            set { SetProperty(ref selectedPart, value); }
        }

        string selectedColour;
        public string SelectedColour
        {
            get { return selectedColour; }
            set { SetProperty(ref selectedColour, value); }
        }

        string selectedColourCode;
        public string SelectedColourCode
        {
            get { return selectedColourCode; }
            set { SetProperty(ref selectedColourCode, value); }
        }

        string selectedSize;
        public string SelectedSize
        {
            get { return selectedSize; }
            set { SetProperty(ref selectedSize, value); }
        }

        int passCount;
        public int PassCount
        {
            get { return passCount; }
            set { SetProperty(ref passCount, value); }
        }

        int defectCount;
        public int DefectCount
        {
            get { return defectCount; }
            set { SetProperty(ref defectCount, value); }
        }

        int rejectCount;
        public int RejectCount
        {
            get { return rejectCount; }
            set { SetProperty(ref rejectCount, value); }
        }

        int targetPiecesCount;
        public int TargetPiecesCount
        {
            get { return targetPiecesCount; }
            set { SetProperty(ref targetPiecesCount, value); }
        }

        int plannedEfficiency;
        public int PlannedEfficiency
        {
            get { return plannedEfficiency; }
            set { SetProperty(ref plannedEfficiency, value); }
        }

        string targetEfficiencyPercentage;
        public string TargetEfficiencyPercentage
        {
            get { return targetEfficiencyPercentage; }
            set { SetProperty(ref targetEfficiencyPercentage, value); }
        }

        int actualPiecesCount;
        public int ActualPiecesCount
        {
            get { return actualPiecesCount; }
            set { SetProperty(ref actualPiecesCount, value); }
        }

        string actualEfficiencyPercentage;
        public string ActualEfficiencyPercentage
        {
            get { return actualEfficiencyPercentage; }
            set { SetProperty(ref actualEfficiencyPercentage, value); }
        }

        int passedShiftCount;
        public int PassedShiftCount
        {
            get { return passedShiftCount; }
            set { SetProperty(ref passedShiftCount, value); }
        }

        int passedCumulative;
        public int PassedCumulative
        {
            get { return passedCumulative; }
            set { SetProperty(ref passedCumulative, value); }
        }

        int defectShiftCount;
        public int DefectShiftCount
        {
            get { return defectShiftCount; }
            set { SetProperty(ref defectShiftCount, value); }
        }

        int defectCumulative;
        public int DefectCumulative
        {
            get { return defectCumulative; }
            set { SetProperty(ref defectCumulative, value); }
        }

        int rejectedShiftCount;
        public int RejectedShiftCount
        {
            get { return rejectedShiftCount; }
            set { SetProperty(ref rejectedShiftCount, value); }
        }

        int rejectedCumulative;
        public int RejectedCumulative
        {
            get { return rejectedCumulative; }
            set { SetProperty(ref rejectedCumulative, value); }
        }

        string defectRateShiftPercentage;
        public string DefectRateShiftPercentage
        {
            get { return defectRateShiftPercentage; }
            set { SetProperty(ref defectRateShiftPercentage, value); }
        }

        string averagePercentage;
        public string AveragePercentage
        {
            get { return averagePercentage; }
            set { SetProperty(ref averagePercentage, value); }
        }

        string btnStopPause;
        public string BtnStopPause
        {
            get { return btnStopPause; }
            set { SetProperty(ref btnStopPause, value); }
        }

        bool startPauseViewVisibility;
        public bool StartPauseViewVisibility
        {
            get { return startPauseViewVisibility; }
            set { SetProperty(ref startPauseViewVisibility, value); }
        }

        bool pausedVisibility;
        public bool PausedVisibility
        {
            get { return pausedVisibility; }
            set { SetProperty(ref pausedVisibility, value); }
        }

        bool quePromptViewVisibility;
        public bool QuePromptViewVisibility
        {
            get { return quePromptViewVisibility; }
            set { SetProperty(ref quePromptViewVisibility, value); }
        }

        StartPauseViewModel selectedPauseReason;
        public object SelectedPauseReason
        {
            get { return selectedPauseReason; }
            set
            {
                SetProperty(ref selectedPauseReason, value as StartPauseViewModel);
                if (selectedPauseReason != null)
                    ModifyCellItem(selectedPauseReason);
            }
        }

        ObservableCollection<StartPauseViewModel> startPauseList;
        public ObservableCollection<StartPauseViewModel> StartPauseList
        {
            get { return startPauseList; }
            set { SetProperty(ref startPauseList, value); }
        }

        #endregion
        string shiftname;
        public OutputPageViewModel()
        {
            InitilizeData();
        }

        public void InitilizeData()
        {
            isTargetDataLoaded = false;
            SearchCommand = new BaseCommand((object obj) => SearchCommandClicked(obj as string));
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
            BackButtonCommand = new BaseCommand((object obj) => BackButtonCommandClicked());
            DropDownCommand = new BaseCommand((object obj) => DropDownCommandClicked(obj));
            Undo5Command = new BaseCommand((object obj) => Undo5CommandClicked());
            PauseCommand = new BaseCommand((object obj) => PauseCommandClicked());
            CompleteCommand = new BaseCommand((object obj) => CompleteCommandClicked(obj));
            MultipleAddCommand = new BaseCommand((object obj) => MultipleAddCommandClicked(obj));
            SwitchChangedCommand = new BaseCommand((object obj) => SwitchChangedCommandClicked(obj));
            AddCommand = new BaseCommand((object obj) => AddCommandClicked(obj));
            ResumeCommand = new BaseCommand((object obj) => ResumeCommandClicked());

            postQCQueue = new List<KeyValuePair<string, string>>();
            transactionService = ServiceLocator.Resolve<ITransactionService>();
            // Assign test data
            SelectedPart = string.Empty;
            SelectedColour = string.Empty;
            SelectedColourCode = string.Empty;
            TargetEfficiencyPercentage = "0%";
            PlannedEfficiency = 0;
            ActualPiecesCount = 0;
            ActualEfficiencyPercentage = "0%";
            PassedShiftCount = 0;
            PassedCumulative = 0;
            DefectShiftCount = 0;
            DefectCumulative = 0;
            RejectedShiftCount = 0;
            RejectedCumulative = 0;
            DefectRateShiftPercentage = "0%";
            AveragePercentage = "0%";
            PassCount = 0;
            RejectCount = 0;
            DefectCount = 0;
            shiftname = Preferences.Get("Shift", string.Empty);
            POShiftData = new List<PurchaseOrderShift>();
        }

        public async void OnScreenAppearing(string arg1, string arg2, Dictionary<string, string> pauseReasonList, string matchPoints)
        {
            if (DefectCount != InspectionPageViewModel.DefectQtyCount || RejectCount != InspectionPageViewModel.RejectQtyCount || PassCount != InspectionPageViewModel.PassQtyCount)
            {
                var existingRecord = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo, InspData.SelectedColor, InspData.SelectedHexCode, shiftname);

                if (existingRecord != null)
                {
                    if (!string.IsNullOrEmpty(matchPoints))
                        existingRecord.MarkPoints += $"{(string.IsNullOrEmpty(existingRecord.MarkPoints) ? "" : "$$")}{matchPoints}";

                    await transactionService.SaveUpdatePoAsync(existingRecord);
                }
                isTargetDataLoaded = true;
            }
            setDefectCount();
            if (isTargetDataLoaded)
                LoadLineTargetData();

            BtnPauseText = arg1;
            BtnResumeText = arg2;
            BtnStopPause = BtnPauseText;

            StartPauseList = new ObservableCollection<StartPauseViewModel>();
            QuePromptViewVisibility = false;
            StartPauseViewVisibility = false;
            PausedVisibility = false;
            int count = 0;

            foreach (var pauseReason in pauseReasonList)
            {
                StartPauseList.Add(new StartPauseViewModel
                {
                    Id = count++,
                    Opacity = 1.0,
                    IsSelected = false,
                    Title = pauseReason.Key,
                    Image = ImageSource.FromFile(pauseReason.Value),
                    BorderColor = (Color)Application.Current.Resources["BaseTextColor"]
                });
            }
        }

        public int IsDefectOrRejectChanged()
        {
            int qcStatus = 0;

            if (DefectCount != InspectionPageViewModel.DefectQtyCount)
                qcStatus = 2;
            else if (RejectCount != InspectionPageViewModel.RejectQtyCount)
                qcStatus = 3;

            return qcStatus;
        }

        #region Command Events

        private async void ResumeCommandClicked(bool hideView = true)
        {
            if (InternetConnectivity.IsNetworkNotAvailable()) return;
            isTargetDataLoaded = false;
            if (StartPauseList.Any(p => p.IsSelected))
            {
                if (BtnStopPause.Equals(BtnPauseText))
                {
                    BtnStopPause = BtnResumeText;
                    await PostDailyActivity(1, 0);
                }
                else
                {
                    BtnStopPause = BtnPauseText;
                    await PostDailyActivity(0, 1);
                }

                if (hideView)
                {
                    StartPauseViewVisibility = false;
                    PausedVisibility = false;

                    // Reset list data
                    StartPauseList = new ObservableCollection<StartPauseViewModel>(StartPauseList.Select(item => {
                        item.Opacity = 1.0;
                        item.IsSelected = false;
                        item.BorderColor = (Color)Application.Current.Resources["BaseTextColor"];

                        return item;
                    }));
                }
            }
        }

        private async void AddCommandClicked(object sender)
        {          
            string shift = shiftname;

            int defectType = 1;

            switch (Convert.ToInt32(sender))
            {
                case 1: // PASS
                    string sizeName = CommonMethods.GetSizeName(SelectedSize);
                    int SizeQty = CommonMethods.GetSizeQty(SelectedSize);
                    var userService = ServiceLocator.Resolve<IUserWebService>();
                    var result = await userService.GetTotalQCQuantity<PostQCOutputDto>(new PostQCInputDto()
                    {
                        PONo = SelectedPo.poNo,
                        Color = SelectedColour,
                        Size = sizeName,
                        FactoryID = SelectedPo.factoryID,
                        Module = SelectedPo.module,
                        QCStatus = 1
                    });
                    if (result != null)
                    {
                        int passedQty = Convert.ToInt32(result.qty) + 1;
                        if (SizeQty < passedQty)
                        {
                            UserDialogs.Instance.Alert("Pass Quantity " + passedQty + " Cannnot be Greater than " + SizeQty);
                            return;
                        }
                    }

                    if (SwitchIndex == 1 && InspectionPageViewModel.DefectQtyCount == 0)
                    {
                        await UserDialogs.Instance.AlertAsync("There is no rework.");
                        return;
                    }
                    int masterId = await PostQC(1);
                    if (masterId >= 0)
                    {
                        var existingRecord1 = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo, InspData.SelectedColor, InspData.SelectedHexCode, shift);
                        existingRecord1.PassQty = existingRecord1.PassQty + 1;

                        await transactionService.SaveUpdatePoAsync(existingRecord1);

                        POShiftData.Add(new PurchaseOrderShift
                        {
                            soNo = existingRecord1.SoNo,
                            poNo = existingRecord1.PoNo,
                            color = SelectedColour,
                            Size = SelectedSize.Split('-')[0],
                            DefectType = 1,
                            Qty = 1,
                            TypeofWork = SwitchIndex + 1
                        });

                        // Update in db
                        if (SwitchIndex == 1)
                        {
                            if (InspectionPageViewModel.DefectQtyCount > 0)
                                InspectionPageViewModel.DefectQtyCount--;
                            if (DefectCount > 0)
                                DefectCount--;
                            defectType = 2;
                        }
                        postQCQueue.Add(new KeyValuePair<string, string>($"1#{masterId}#1", $"{SwitchIndex}#{defectType}"));
                        PassCount++;
                        InspectionPageViewModel.PassQtyCount++;
                        LoadLineTargetData();
                    }
                    break;
                case 2: // DEFECT
                case 3: // REJECT
                    if (SwitchIndex == 1 && InspectionPageViewModel.DefectQtyCount == 0)
                    {
                        await UserDialogs.Instance.AlertAsync("There is no rework.");
                        return;
                    }
                    DefectSelectionPageViewModel.ReloadData = true;
                    DefectCardPageViewModel.SelectedDefects = new List<DefectCardListViewModel>();
                    var existingRecord = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo, InspData.SelectedColor, InspData.SelectedHexCode, shift);
                    // Get all marks for current PO
                    markPoints = (existingRecord != null && existingRecord.MarkPoints != null) ? existingRecord.MarkPoints : "";
                    if (!_Frametapped)
                    {
                        _Frametapped = true;
                        await NativeService.NavigationService.NavigateAsync("DefectSelectionPage", new PostQCInputDto()
                        {
                            SONo = SelectedPo.soNo,
                            TypeOfWork = SwitchIndex + 1,
                            PONo = SelectedPo.poNo,
                            Color = SelectedColour,
                            HexCode = SelectedPo.hexcode,
                            Part = SelectedPart,
                            Size = CommonMethods.GetSizeName(SelectedSize),
                            Qty = 1,
                            QCStatus = Convert.ToInt32(sender),
                            UserID = UserID,
                            FactoryID = SelectedPo.factoryID,
                            Line = SelectedPo.line,
                            Module = SelectedPo.module,
                            ShiftName = shiftname,
                            WFXColorCode = selectedPo.WFXColorCode,
                            WFXColorName = selectedPo.WFXColorName
                            //,
                            //ProcessCode = SelectedPo.ProcessCode
                        });
                    }
                    break;
            }
        }

        private void SwitchChangedCommandClicked(object sender)
        {
            if (Convert.ToInt32(sender) == 1 && InspectionPageViewModel.DefectQtyCount == 0)
                return;

            SwitchIndex = Convert.ToInt32(sender);
        }

        private void MultipleAddCommandClicked(object sender)
        {
            if (Convert.ToInt32(sender) == 1)
            {
                NativeService.NavigationService.NavigateAsync("BulkOutputPage", new PostQCInputDto()
                {
                    SONo = SelectedPo.soNo,
                    TypeOfWork = SwitchIndex + 1,
                    PONo = SelectedPo.poNo,
                    Color = SelectedColour,
                    Part = SelectedPart,
                    Size = CommonMethods.GetSizeName(SelectedSize),
                    UserID = UserID,
                    FactoryID = SelectedPo.factoryID,
                    Line = SelectedPo.line,
                    Module = SelectedPo.module,
                    QCStatus = Convert.ToInt32(sender),
                    Qty = CommonMethods.GetSizeQty(SelectedSize),
                    WFXColorCode = selectedPo.WFXColorCode,
                    WFXColorName = selectedPo.WFXColorName,
                 //   ProcessCode = SelectedPo.ProcessCode,
                    tbl_QCDefectDetails = new List<TblQCDefectDetail>()
                });
            }
        }

        private async void CompleteCommandClicked(object obj)
        {
            if (Convert.ToInt32(obj) == 0)
            {
                QuePromptViewVisibility = true;
                StartPauseViewVisibility = false;
            }
            else if (Convert.ToInt32(obj) == 1)
            {
                UserDialogs.Instance.ShowLoading(LoadingText);
                var userService = ServiceLocator.Resolve<IUserWebService>();
                {
                    var result = await userService.PostEndShiftAsync<BaseResponseDto>(new PostQCInputDto()
                    {
                        FactoryID = selectedPo.factoryID,
                        PONo = SelectedPo.poNo,
                        SONo = SelectedPo.soNo,
                        Qty = InspectionPageViewModel.PassQtyCount + InspectionPageViewModel.DefectQtyCount
                    });
                }


                UserDialogs.Instance.HideLoading();
                bool res = await PostDailyActivity(0, 0);
                QuePromptViewVisibility = false;

                await UserDialogs.Instance.AlertAsync(res ? "Shift ended successfully." : "Fail to end shift");
                if (res)
                    ClearData();
                switchIndex=0;
            }
            else
                QuePromptViewVisibility = false;
        }

        private void PauseCommandClicked()
        {
            QuePromptViewVisibility = false;
            StartPauseViewVisibility = true;
        }

        private async void Undo5CommandClicked()
        {
            if (postQCQueue.Count > 0 && (PassCount > 0 || DefectCount > 0 || RejectCount > 0 || postQCQueue.Last().Key.Split('#')[1] == "Colour" || postQCQueue.Last().Key.Split('#')[1] == "Size"))
            {
                try
                {
                    if (InternetConnectivity.IsNetworkNotAvailable()) return;

                   // UserDialogs.Instance.ShowLoading(LoadingText);

                    // Queue bucket
                    var undoData = postQCQueue.Last();
                    var arr = undoData.Key.Split('#');

                    if (arr[1] == "Colour" || arr[1] == "Size")
                    {
                        string[] undoValue = undoData.Value.Split('#');

                        DropDownCommandClicked(arr[1], "onUndo", undoValue[1]);
                        //UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        postQCQueue.RemoveAt(postQCQueue.Count - 1);
                        POShiftData.RemoveAt(POShiftData.Count - 1);

                        int masterId = Convert.ToInt32(arr[1]);

                        //web service call to validate user login
                        var userService = ServiceLocator.Resolve<IUserWebService>();
                        var result = await userService.PostUndoAsync<BaseResponseDto>(new UndoInputDto()
                        {
                            QCMasterId = masterId
                        });

                        UserDialogs.Instance.HideLoading();
                        var existingRecord = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo, InspData.SelectedColor, InspData.SelectedHexCode, ABSTRACTION.CommonMethods.GetCurrentShift(InspectionPageViewModel.shiftDetails, SelectedPo.factoryID));
                        if (result.status == System.Net.HttpStatusCode.OK)
                        {
                            int qcStatus = Convert.ToInt32(arr[0]);
                            int qty = Convert.ToInt32(arr[2]);

                            switch (qcStatus)
                            {
                                case 1:
                                    InspectionPageViewModel.PassQtyCount -= qty;
                                    InspectionPageViewModel.PassQtyCount = InspectionPageViewModel.PassQtyCount < 0 ? 0 : InspectionPageViewModel.PassQtyCount;
                                    PassCount -= qty;
                                    PassCount = PassCount < 0 ? 0 : PassCount;
                                    if (existingRecord != null)
                                    {
                                        existingRecord.PassQty -= qty;
                                    }
                                    showToastMessage(SelectedColour + " " + SelectedSize.Split('-')[0] + "  " + qty + "  Pass Pieces Undo done.");
                                    break;
                                case 2:
                                    InspectionPageViewModel.DefectQtyCount -= qty;
                                    InspectionPageViewModel.ShiftDefectQtyCount -= qty;
                                    InspectionPageViewModel.DefectQtyCount = InspectionPageViewModel.DefectQtyCount < 0 ? 0 : InspectionPageViewModel.DefectQtyCount;
                                    InspectionPageViewModel.ShiftDefectQtyCount = InspectionPageViewModel.ShiftDefectQtyCount < 0 ? 0 : InspectionPageViewModel.ShiftDefectQtyCount;
                                    DefectCount -= qty;
                                    DefectCount = DefectCount < 0 ? 0 : DefectCount;
                                    if (existingRecord != null)
                                    {
                                        existingRecord.DefectQty -= qty;
                                        var markpointArr = existingRecord.MarkPoints.Split(new[] { "$$" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        markpointArr.RemoveAt(markpointArr.Count - 1);
                                        string mpoints = "";
                                        foreach (var mp in markpointArr)
                                        {
                                            mpoints += $"{(string.IsNullOrEmpty(mpoints) ? "" : "$$")}{mp}";
                                        }
                                        existingRecord.MarkPoints = mpoints;
                                    }
                                    showToastMessage(SelectedColour + " " + SelectedSize.Split('-')[0] + "  " + qty + "  Defect Pieces Undo done.");

                                    break;
                                case 3:
                                    InspectionPageViewModel.RejectQtyCount -= qty;
                                    InspectionPageViewModel.RejectQtyCount = InspectionPageViewModel.RejectQtyCount < 0 ? 0 : InspectionPageViewModel.RejectQtyCount;
                                    RejectCount -= qty;
                                    RejectCount = RejectCount < 0 ? 0 : RejectCount;
                                    if (existingRecord != null)
                                    {
                                        existingRecord.RejectQty -= qty;
                                        var markpointArr = existingRecord.MarkPoints.Split(new[] { "$$" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        markpointArr.RemoveAt(markpointArr.Count - 1);
                                        string mpoints = "";
                                        foreach (var mp in markpointArr)
                                        {
                                            mpoints += $"{(string.IsNullOrEmpty(mpoints) ? "" : "$$")}{mp}";
                                        }
                                        existingRecord.MarkPoints = mpoints;
                                    }
                                    showToastMessage(SelectedColour + " " + SelectedSize.Split('-')[0] + " " + qty + " Reject Pieces Undo done.");

                                    break;
                            }

                            // Reset rework item
                            if (postQCQueue.Any())
                            {

                                lastData = postQCQueue.Last();
                            }
                            int qtyLast = Convert.ToInt32(undoData.Key.Split('#')[2]);

                            var arrValue = undoData.Value.Split('#');
                            int typeOfWork = Convert.ToInt32(arrValue[1]);

                            {
                                if (lastData.Key != null && lastData.Key.Count() > 0)
                                    SwitchIndex = Convert.ToInt32(lastData.Value.Split('#')[0]);
                                if (typeOfWork != qcStatus)
                                {
                                    switch (typeOfWork)
                                    {
                                        case 1:
                                            InspectionPageViewModel.PassQtyCount += qtyLast;
                                            InspectionPageViewModel.PassQtyCount = InspectionPageViewModel.PassQtyCount < 0 ? 0 : InspectionPageViewModel.PassQtyCount;
                                            PassCount += qtyLast;
                                            PassCount = PassCount < 0 ? 0 : PassCount;
                                            if (existingRecord != null)
                                            {
                                                existingRecord.PassQty += qtyLast;
                                            }

                                            break;
                                        case 2:
                                            InspectionPageViewModel.DefectQtyCount += qtyLast;
                                            InspectionPageViewModel.DefectQtyCount = InspectionPageViewModel.DefectQtyCount < 0 ? 0 : InspectionPageViewModel.DefectQtyCount;
                                            DefectCount += qtyLast;
                                            DefectCount = DefectCount < 0 ? 0 : DefectCount;

                                            break;
                                        case 3:
                                            InspectionPageViewModel.RejectQtyCount += qtyLast;
                                            InspectionPageViewModel.RejectQtyCount = InspectionPageViewModel.RejectQtyCount < 0 ? 0 : InspectionPageViewModel.RejectQtyCount;
                                            RejectCount += qtyLast;
                                            if (existingRecord != null)
                                            {
                                                existingRecord.RejectQty += qtyLast;
                                            }
                                            RejectCount = RejectCount < 0 ? 0 : RejectCount;

                                            break;
                                    }
                                }
                            }
                            
                            if (existingRecord != null)
                            {
                                await transactionService.SaveUpdatePoAsync(existingRecord);
                            }
                            setDefectCount();
                            LoadLineTargetData();
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Logger.AddException(ex);
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
                }
            }
            else
                await UserDialogs.Instance.AlertAsync("No work to undo or not allowed to undo old work.");
        }

        private void showToastMessage(string message)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
            }
        }
        private async void DropDownCommandClicked(object sender, string calledfrom = "onDropdownChange", string undoValue = "")
        {
            var existingRecord = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo, InspData.SelectedColor, InspData.SelectedHexCode, shiftname);

            string selectedValue;
            switch (sender)
            {
                case "Part":
                    selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, InspData.Parts.ToArray());
                    SelectedPart = selectedValue.Equals("Cancel") ? SelectedPart : selectedValue;
                    break;
                case "Colour":
                    var allPoRecords = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo);
                    
                    if (calledfrom == "onUndo")
                    {
                        SelectedColour = undoValue.Equals("Cancel") ? SelectedColour : undoValue;
                        SelectedColourCode = undoValue.Equals("Cancel") ? SelectedColourCode : InspData.Colors.FirstOrDefault(x => x.Key == undoValue).Value;
                        postQCQueue.RemoveAt(postQCQueue.Count - 1);
               
                    }
                    else
                    {
                        selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, InspData.Colors.Keys.ToArray());
                        if (selectedValue == "Cancel") {
                            return;
                        }
                        postQCQueue.Add(new KeyValuePair<string, string>($"1#Size#1", $"{SwitchIndex}#{SelectedSize}"));
                        postQCQueue.Add(new KeyValuePair<string, string>($"1#Colour#1", $"{SwitchIndex}#{selectedColour}"));
                        SelectedColour = selectedValue.Equals("Cancel") ? SelectedColour : selectedValue;
                        SelectedColourCode = selectedValue.Equals("Cancel") ? SelectedColourCode : InspData.Colors.FirstOrDefault(x => x.Key == selectedValue).Value;
                        // Recreate 'SelecterdPo' & 'InspData.Sizes' for selected color
                    }
                    var po = allPoRecords.Where(p => p.Color == SelectedColour).FirstOrDefault();
                    isTargetDataLoaded = false;
                    SelectedPo = new PurchaseOrder1
                    {
                        orderID = po.OrderID,
                        module = po.Module,
                        soNo = po.SoNo,
                        poNo = po.PoNo,
                        style = po.Style,
                        fit = po.Fit,
                        product = po.Product,
                        season = po.Season,
                        customer = po.Customer,
                        planStDt = po.PlanStDt,
                        exFactory = po.ExFactory,
                        primaryPart = po.PrimaryPart,
                        part = po.Part,
                        color = po.Color,
                        hexcode = po.Hexcode,
                        fabric = po.Fabric,
                        orderRemark = po.OrderRemark,
                        isSizeRun = po.IsSizeRun,
                        poQty = SelectedPo.poQty,
                        sizeList = po.SizeList,
                        orderStatus = po.OrderStatus,
                        userID = po.UserID,
                        factoryID = po.FactoryID,
                        entryDate = po.EntryDate,
                        line = SelectedPo.line,
                        totalColors = SelectedPo.totalColors,
                        completedColors = SelectedPo.completedColors,
                        completedQty = SelectedPo.completedQty,
                        colorsGraphProgress = SelectedPo.colorsGraphProgress,
                        qtyGraphProgress = SelectedPo.qtyGraphProgress,
                        WFXColorCode = po.WFXColorCode,
                        WFXColorName = po.WFXColorName
                        //,
                        //ProcessCode = po.ProcessCode,
                        //ProcessName = po.ProcessName,
                        //FulfillmentType = po.FulfillmentType
                    };
                    InspData.SelectedColor = SelectedColour;
                    InspData.SelectedHexCode = SelectedColourCode;
                    InspData.SelectedPo = SelectedPo;
                    InspData.Sizes = po.SizeList.Split(',');
                    if (calledfrom == "onUndo") {
                        var undoData = postQCQueue.Last();
                        var arr = undoData.Key.Split('#');

                        if (arr[1] == "Size")
                        {
                            string[] undoValue1 = undoData.Value.Split('#');

                            DropDownCommandClicked(arr[1], "onUndo", undoValue1[1]);
                            InspData.SelectedSize = undoValue1[1];
                        }

                        //SelectedSize = undoValue.Equals("Cancel") ? SelectedSize : undoValue;
                        //postQCQueue.RemoveAt(postQCQueue.Count - 1);
                        showToastMessage("Back to " + SelectedColour + " " + SelectedSize.Split('-')[0]);
                        //UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        InspData.SelectedSize = InspData.Sizes.First();
                        SelectedSize = InspData.SelectedSize;
                       
                    }
                    setDefectCount();
                    break;
                case "Size":
                    if (calledfrom == "onUndo")
                    {
                        SelectedSize = undoValue.Equals("Cancel") ? SelectedSize : undoValue;
                        postQCQueue.RemoveAt(postQCQueue.Count - 1);
                        showToastMessage("Back to " + SelectedColour + " " + SelectedSize.Split('-')[0]);
                        //UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        selectedValue = await UserDialogs.Instance.ActionSheetAsync($"Choose {sender}", "Cancel", null, null, InspData.Sizes.ToArray());
                        if (selectedValue == "Cancel")
                        {
                            return;
                        }
                        postQCQueue.Add(new KeyValuePair<string, string>($"1#Size#1", $"{SwitchIndex}#{SelectedSize}"));
                        SelectedSize = selectedValue.Equals("Cancel") ? SelectedSize : selectedValue;
                    }
                    setDefectCount();
                    break;
            }
        }

        private async void setDefectCount()
        {
            string sizeName = CommonMethods.GetSizeName(SelectedSize);
            var userService = ServiceLocator.Resolve<IUserWebService>();
            var result = await userService.GetTotalQCQuantity<PostQCOutputDto>(new PostQCInputDto()
            {
                PONo = SelectedPo.poNo,
                Color = InspData.SelectedColor,
                Size = sizeName,
                FactoryID = SelectedPo.factoryID,
                Module = SelectedPo.module,
                Line = Preferences.Get("LINE_ID", string.Empty),
                QCStatus = 2

            });
            if (result != null)
            {
                DefectCount = result.qty;
                DefectCount = DefectCount < 0 ? 0 : DefectCount;
                InspectionPageViewModel.DefectQtyCount = DefectCount;
            }
            PassCount = POShiftData.Where(x => x.color == SelectedColour && x.Size == SelectedSize.Split('-')[0] && x.DefectType == 1).Sum(x => x.Qty);
            PassCount = PassCount < 0 ? 0 : PassCount;
            InspectionPageViewModel.PassQtyCount = PassCount;
            RejectCount = POShiftData.Where(x => x.color == SelectedColour && x.Size == SelectedSize.Split('-')[0] && x.DefectType == 3).Count();
            RejectCount = RejectCount < 0 ? 0 : RejectCount;
            InspectionPageViewModel.RejectQtyCount = RejectCount;
        }

        

        private void BackButtonCommandClicked()
        {
            if (PassCount == 0 && DefectCount == 0 && RejectCount == 0)
                NativeService.NavigationService.GoBack();
            else
                //UserDialogs.Instance.AlertAsync("Back event not allowed.");
                NativeService.NavigationService.GoBack();
        }

        private void SearchCommandClicked(string searchText)
        {
            IsSearchBarVisible = !IsSearchBarVisible;
        }

        private void CloseCommandClicked()
        {
            IsSearchBarVisible = !IsSearchBarVisible;
        }

        #endregion
        private async Task<int> PostQC(int qCStatus)
        {
            int returnValue = -1;
            if (InternetConnectivity.IsNetworkNotAvailable()) return returnValue;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);

                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();

                var result = await userService.PostQCAsync<PostQCOutputDto>(new PostQCInputDto()
                {
                    TypeOfWork = SwitchIndex + 1,
                    PONo = SelectedPo.poNo,
                    Color = SelectedColour,
                    Part = SelectedPart,
                    Size = CommonMethods.GetSizeName(SelectedSize),
                    Qty = 1,
                    QCStatus = qCStatus,
                    UserID = UserID,
                    FactoryID = SelectedPo.factoryID,
                    Line = SelectedPo.line,
                    Module = SelectedPo.module,
                    ShiftName = shiftname,
                    WFXColorCode = selectedPo.WFXColorCode,
                    WFXColorName = selectedPo.WFXColorName,
                   // ProcessCode = selectedPo.ProcessCode,
                    TabletID = Preferences.Get("TabletID", string.Empty),
                    tbl_QCDefectDetails = new List<TblQCDefectDetail>()
                });

                UserDialogs.Instance.HideLoading();

                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    returnValue = result.id;
                }
                else
                {
                    UserDialogs.Instance.Alert("Not able to submit data, Please try again later.");
                }
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
            }
            return returnValue;
        }

        private async Task<bool> PostDailyActivity(int isActive, int isPause)
        {
            bool IsSuccess = false;

            if (InternetConnectivity.IsNetworkNotAvailable()) return IsSuccess;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);

                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();
                var result = await userService.PostDailyActivityAsync<BaseResponseDto>(new DailyActivityInputDto()
                {
                    ActivitDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    UserID = UserID,
                    LastUpdatedDateTime = InspectionPageViewModel.WorkStartDt.ToString("yyyy-MM-dd"),
                    Seconds = (DateTime.Now - InspectionPageViewModel.WorkStartDt).Seconds,
                    IsActive = isActive,
                    IsPause = isPause,
                    PONo = SelectedPo.poNo,
                    LineName = LineMan,
                    OverTime = 0,
                    FirstLogin = DateTime.Now.ToString("yyyy-MM-dd"),
                    LastLogOut = DateTime.Now.ToString("yyyy-MM-dd"),
                    Manhrs = 3
                });

                ShiftTargetOutputDto result1 = null;

                if (!isTargetDataLoaded)
                {
                    result1 = await userService.PostShiftTargetAsync<ShiftTargetOutputDto>(new ShiftTargetInputDto
                    {
                        Line = string.IsNullOrEmpty(SelectedPo.line) ? "" : SelectedPo.line,
                        PONo = SelectedPo.poNo,
                        ShiftName = shiftname
                    });
                }

                UserDialogs.Instance.HideLoading();

                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    IsSuccess = true;
                    InspectionPageViewModel.WorkStartDt = DateTime.Now;
                }

                var shiftTarget = new ShiftTarget();
                if (result1 != null && result1.data != null && result1.data.Count > 0)
                {
                    for (var i = 0; i < result1.data.Count; i++)
                    {
                        shiftTarget.plannedEffeciency += result1.data[i].plannedEffeciency;
                        shiftTarget.plannedTarget += result1.data[i].plannedTarget;
                    }
                    shiftTarget.plannedEffeciency = (int)Math.Ceiling((double)shiftTarget.plannedEffeciency / result1.data.Count);
                }
                SelectedPart = InspData.SelectedPart;
                SelectedColour = InspData.SelectedColor;
                SelectedColourCode = InspData.SelectedHexCode;
                SelectedSize = InspData.SelectedSize;
                TargetPiecesCount = shiftTarget?.plannedTarget ?? 0;
                PlannedEfficiency = shiftTarget?.plannedEffeciency ?? 0;
                LoadLineTargetData();
                isTargetDataLoaded = true;
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
                UserDialogs.Instance.HideLoading();

                UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
            }
            return IsSuccess;
        }

        private void ModifyCellItem(StartPauseViewModel selectedPauseReason)
        {
            if (InternetConnectivity.IsNetworkNotAvailable() || PausedVisibility) return;

            bool isSelected = !selectedPauseReason.IsSelected;

            if (StartPauseList.Any(t => t.IsSelected) && isSelected)
            {
                selectedPauseReason.IsSelected = false;
                return;
            }

            var tempStartPauseList = StartPauseList.Select(item => {
                if (item.Id == selectedPauseReason.Id)
                {
                    item.IsSelected = isSelected;
                    item.BorderColor = (Color)Application.Current.Resources[item.IsSelected ? "SelectedRejectBorderColor" : "BaseTextColor"];
                    PausedVisibility = item.IsSelected;
                }
                return item;
            });

            // Blur unselected cloth type cells
            bool atLeastOne = tempStartPauseList.Any(t => t.IsSelected);
            StartPauseList = new ObservableCollection<StartPauseViewModel>(tempStartPauseList.Select(item =>
            {
                if (item.Id != selectedPauseReason.Id)
                {
                    item.IsSelected = false;
                    item.Opacity = atLeastOne ? 0.2 : 1.0;
                }
                return item;
            }));

            if (PausedVisibility && atLeastOne)
                ResumeCommandClicked(false);
        }

        private async void LoadLineTargetData()
        {
            var existingRecord = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo);
            ActualPiecesCount = POShiftData.Where(x => x.DefectType == 1).Sum(x => x.Qty) +
                InspectionPageViewModel.ShiftDefectQtyCount + POShiftData.Where(x => x.DefectType == 3).Sum(x => x.Qty)
                - POShiftData.Where(x => x.TypeofWork == 2).Sum(x => x.Qty);
            ActualEfficiencyPercentage = TargetPiecesCount == 0 ? "0%" : $"{Math.Ceiling((double)POShiftData.Where(x => x.DefectType == 1).Sum(x => x.Qty) * ((double)PlannedEfficiency / (double)TargetPiecesCount))}%";
            PassedShiftCount = POShiftData.Where(x => x.DefectType == 1).Sum(x => x.Qty);;
            PassedCumulative = existingRecord.Select(x => x.PassQty).Sum();
            DefectShiftCount = InspectionPageViewModel.ShiftDefectQtyCount;
            DefectCumulative = existingRecord.Select(x => x.DefectQty).Sum();
            RejectedShiftCount = POShiftData.Where(x => x.DefectType == 3).Sum(x => x.Qty);
            RejectedCumulative = existingRecord.Select(x => x.RejectQty).Sum();
            DefectRateShiftPercentage = RejectCount == 0 || ActualPiecesCount == 0 ? "0%" : $"{Math.Round((double)RejectCount / ActualPiecesCount * 100, 1)}%";
            TargetEfficiencyPercentage = $"{PlannedEfficiency}%";
            AveragePercentage = DefectRateShiftPercentage;
        }
    }
}
