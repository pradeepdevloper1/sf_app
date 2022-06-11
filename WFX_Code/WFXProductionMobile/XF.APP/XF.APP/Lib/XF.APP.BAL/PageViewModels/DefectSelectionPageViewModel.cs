using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DefectSelectionPageViewModel : BaseViewModel, IDefectSelectionViewModel
    {
        public ICommand SaveDefectCommand { set; get; }
        public ICommand CloseCommand { get; set; }
        public ICommand MarkOnGarmentCommand { get; set; }
        private List<DefectOutputDto> DefectListOriginal;
        private List<Operation> OperationListOriginal;
        private ObservableCollection<InspDefectViewModel> DefectsOriginal;
        private ObservableCollection<InspDefectViewModel> LocOrOperationsOriginal;
        private bool IsDefect;
        private Color CellBorderColor;
        private Color CellUnselectedTextColor;
        private string CellHeaderTextColor;
        public static bool ReloadData = true;
        public static int CurrentDefectCount = 0;

        #region Bindable Properties

        ObservableCollection<InspDefectViewModel> clothTypes;
        public ObservableCollection<InspDefectViewModel> ClothTypes
        {
            get { return clothTypes; }
            set { SetProperty(ref clothTypes, value); }
        }

        ObservableCollection<InspDefectViewModel> locOrOperations;
        public ObservableCollection<InspDefectViewModel> LocOrOperations
        {
            get { return locOrOperations; }
            set { SetProperty(ref locOrOperations, value); }
        }

        ObservableCollection<InspDefectViewModel> defects;
        public ObservableCollection<InspDefectViewModel> Defects
        {
            get { return defects; }
            set { SetProperty(ref defects, value); }
        }

        PostQCInputDto postQCDto;
        public PostQCInputDto PostQCInput
        {
            get { return postQCDto; }
            set { SetProperty(ref postQCDto, value); }
        }

        bool saveDefectVilibility;
        public bool SaveDefectVilibility
        {
            get { return saveDefectVilibility; }
            set { SetProperty(ref saveDefectVilibility, value); }
        }

        bool locOrOperationsVilibility;
        public bool LocOrOperationsVilibility
        {
            get { return locOrOperationsVilibility; }
            set { SetProperty(ref locOrOperationsVilibility, value); }
        }

        bool locOrOperPlaceholderVilibility;
        public bool LocOrOperPlaceholderVilibility
        {
            get { return locOrOperPlaceholderVilibility; }
            set { SetProperty(ref locOrOperPlaceholderVilibility, value); }
        }

        bool defectsVilibility;
        public bool DefectsVilibility
        {
            get { return defectsVilibility; }
            set { SetProperty(ref defectsVilibility, value); }
        }

        bool defectsPlaceholderVilibility;
        public bool DefectsPlaceholderVilibility
        {
            get { return defectsPlaceholderVilibility; }
            set { SetProperty(ref defectsPlaceholderVilibility, value); }
        }

        bool markOnGarmentViewVilibility;
        public bool MarkOnGarmentViewVilibility
        {
            get { return markOnGarmentViewVilibility; }
            set { SetProperty(ref markOnGarmentViewVilibility, value); }
        }

        int selectedListCellId;
        public int SelectedListCellId
        {
            get { return selectedListCellId; }
            set { SetProperty(ref selectedListCellId, value); }
        }

        Color placeHolderBgColor;
        public Color PlaceHolderBgColor
        {
            get { return placeHolderBgColor; }
            set { SetProperty(ref placeHolderBgColor, value); }
        }

        Color placeHolderTextColor;
        public Color PlaceHolderTextColor
        {
            get { return placeHolderTextColor; }
            set { SetProperty(ref placeHolderTextColor, value); }
        }

        Color placeHolderBorderColor;
        public Color PlaceHolderBorderColor
        {
            get { return placeHolderBorderColor; }
            set { SetProperty(ref placeHolderBorderColor, value); }
        }

        Color corousalViewBtnColor;
        public Color CorousalViewBtnColor
        {
            get { return corousalViewBtnColor; }
            set { SetProperty(ref corousalViewBtnColor, value); }
        }

        string selectedClothType;
        public string SelectedClothType
        {
            get { return selectedClothType; }
            set { SetProperty(ref selectedClothType, value); }
        }

        string selectedOper;
        public string SelectedOper
        {
            get { return selectedOper; }
            set { SetProperty(ref selectedOper, value); }
        }

        string selectedDefects;
        public string SelectedDefects
        {
            get { return selectedDefects; }
            set { SetProperty(ref selectedDefects, value); }
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

        string locOrOperSearchTxt;
        public string LocOrOperSearchTxt
        {
            get { return locOrOperSearchTxt; }
            set
            {
                SetProperty(ref locOrOperSearchTxt, value);
                FilterListData(value, 1);
            }
        }

        string defectsSearchTxt;
        public string DefectsSearchTxt
        {
            get { return defectsSearchTxt; }
            set
            {
                SetProperty(ref defectsSearchTxt, value);
                FilterListData(value, 2);
            }
        }
      
        #endregion

        public DefectSelectionPageViewModel()
        {
            SaveDefectCommand = new BaseCommand(async (object obj) => await SaveDefectCommandClicked());
            MarkOnGarmentCommand = new BaseCommand((object obj) => MarkOnGarmentCommandClicked(obj));
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
        }

        public void OnScreenAppearing()
        {
            if (ReloadData)
            {
                ClothTypes = new ObservableCollection<InspDefectViewModel>();
                LocOrOperations = new ObservableCollection<InspDefectViewModel>();
                Defects = new ObservableCollection<InspDefectViewModel>();
                DefectListOriginal = new List<DefectOutputDto>();
                OperationListOriginal = new List<Operation>();
                LocOrOperPlaceholderVilibility = true;
                DefectsPlaceholderVilibility = true;
                MarkOnGarmentViewVilibility = false;
                SaveDefectVilibility = false;
                IsBusy = false;

                // Set color theme
                IsDefect = PostQCInput.QCStatus == 2;
                PlaceHolderBgColor = (Color)Application.Current.Resources[IsDefect ? "DefectPlaceHolderBgColor" : "RejectPlaceHolderBgColor"];
                PlaceHolderTextColor = (Color)Application.Current.Resources[IsDefect ? "DefectPlaceHolderTextColor" : "PrimaryTextColor"];
                PlaceHolderBorderColor = (Color)Application.Current.Resources[IsDefect ? "DefectPlaceHolderBorderColor" : "RejectPlaceHolderBorderColor"];
                CorousalViewBtnColor = Color.FromHex(IsDefect ? "#FFB300" : "#B65356");
                CellBorderColor = (Color)Application.Current.Resources[IsDefect ? "SelectedDefectBorderColor" : "SelectedRejectBorderColor"];
                CellUnselectedTextColor = (Color)Application.Current.Resources[IsDefect ? "DefectTextColor" : "RejectTextColor"];
                CellHeaderTextColor = IsDefect ? "OutputDefectTextColor" : "OutputRejectedTextColor";
            }
        }

        public async Task<List<PoImageDto>> FetchListData()
        {
            List<PoImageDto> imageUrls = new List<PoImageDto>();
            if (ReloadData)
            {
                try
                {
                    if (InternetConnectivity.IsNetworkNotAvailable()) return null;
                    var userService = ServiceLocator.Resolve<IUserWebService>();
                    DefectListOriginal = await userService.GetDefectListAsync<List<DefectOutputDto>>();
                    var operResp = await userService.PostOperationListAsync<OperationOutputDto>(new PurchaseOrderInputDto { PONo = PostQCInput.PONo });
                    OperationListOriginal = operResp.data;
                    var poImageList = await userService.GetPoListAsync<SoPoOutputDto>(new PoInputDto()
                    {
                        PONo = PostQCInput.PONo,
                        Line = Preferences.Get("LINE_ID", string.Empty)
                    }, Preferences.Get("AUTH_KEY", null));

                    if (poImageList != null && poImageList.imagelist != null)
                        imageUrls = new List<PoImageDto>(poImageList.imagelist.Where(x => x.imagePath.Contains("http")));
 
                    if (DefectListOriginal == null || OperationListOriginal == null)
                    {
                        UserDialogs.Instance.Alert("Not able to fetch list data, Please try again later.");
                    }

                    var clothTypes = DefectListOriginal.Select(d => d.defectType).Distinct();

                    if (clothTypes != null && clothTypes.Count() > 0)
                    {
                        int count = 0;
                        foreach (var cloth in clothTypes)
                        {   
                            ClothTypes.Add(new InspDefectViewModel
                            {
                                Id = count++,
                                HeaderText = cloth,
                                IsSelected = false,
                                Opacity = 1.0,
                                BorderWidth = 1.0,
                                BorderColor = Color.White,
                                BackgroundColor = Color.White,
                                HasShadow = true,
                                HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"]
                            });

                            if (ClothTypes.Count() > 1 && ClothTypes[clothTypes.Count() - 1].HeaderText == cloth)
                            {
                                ClothTypes.RemoveAt(ClothTypes.Count()-1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Logger.AddException(ex);
                }
                finally
                {
                    IsBusy = false;
                }

                ReloadData = false;
            }
            else
                imageUrls = null;

            return imageUrls;
        }

        async private Task SaveDefectCommandClicked()
        {
            var operation = LocOrOperations.Single(l => l.IsSelected);
            var defects = Defects.Where(l => l.IsSelected);
            try
            {
                CurrentDefectCount = 0;
                foreach (var defect in defects)
                {
                    CurrentDefectCount ++;
                    PostQCInput.tbl_QCDefectDetails.Add(new TblQCDefectDetail
                    {
                        DefectType = ClothTypes.Single(c => c.IsSelected).HeaderText,
                        DefectName = $"{defect.SubText} | {defect.HeaderText}",
                        OperationName = $"{operation.SubText} | {operation.HeaderText}"
                    });
                }
                await NativeService.NavigationService.NavigateAsync("DefectCardPage", PostQCInput);
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
            OutputPageViewModel._Frametapped = false;
        }

        private void MarkOnGarmentCommandClicked(object sender)
        {
            MarkOnGarmentViewVilibility = Convert.ToInt32(sender) == 0;
        }

        private void FilterListData(string searchText, int typeId)
        {
            if (searchText == null) return;

            searchText = searchText.ToLower();

            if (searchText == "")
            {
                if (typeId == 1)
                    LocOrOperations = LocOrOperationsOriginal;
                else if (typeId == 2)
                    Defects = DefectsOriginal;
            }
            else if (typeId == 1 && LocOrOperationsOriginal != null)
            {
                LocOrOperations = new ObservableCollection<InspDefectViewModel>(LocOrOperationsOriginal.Where(x => x.HeaderText.ToLower().Contains(searchText) ||
                x.SubText.ToLower().Contains(searchText)));
            }
            else if (typeId == 2 && DefectsOriginal != null)
            {
                Defects = new ObservableCollection<InspDefectViewModel>(DefectsOriginal.Where(x => x.HeaderText.ToLower().Contains(searchText) ||
                x.SubText.ToLower().Contains(searchText)));
            }
        }

        private void CloseCommandClicked()
        {
            OutputPageViewModel._Frametapped = false;
            NativeService.NavigationService.GoBack();
        }

        private void ModifyCellItem(InspDefectViewModel selectedItem)
        {
            InspDefectViewModel item = SelectedItem as InspDefectViewModel;
            bool isSelected = !item.IsSelected;

            switch (SelectedListCellId)
            {
                case 1: // Select Type
                    if (ClothTypes.Any(t => t.IsSelected) && isSelected)
                    {
                        item.IsSelected = false;
                        return;
                    }

                    var tempClothTypes = ClothTypes.Select(clothType =>
                    {
                        if (clothType.Id == item.Id)
                        {
                            clothType.IsSelected = isSelected;
                            clothType.BorderColor = clothType.IsSelected ? CellBorderColor : Color.White;
                            clothType.HeaderTextColor = (Color)Application.Current.Resources[clothType.IsSelected ? CellHeaderTextColor : "BaseTextColor"];
                            clothType.BorderWidth = clothType.IsSelected ? 2.0 : 1.0;

                            if (isSelected)
                                SelectedClothType = clothType.HeaderText.ToUpper();
                        }
                        return clothType;
                    });

                    // Blur unselected cloth type cells
                    if (tempClothTypes.Any(t => t.IsSelected))
                    {
                        ClothTypes = new ObservableCollection<InspDefectViewModel>(tempClothTypes.Select(clothType =>
                        {
                            if (clothType.Id != item.Id)
                            {
                                clothType.IsSelected = false;
                                clothType.Opacity = 0.2;
                                clothType.BorderWidth = 1.0;
                                clothType.BorderColor = PlaceHolderBorderColor;
                                clothType.BackgroundColor = CellBorderColor;
                                clothType.HeaderTextColor = CellUnselectedTextColor;
                            }

                            return clothType;
                        }));
                    }
                    else
                    {
                        ClothTypes = new ObservableCollection<InspDefectViewModel>(ClothTypes.Select(clothType =>
                        {
                            clothType.IsSelected = false;
                            clothType.Opacity = 1.0;
                            clothType.BorderWidth = 1.0;
                            clothType.BorderColor = Color.White;
                            clothType.BackgroundColor = Color.White;
                            clothType.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];

                            return clothType;
                        }));
                    }

                    LocOrOperations.Clear();
                    if (isSelected)
                    {
                        if (OperationListOriginal != null && OperationListOriginal.Count > 0)
                        {
                            int counter = 0;
                            foreach (var operation in OperationListOriginal)
                            {
                                LocOrOperations.Add(new InspDefectViewModel
                                {
                                    Id = counter++,
                                    HeaderText = operation.operationName,
                                    SubText = operation.operationCode,
                                    IsSelected = false,
                                    BorderColor = Color.White,
                                    BackgroundColor = Color.White,
                                    HasShadow = true,
                                    HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"],
                                    SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"],
                                    Opacity = 1.0,
                                    BorderWidth = 1.0
                                });
                            }
                        }

                        LocOrOperationsVilibility = LocOrOperations.Count > 0 && isSelected;
                        LocOrOperPlaceholderVilibility = !LocOrOperationsVilibility;

                        // Reset other dependent lists
                        if (LocOrOperPlaceholderVilibility)
                        {
                            DefectsVilibility = false;
                            DefectsPlaceholderVilibility = true;
                            LocOrOperations = new ObservableCollection<InspDefectViewModel>(LocOrOperations.Select(locOrOper =>
                            {
                                locOrOper.IsSelected = false;
                                locOrOper.Opacity = 1.0;
                                locOrOper.BorderWidth = 1.0;
                                locOrOper.BorderColor = Color.White;
                                locOrOper.BackgroundColor = Color.White;
                                locOrOper.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                                locOrOper.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];

                                return locOrOper;
                            }));

                            SaveDefectVilibility = false;
                            Defects = new ObservableCollection<InspDefectViewModel>(Defects.Select(defect =>
                            {
                                defect.IsSelected = false;
                                defect.Opacity = 1.0;
                                defect.BorderWidth = 1.0;
                                defect.BorderColor = Color.White;
                                defect.BackgroundColor = Color.White;
                                defect.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                                defect.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];

                                return defect;
                            }));
                        }
                    }
                    else 
                    {
                        LocOrOperationsVilibility = false;
                        LocOrOperPlaceholderVilibility = !LocOrOperationsVilibility;
                        DefectsVilibility = false;
                        DefectsPlaceholderVilibility = !DefectsVilibility;
                    }
                    // For filter use
                    LocOrOperationsOriginal = LocOrOperations;
                    break;
                case 2: // Select Location/Operation
                    if (LocOrOperationsOriginal.Any(t => t.IsSelected) && isSelected)
                    {
                        item.IsSelected = false;
                        return;
                    }

                    var tempLocOrOperations = LocOrOperationsOriginal.Select(locOrOper =>
                    {
                        if (locOrOper.Id == item.Id)
                        {
                            locOrOper.IsSelected = isSelected;
                            locOrOper.BorderColor = locOrOper.IsSelected ? CellBorderColor : Color.White;
                            locOrOper.HeaderTextColor = (Color)Application.Current.Resources[locOrOper.IsSelected ? CellHeaderTextColor : "BaseTextColor"];
                            locOrOper.SubTextColor = (Color)Application.Current.Resources[locOrOper.IsSelected ? CellHeaderTextColor : "SmallHeaderTextColor"];
                            locOrOper.BorderWidth = locOrOper.IsSelected ? 2.0 : 1.0;

                            if (isSelected)
                                SelectedOper = $"{locOrOper.SubText} | {locOrOper.HeaderText}".ToUpper();
                        }
                        return locOrOper;
                    });

                    // Blur unselected location/operation cells
                    if (tempLocOrOperations.Any(t => t.IsSelected))
                    {
                        LocOrOperationsOriginal = new ObservableCollection<InspDefectViewModel>(tempLocOrOperations.Select(locOrOper =>
                        {
                            if (locOrOper.Id != item.Id)
                            {
                                locOrOper.IsSelected = false;
                                locOrOper.Opacity = 0.2;
                                locOrOper.BorderWidth = 1.0;
                                locOrOper.BorderColor = PlaceHolderBorderColor;
                                locOrOper.BackgroundColor = CellBorderColor;
                                locOrOper.HeaderTextColor = CellUnselectedTextColor;
                                locOrOper.SubTextColor = CellUnselectedTextColor;
                            }

                            return locOrOper;
                        }));
                    }
                    else
                    {
                        LocOrOperationsOriginal = new ObservableCollection<InspDefectViewModel>(LocOrOperationsOriginal.Select(locOrOper =>
                        {

                            locOrOper.IsSelected = false;
                            locOrOper.Opacity = 1.0;
                            locOrOper.BorderWidth = 1.0;
                            locOrOper.BorderColor = Color.White;
                            locOrOper.BackgroundColor = Color.White;
                            locOrOper.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                            locOrOper.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];

                            return locOrOper;
                        }));
                    }

                    Defects.Clear();
                    if (isSelected)
                    {
                        int counter = 0;
                        var clothTypes = DefectListOriginal.Where(d => d.defectType == ClothTypes.First(c => c.IsSelected).HeaderText).Distinct();
                        foreach (var defect in clothTypes)
                        {
                            Defects.Add(new InspDefectViewModel
                            {
                                Id = counter++,
                                HeaderText = defect.defectName,
                                SubText = defect.defectCode,
                                IsSelected = false,
                                BorderColor = Color.White,
                                BackgroundColor = Color.White,
                                HasShadow = true,
                                HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"],
                                SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"],
                                Opacity = 1.0,
                                BorderWidth = 1.0
                            });
                        }
                    }

                    DefectsVilibility = Defects.Count > 0 && isSelected;
                    DefectsPlaceholderVilibility = !DefectsVilibility;

                    // Reset other dependent lists
                    if (DefectsPlaceholderVilibility)
                    {
                        SaveDefectVilibility = false;
                        Defects = new ObservableCollection<InspDefectViewModel>(Defects.Select(defect =>
                        {
                            defect.IsSelected = false;
                            defect.Opacity = 1.0;
                            defect.BorderWidth = 1.0;
                            defect.BorderColor = Color.White;
                            defect.BackgroundColor = Color.White;
                            defect.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                            defect.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];

                            return defect;
                        }));
                    }

                    // For filter use
                    DefectsOriginal = Defects;
                    LocOrOperations = new ObservableCollection<InspDefectViewModel>(LocOrOperationsOriginal.Where(l => LocOrOperations.Any(lo => lo.Id == l.Id)));
                    break;
                case 3: // Defects
                    var tempDefects = DefectsOriginal.Select(defect =>
                    {
                        if (defect.Id == item.Id)
                        {
                            defect.IsSelected = isSelected;
                            defect.BorderColor = defect.IsSelected ? CellBorderColor : Color.White;
                            defect.HeaderTextColor = (Color)Application.Current.Resources[defect.IsSelected ? CellHeaderTextColor : "BaseTextColor"];
                            defect.SubTextColor = (Color)Application.Current.Resources[defect.IsSelected ? CellHeaderTextColor : "SmallHeaderTextColor"];
                            defect.BorderWidth = defect.IsSelected ? 2.0 : 1.0;
                            defect.Opacity = defect.IsSelected ? 0.2 : 1.0;
                        }
                        return defect;
                    });

                    SelectedDefects = string.Empty;
                    foreach (var defect in tempDefects)
                    {
                        SelectedDefects += $"{defect.SubText} | {defect.HeaderText}\n".ToUpper();
                    }

                    // Blur unselected defect cells
                    bool atLeastOne = tempDefects.Any(t => t.IsSelected);
                    DefectsOriginal = new ObservableCollection<InspDefectViewModel>(tempDefects.Select(defect =>
                    {
                        if (!defect.IsSelected)
                        {
                            defect.Opacity = atLeastOne ? 0.2 : 1.0;
                            defect.BorderWidth = atLeastOne ? 1.0 : 2.0;
                            defect.BorderColor = atLeastOne ? PlaceHolderBorderColor : Color.White;
                            defect.BackgroundColor = atLeastOne ? CellBorderColor : Color.White;
                            defect.HeaderTextColor = (Color)Application.Current.Resources["BaseTextColor"];
                            defect.SubTextColor = (Color)Application.Current.Resources["SmallHeaderTextColor"];
                        }
                        else
                            defect.BackgroundColor = atLeastOne ? Color.White : CellBorderColor;

                        return defect;
                    }));

                    SaveDefectVilibility = Defects.Any(s => s.IsSelected);

                    // For filter use
                    Defects = new ObservableCollection<InspDefectViewModel>(DefectsOriginal.Where(d => Defects.Any(deo => deo.Id == d.Id)));
                    break;
            }
        }

        public void ShowLoader()
        {
            Device.BeginInvokeOnMainThread(() => { UserDialogs.Instance.ShowLoading(LoadingText); });
        }

        public void HideLoader()
        {
            Device.BeginInvokeOnMainThread(() => { UserDialogs.Instance.HideLoading(); });
        }
    }
}
