using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.APP.BAL
{
    public class DropDownValue
    {
        public string ValueToDisplay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string shiftName { get; set; }
        public int SelectedValue { get; set; }
    }

    public class InspectionPageViewModel : NavigationSearchPageViewModel, IInspectionViewModel
    {
        public ICommand StartInspectionCommand { set; get; }
  
        public static int PassQtyCount;
        public static int DefectQtyCount;
        public static int RejectQtyCount;
        public static int ShiftDefectQtyCount;
        public static DateTime WorkStartDt;
        public static List<Shift> shiftDetails = new List<Shift>();

        private List<PurchaseOrderDto> allPoRecords;

        private ITransactionService transactionService;
        private InspDataViewModel inspData;

        #region Bindable Properties

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

        bool startInspectionVilibility;
        public bool StartInspectionVilibility
        {
            get { return startInspectionVilibility; }
            set { SetProperty(ref startInspectionVilibility, value); }
        }
        bool coloursVilibility;
        public bool ColoursVilibility
        {
            get { return coloursVilibility; }
            set { SetProperty(ref coloursVilibility, value); }
        }

        bool coloursPlaceholderVilibility;
        public bool ColoursPlaceholderVilibility
        {
            get { return coloursPlaceholderVilibility; }
            set { SetProperty(ref coloursPlaceholderVilibility, value); }
        }

        bool sizeVilibility;
        public bool SizeVilibility
        {
            get { return sizeVilibility; }
            set { SetProperty(ref sizeVilibility, value); }
        }

        bool sizePlaceholderVilibility;
        public bool SizePlaceholderVilibility
        {
            get { return sizePlaceholderVilibility; }
            set { SetProperty(ref sizePlaceholderVilibility, value); }
        }
        int selectedListCellId;
        public int SelectedListCellId
        {
            get { return selectedListCellId; }
            set { SetProperty(ref selectedListCellId, value); }
        }

        int piecesQty;
        public int PiecesQty
        {
            get { return piecesQty; }
            set { SetProperty(ref piecesQty, value); }
        }

        int coloursQty;
        public int ColoursQty
        {
            get { return coloursQty; }
            set { SetProperty(ref coloursQty, value); }
        }

        int sizesQty;
        public int SizesQty
        {
            get { return sizesQty; }
            set { SetProperty(ref sizesQty, value); }
        }
        int selectedValue;
        public int SelectedValue
        {
            get { return selectedValue; }
            set { SetProperty(ref selectedValue, value); }

        }
        
        public ObservableCollection<DropDownValue> DropdownValue
        {
            get
            {
                var ShiftDetails = new ObservableCollection<DropDownValue>();
                var userService = ServiceLocator.Resolve<IUserWebService>();
                TimeSpan TimeDiff = new TimeSpan();
                TimeSpan currenttime = DateTime.Now.TimeOfDay;
                var result = Task.Run(async () => await userService.GetShiftAsync<ShiftOutputDto>()).Result;
                shiftDetails = result.data.ToList();
                selectedValue = 0;
                if (shiftDetails != null && shiftDetails.Count > 0)
                {
                    ShiftDetails.Add(new DropDownValue { ValueToDisplay = "Select Shift" });
                    for (int i = 0; i < shiftDetails.Count; i++)
                    {
                        var shiftName = shiftDetails[i].shiftName;
                        TimeSpan shiftstartTime = shiftDetails[i].shiftStartTime;
                        TimeSpan shiftendtime = shiftDetails[i].shiftEndTime;

                        var showtime = shiftName + Environment.NewLine + '(' + DateTime.Today.Add(shiftstartTime).ToString("hh:mm tt") + '-' + DateTime.Today.Add(shiftendtime).ToString("hh:mm tt") + ')';
                        ShiftDetails.Add(new DropDownValue
                        {
                            ValueToDisplay = showtime,
                            StartTime = shiftstartTime,
                            EndTime = shiftendtime,
                            shiftName = shiftName,
                            SelectedValue = selectedValue,
                        });
                        TimeDiff = shiftstartTime - currenttime;

                        if (currenttime > shiftstartTime && TimeDiff.TotalMinutes < 240.00 && currenttime < shiftendtime )
                        {
                            SelectedValue = i + 1;
                        }
                    }
                }
                return ShiftDetails;
            }
        }
        PurchaseOrder1 selectedPo;
        public PurchaseOrder1 SelectedPo
        {
            get { return selectedPo; }
            set { SetProperty(ref selectedPo, value); }
        }

        InspDefectViewModel selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value as InspDefectViewModel);

                if (selectedItem != null)
                    ModifyCellItem(selectedItem);
            }
        }

        ObservableCollection<InspDefectViewModel> parts;
        public ObservableCollection<InspDefectViewModel> Parts
        {
            get { return parts; }
            set { SetProperty(ref parts, value); }
        }

        ObservableCollection<InspDefectViewModel> colours;
        public ObservableCollection<InspDefectViewModel> Colours
        {
            get { return colours; }
            set { SetProperty(ref colours, value); }
        }

        ObservableCollection<InspDefectViewModel> sizes;
        public ObservableCollection<InspDefectViewModel> Sizes
        {
            get { return sizes; }
            set { SetProperty(ref sizes, value); }
        }

        #endregion

        public InspectionPageViewModel()
        {
            StartInspectionCommand = new BaseCommand(async (object obj) => await StartInspectionCommandClicked());
            SearchCommand = new BaseCommand((object obj) => SearchCommandClicked(obj as string));
            BackButtonCommand = new BaseCommand((object obj) => BackButtonCommandClicked());
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
        }

        public void OnScreenAppearing()
        {
            Parts = new ObservableCollection<InspDefectViewModel>();
            Colours = new ObservableCollection<InspDefectViewModel>();
            Sizes = new ObservableCollection<InspDefectViewModel>();
            inspData = new InspDataViewModel();
            //shiftDetails = new List<Shift>();
            PassQtyCount = 0;
            DefectQtyCount = 0;
            RejectQtyCount = 0;
            ShiftDefectQtyCount = 0;
            ColoursPlaceholderVilibility = true;
            SizePlaceholderVilibility = true;
            ColoursVilibility = false;
            SizeVilibility = false;
            StartInspectionVilibility = false;
            IsSearchBarVisible = false;
            IsBusy = false;
            int count = 0;

            Parts.Clear();
            Task.Run(async () =>
            {
                transactionService = ServiceLocator.Resolve<ITransactionService>();
                var allRecords = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo);
                allPoRecords = allRecords.ToList();

                SizesQty = SelectedPo.sizeList.Split(',').Count();
                ColoursQty = allRecords.Select(x => x.Color).Distinct().Count();
                PiecesQty = allRecords.Select(x => x.PoQty).Distinct().Sum();

                foreach (var p in allRecords.Select(x => x.Part).Distinct())
                {
                    Parts.Add(new InspDefectViewModel
                    {
                        Id = count++,
                        HeaderText = p,
                        SubText = "Garment",
                        IsSelected = false,
                        Opacity = 1.0,
                        BorderColor = Color.White,
                        BackgroundColor = Color.White,
                        HasShadow = true,
                        HeaderTextColor = (Color)Application.Current.Resources["SecondaryTextColor"],
                        SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"]
                    });
                }
            });
        }

        async private Task StartInspectionCommandClicked()
        {
            if (InternetConnectivity.IsNetworkNotAvailable()) return;

            try
            {
                string shiftName = Preferences.Get("Shift", "");
                if (shiftName == "")
                {
                    UserDialogs.Instance.Alert("Please Select Shift DropDown.");
                    return;
                }
                if (!(shiftDetails != null && shiftDetails.Count > 0))
                {
                    UserDialogs.Instance.ShowLoading(LoadingText);

                    var userService = ServiceLocator.Resolve<IUserWebService>();
                    var result = await userService.GetShiftAsync<ShiftOutputDto>();

                    UserDialogs.Instance.HideLoading();
                    

                    if (result != null && result.status == System.Net.HttpStatusCode.OK)
                    {
                        shiftDetails = result.data;
                        GoToOutputScreen();
                    }
                    else
                        UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
                }
                else
                    GoToOutputScreen();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Logger.AddException(ex);
                UserDialogs.Instance.Alert("Something went wrong, Please try again later.");
            }
        }

        private async void GoToOutputScreen()
        {
            WorkStartDt = DateTime.Now;

            inspData.SelectedPo = SelectedPo;
            inspData.Parts = Parts.Select(p => p.HeaderText);
            inspData.Sizes = Sizes.Select(s => s.HeaderText);
            inspData.Colors = new Dictionary<string, string>();
            foreach (var c in Colours)
                if (!inspData.Colors.ContainsKey(c.HeaderText))
                    inspData.Colors.Add(c.HeaderText, c.BoxColor.ToHex());
                await NativeService.NavigationService.NavigateAsync("OutputPage", inspData);

        }

        private void BackButtonCommandClicked()
        {
            NativeService.NavigationService.GoBack();
        }


        private void SearchCommandClicked(string searchText)
        {
            IsSearchBarVisible = !IsSearchBarVisible;

            FilterListData(searchText);
        }

        private void CloseCommandClicked()
        {
            IsSearchBarVisible = !IsSearchBarVisible;
            SearchText = string.Empty;
        }

        private void FilterListData(string searchText)
        {
            if (searchText == null) return;
            else if (searchText == "")
            {
            }
            else
            {
            }
        }

        private async void ModifyCellItem(InspDefectViewModel selectedItem)
        {
            InspDefectViewModel item = SelectedItem as InspDefectViewModel;
            bool isSelected = !item.IsSelected;

            switch (SelectedListCellId)
            {
                case 1: // Parts
                    Parts = new ObservableCollection<InspDefectViewModel>(Parts.Select(part =>
                    {
                        if (part.HeaderText == item.HeaderText)
                        {
                            part.IsSelected = isSelected;
                            part.BorderColor = part.IsSelected ? (Color)Application.Current.Resources["SelectedCardViewBorderColor"] : Color.White;

                            if (part.IsSelected)
                                inspData.SelectedPart = part.HeaderText;
                        }
                        return part;
                    }));

                    Colours.Clear();
                    if (isSelected)
                    {
                        int count = 0;
                        var poRecords = await transactionService.GetByIdPoAsync(SelectedPo.soNo, SelectedPo.poNo);
                        allPoRecords = poRecords.ToList();

                        foreach (var p in poRecords.Distinct())
                        {
                            if (Colours.Any(c => c.HeaderText == p.Color)) continue;
                            Colours.Add(new InspDefectViewModel
                            {
                                Id = count++,
                                HeaderText = p.Color,
                                SubText = p.Fabric,
                                IsSelected = false,
                                HasShadow = true,
                                Opacity = 1.0,
                                BorderColor = Color.White,
                                BackgroundColor = Color.White,
                                BoxColor = Color.FromHex(p.Hexcode),
                                HeaderTextColor = (Color)Application.Current.Resources["SecondaryTextColor"],
                                SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"]
                            });
                        }
                    }
                    ColoursVilibility = Colours.Count > 0 && isSelected;
                    ColoursPlaceholderVilibility = !ColoursVilibility;

                    // Reset other dependent lists
                    if (ColoursPlaceholderVilibility)
                    {
                        SizeVilibility = false;
                        SizePlaceholderVilibility = true;
                        Colours = new ObservableCollection<InspDefectViewModel>(Colours.Select(colour =>
                        {
                            colour.IsSelected = false;
                            colour.BorderColor = Color.White;
                            return colour;
                        }));

                        StartInspectionVilibility = false;
                        Sizes = new ObservableCollection<InspDefectViewModel>(Sizes.Select(size =>
                        {
                            size.IsSelected = false;
                            size.BorderColor = Color.White;
                            size.Opacity = 1.0;

                            return size;
                        }));
                    }
                    break;
                case 2: // Colours
                    if (Colours.Any(t => t.IsSelected) && isSelected)
                    {
                        item.IsSelected = false;
                        return;
                    }

                    var tempColours = Colours.Select(color =>
                    {
                        if (color.Id == item.Id)
                        {
                            color.IsSelected = isSelected;
                            color.BorderColor = color.IsSelected ? (Color)Application.Current.Resources["SelectedCardViewBorderColor"] : Color.White;
                            color.BorderWidth = color.IsSelected ? 2.0 : 1.0;
                        }
                        return color;
                    });

                    // Blur unselected location/operation cells
                    if (tempColours.Any(t => t.IsSelected))
                    {
                        Colours = new ObservableCollection<InspDefectViewModel>(tempColours.Select(color =>
                        {
                            if (color.Id != item.Id)
                            {
                                color.IsSelected = false;
                                color.Opacity = 0.2;
                                color.BorderWidth = 1.0;
                                color.BorderColor = Color.White;
                                color.BackgroundColor = Color.White;
                                color.HeaderTextColor = (Color)Application.Current.Resources["SelectedCardViewBorderColor"];
                                color.SubTextColor = (Color)Application.Current.Resources["SelectedCardViewBorderColor"];
                            }

                            return color;
                        }));
                    }
                    else
                    {
                        Colours = new ObservableCollection<InspDefectViewModel>(Colours.Select(color =>
                        {

                            color.IsSelected = false;
                            color.Opacity = 1.0;
                            color.BorderWidth = 1.0;
                            color.BorderColor = Color.White;
                            color.BackgroundColor = Color.White;
                            color.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                            color.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];

                            return color;
                        }));
                    }

                    //set selected colors
                    var selectedColor = Colours.FirstOrDefault(s => s.IsSelected);
                    if (selectedColor != null)
                    {
                        inspData.SelectedColor = selectedColor.HeaderText;
                        inspData.SelectedHexCode = selectedColor.BoxColor.ToHex().Remove(1, 2);
                    }
                    else
                    {
                        inspData.SelectedColor = null;
                        inspData.SelectedHexCode = null;
                    }

                    Sizes.Clear();
                    if (isSelected)
                    {
                        // Recreate SelecterdPo object for selected color
                        var po = allPoRecords.Where(p => p.Color == item.HeaderText).FirstOrDefault();
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

                        int count = 0;
                        foreach (var size in SelectedPo.sizeList.Split(','))
                        {
                            Sizes.Add(new InspDefectViewModel
                            {
                                Id = count++,
                                HeaderText = size,
                                IsSelected = false,
                                HasShadow = true,
                                Opacity = 1.0,
                                BorderColor = Color.White,
                                BackgroundColor = Color.White,
                                HeaderTextColor = (Color)Application.Current.Resources["SecondaryTextColor"],
                            });
                        }
                    }
                    SizeVilibility = Sizes.Count > 0 && isSelected;
                    SizePlaceholderVilibility = !SizeVilibility;

                    // Reset other dependent lists
                    if (SizePlaceholderVilibility)
                    {
                        StartInspectionVilibility = false;
                        Sizes = new ObservableCollection<InspDefectViewModel>(Sizes.Select(size =>
                        {
                            size.IsSelected = false;
                            size.BorderColor = Color.White;
                            size.Opacity = 1.0;

                            return size;
                        }));
                    }
                    break;
                case 3: // Sizes
                    var tempSizes = Sizes.Select(size =>
                    {              
                        if (size.HeaderText == item.HeaderText)
                        {
                            size.IsSelected = isSelected;
                            size.BorderColor = size.IsSelected ? (Color)Application.Current.Resources["SelectedCardViewBorderColor"] : Color.White;
                            size.Opacity = 1.0;

                            if (size.IsSelected)
                                inspData.SelectedSize = size.HeaderText;
                        }
                        return size;
                    });

                    // Blur unselected size cells
                    if (tempSizes.Any(t => t.IsSelected))
                    {
                        Sizes = new ObservableCollection<InspDefectViewModel>(tempSizes.Select(size =>
                        {
                            size.BackgroundColor = size.IsSelected ? Color.White : (Color)Application.Current.Resources["InspPlaceHolderBgColor"];
                            size.BorderColor = (Color)Application.Current.Resources[size.IsSelected ? "SelectedCardViewBorderColor" : "InspPlaceHolderBgColor"];
                            size.Opacity = size.IsSelected ? 1.0 : 0.2;

                            return size;
                        }));
                    }
                    else
                    {
                        Sizes = new ObservableCollection<InspDefectViewModel>(tempSizes.Select(size =>
                        {
                            size.BackgroundColor = Color.White;
                            size.BorderColor = Color.White;
                            size.Opacity = 1.0;

                            return size;
                        }));
                    }
                    StartInspectionVilibility = Sizes.Any(s => s.IsSelected);
                    break;
            }
        }
    }
}
