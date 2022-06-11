using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using XF.APP.ApiService;
namespace XF.APP.BAL
{
    public class DefectCardPageViewModel : BaseViewModel, IDefectCardViewModel
    {
        public ICommand CloseCommand { get; set; }
        private ITransactionService transactionService;
        public static List<DefectCardListViewModel> SelectedDefects = new List<DefectCardListViewModel>();

        ObservableCollection<DefectCardListViewModel> defects;
        public ObservableCollection<DefectCardListViewModel> Defects
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

        string selectedClothType;
        public string SelectedClothType
        {
            get { return selectedClothType; }
            set { SetProperty(ref selectedClothType, value); }
        }

        Color corousalViewBtnColor;
        public Color CorousalViewBtnColor
        {
            get { return corousalViewBtnColor; }
            set { SetProperty(ref corousalViewBtnColor, value); }
        }

        public DefectCardPageViewModel()
        {
            transactionService = ServiceLocator.Resolve<ITransactionService>();
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
        }

        public void OnScreenAppearing()
        {
            string defectText = string.Empty;
            var count = DefectSelectionPageViewModel.CurrentDefectCount;
            int lastIndex = PostQCInput.tbl_QCDefectDetails.Count - 1;
            //defectText = PostQCInput.tbl_QCDefectDetails[lastIndex].DefectName;
            var defect = PostQCInput.tbl_QCDefectDetails[lastIndex];
            var fromIndex = lastIndex - count + 1;
            var data = PostQCInput.tbl_QCDefectDetails.ToList().GetRange(fromIndex, count);
            foreach (var d in data)
                defectText += d.DefectName + "\n";

            bool isPresent = SelectedDefects.Any(d => d.TypeText == defect.DefectType.ToUpper() && d.LocOrOperText == defect.OperationName.ToUpper() && d.DefectListText == defectText.ToUpper());
            if (!isPresent)
            {
                SelectedDefects.Add(new DefectCardListViewModel
                {
                    Id = SelectedDefects.Count,
                    HasShadow = true,
                    TypeText = defect.DefectType.ToUpper(),
                    LocOrOperText = defect.OperationName.ToUpper(),
                    DefectListText = defectText.ToUpper(),
                    Opacity = 0.4
                });
            }

            SelectedDefects[0].IsSelected = true;
            SelectedDefects[0].Opacity = 1.0;
            SelectedClothType = SelectedDefects[0].TypeText;
            CorousalViewBtnColor = Color.FromHex(PostQCInput.QCStatus == 2 ? "#FFB300" : "#B65356");

            Defects = new ObservableCollection<DefectCardListViewModel>(SelectedDefects);
            Defects.Add(new DefectCardListViewModel
            {
                Id = Defects.Count,
                IsAddCell = true,
                BackgroundColor = (Color)Application.Current.Resources[PostQCInput.QCStatus == 2 ? "DefectPlaceHolderBgColor" : "RejectPlaceHolderBgColor"]
            });
        }

        public void SubmitDefect(Dictionary<string, byte[]> imageData)
        {
            if (InternetConnectivity.IsNetworkNotAvailable())
                return;
            _ = SubmitCommandClicked(imageData);
        }

        async private Task SubmitCommandClicked(Dictionary<string, byte[]> imageData)
        {
            try
            {
                PostQCInput.TabletID =Preferences.Get("TabletID", string.Empty);
                var userService = ServiceLocator.Resolve<IUserWebService>();
                var result = await userService.PostQCAsync<PostQCOutputDto>(PostQCInput);

                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    var existingRecord = await transactionService.GetByIdPoAsync(PostQCInput.SONo, PostQCInput.PONo, PostQCInput.Color, PostQCInput.HexCode, PostQCInput.ShiftName);
                    foreach (var img in imageData)
                    {
                        var res = await UploadFile(img.Value, img.Key.Split(new string[] { "##" }, StringSplitOptions.None)[1], result);
                    }
                    UserDialogs.Instance.HideLoading();

                    if ((PostQCInput.QCStatus == 3 || PostQCInput.QCStatus == 2) && PostQCInput.TypeOfWork == 2)
                        OutputPageViewModel.postQCQueue.Add(new KeyValuePair<string, string>($"{PostQCInput.QCStatus}#{result.id}#{PostQCInput.Qty}", $"{PostQCInput.TypeOfWork - 1}#{PostQCInput.QCStatus - 1}"));
                    //else if (PostQCInput.QCStatus == 2 && PostQCInput.TypeOfWork == 2) { }
                    else
                        OutputPageViewModel.postQCQueue.Add(new KeyValuePair<string, string>($"{PostQCInput.QCStatus}#{result.id}#{PostQCInput.Qty}", $"{PostQCInput.TypeOfWork - 1}#{PostQCInput.QCStatus}"));

                    if (PostQCInput.QCStatus == 2)
                    {
                        // Avoid increment for rework of defect item
                        //if (PostQCInput.TypeOfWork == 1)
                        //{
                            InspectionPageViewModel.DefectQtyCount++;
                            InspectionPageViewModel.ShiftDefectQtyCount++;
                            existingRecord.DefectQty++;
                            OutputPageViewModel.POShiftData.Add(new PurchaseOrderShift
                            {
                                soNo = PostQCInput.SONo,
                                poNo = PostQCInput.PONo,
                                color = PostQCInput.Color,
                                Size = PostQCInput.Size,
                                DefectType = 2,
                                Qty = 1,
                                TypeofWork = PostQCInput.TypeOfWork
                            });
                        //}
                    }
                    else
                    {
                        if (PostQCInput.TypeOfWork == 2)
                            InspectionPageViewModel.DefectQtyCount--;
                        InspectionPageViewModel.RejectQtyCount++;
                        existingRecord.RejectQty++;
                        OutputPageViewModel.POShiftData.Add(new PurchaseOrderShift
                        {
                            soNo = PostQCInput.SONo,
                            poNo = PostQCInput.PONo,
                            color = PostQCInput.Color,
                            Size = PostQCInput.Size,
                            DefectType = 3,
                            Qty = 1,
                            TypeofWork = PostQCInput.TypeOfWork
                        });
                    }
                    await transactionService.SaveUpdatePoAsync(existingRecord);
                }
                // Redirect to result screen
                await NativeService.NavigationService.NavigateAsync("ResultMsgPage", result.status == System.Net.HttpStatusCode.OK ? 1 : 2);
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
        }

        public async Task<bool> UploadFile(byte[] imageData, string imageName, PostQCOutputDto result)
        {
            //variable
            var url = new Uri(string.Format(ApiService.Constants.BaseUrl, $"Image/UploadPOShiftImages?pono={PostQCInput.PONo}"));
            try
            {
                //create new HttpClient and MultipartFormDataContent and add our file, and StudentId
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("AUTH_KEY", null));
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(imageData);
                content.Add(baContent, "files", imageName);

                //upload MultipartFormDataContent content async and store response in response var
                var response = await client.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //read response result as a string async into json var
                    var responsestr = response.Content.ReadAsStringAsync().Result;

                    //debug
                    Debug.WriteLine($"{imageName} Uploaded: {responsestr}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                //debug
                Debug.WriteLine("Exception Caught: " + e.ToString());
                return false;
            }
        }

        private async void CloseCommandClicked()
        {
            //await NativeService.NavigationService.GoBack();
            await NativeService.NavigationService.RemoveNPagesFromStack(2);
        }

        public bool ModifyCellItem(object selectedItem)
        {
            var item = selectedItem as DefectCardListViewModel;

            bool isSelectionChanged = false;
            bool isSelected = !item.IsSelected;

            if (item.IsAddCell)
            {
                DefectSelectionPageViewModel.ReloadData = true;
                NativeService.NavigationService.GoBack();
            }

            Defects = new ObservableCollection<DefectCardListViewModel>(Defects.Select(defect =>
            {
                if (defect.Id == item.Id)
                {
                    defect.IsSelected = isSelected;
                    defect.Opacity = 1.0;

                    if (isSelected)
                    {
                        isSelectionChanged = !SelectedClothType.Equals(defect.TypeText);
                        SelectedClothType = defect.TypeText;
                    }
                }
                else
                {
                    defect.IsSelected = false;
                    defect.Opacity = 0.2;
                }
                return defect;
            }));

            return isSelectionChanged;
        }

        public string GetMarkPoints()
        {
            return OutputPageViewModel.markPoints;
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
