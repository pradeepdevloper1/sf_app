using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Essentials;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.APP.BAL
{
    public class BulkOutputPageViewModel : BaseViewModel, IBulkOutputViewModel
    {
        public ICommand CloseCommand { set; get; }
        public ICommand ClearCommand { set; get; }
        public ICommand SubmitCommand { set; get; }
        public ICommand NumericButtonCommand { set; get; }
        private ITransactionService transactionService;
        string _bulkOutputType;
        public string BulkOutputType
        {
            get { return _bulkOutputType; }
            set { SetProperty(ref _bulkOutputType, value); }
        }

        string _smileType;
        public string SmileType
        {
            get { return _smileType; }
            set { SetProperty(ref _smileType, value); }
        }

        Color _roundedBtnBgColor;
        public Color RoundedBtnBgColor
        {
            get { return _roundedBtnBgColor; }
            set { SetProperty(ref _roundedBtnBgColor, value); }
        }

        int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set { SetProperty(ref _totalCount, value); }
        }

        PostQCInputDto postQCDto;
        public PostQCInputDto PostQCInput
        {
            get { return postQCDto; }
            set { SetProperty(ref postQCDto, value); }
        }

        public BulkOutputPageViewModel()
        {
            CloseCommand = new BaseCommand((object obj) => CloseCommandClicked());
            ClearCommand = new BaseCommand((object obj) => ClearCommandClicked());
            SubmitCommand = new BaseCommand(async (object obj) => await SubmitCommandClicked());
            NumericButtonCommand = new BaseCommand((object obj) => NumericButtonCommandClicked(obj));
            transactionService = ServiceLocator.Resolve<ITransactionService>();
        }

        public void OnScreenAppearing()
        {
            TotalCount = 0;
        }

        private void CloseCommandClicked()
        {
            NativeService.NavigationService.GoBack();
        }

        private void ClearCommandClicked()
        {
            TotalCount = TotalCount.ToString().Length > 1 ? TotalCount / 10 : 0;
        }

        private async Task SubmitCommandClicked()
        {
            if (InternetConnectivity.IsNetworkNotAvailable()) return;

            try
            {
                UserDialogs.Instance.ShowLoading(LoadingText);
                string shiftname = Preferences.Get("Shift", string.Empty);
                PostQCInput.ShiftName = shiftname;
                var existingRecord = await transactionService.GetByIdPoAsync(PostQCInput.SONo, PostQCInput.PONo, PostQCInput.Color, PostQCInput.HexCode, PostQCInput.ShiftName);
                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();
                int SizeQty = PostQCInput.Qty;
                int QCStatus = 0;
                if (PostQCInput.TypeOfWork == 2)
                    QCStatus = 2;
                else
                    QCStatus = 1;
                var result1 = await userService.GetTotalQCQuantity<PostQCOutputDto>(new PostQCInputDto()
                {
                    PONo = PostQCInput.PONo,
                    Color = PostQCInput.Color,
                    Size = PostQCInput.Size,
                    FactoryID = PostQCInput.FactoryID,
                    Module = PostQCInput.Module,
                    QCStatus = QCStatus,
                    Line = Preferences.Get("LINE_ID", string.Empty)
                });

                if (PostQCInput.TypeOfWork == 2)
                {
                    if (result1.qty - TotalCount < 0)
                    {
                        UserDialogs.Instance.Alert("There is only " + result1.qty + " Units to rework.");
                        UserDialogs.Instance.HideLoading();
                        return;
                    }
                }
                else
                {
                    int passedQty = Convert.ToInt32(result1.qty) + TotalCount;
                    if (SizeQty < passedQty)
                    {
                        UserDialogs.Instance.Alert("Pass Quantity " + passedQty + " Cannnot be Greater than " + SizeQty);
                        UserDialogs.Instance.HideLoading();
                        return;
                    }
                }
                PostQCInput.Qty = TotalCount;
                PostQCInput.TabletID = Preferences.Get("TabletID", string.Empty);

                existingRecord.PassQty = existingRecord.PassQty + TotalCount;

                await transactionService.SaveUpdatePoAsync(existingRecord);

                var result = await userService.PostQCAsync<PostQCOutputDto>(PostQCInput);

                UserDialogs.Instance.HideLoading();

                if (result.status == System.Net.HttpStatusCode.OK)
                {
                    switch (PostQCInput.QCStatus)
                    {
                        case 1:
                            if (PostQCInput.TypeOfWork == 2 && InspectionPageViewModel.DefectQtyCount > 0)
                                InspectionPageViewModel.DefectQtyCount -= PostQCInput.Qty;

                            if (PostQCInput.TypeOfWork == 3 && InspectionPageViewModel.RejectQtyCount > 0)
                                InspectionPageViewModel.RejectQtyCount -= PostQCInput.Qty;

                            InspectionPageViewModel.PassQtyCount += PostQCInput.Qty;
                            OutputPageViewModel.POShiftData.Add(new PurchaseOrderShift
                            {
                                soNo = PostQCInput.SONo,
                                poNo = PostQCInput.PONo,
                                color = PostQCInput.Color,
                                Size = PostQCInput.Size,
                                DefectType = 1,
                                Qty = PostQCInput.Qty,
                                TypeofWork = PostQCInput.TypeOfWork
                            });
                            OutputPageViewModel.postQCQueue.Add(new KeyValuePair<string, string>($"1#{result.id}#{PostQCInput.Qty}", $"{PostQCInput.TypeOfWork - 1}#{PostQCInput.TypeOfWork}"));
                            break;
                    }

                    await NativeService.NavigationService.GoBack();
                }
                else
                {
                    UserDialogs.Instance.Alert("Not able to sync the data, Please try again later.");
                }
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
        }

        private void NumericButtonCommandClicked(object obj)
        {
            var id = obj as string;

            try
            {
                switch (id)
                {
                    case "0":
                        TotalCount = int.Parse($"{TotalCount}0");
                        break;
                    case "1":
                        TotalCount = int.Parse($"{TotalCount}1");
                        break;
                    case "2":
                        TotalCount = int.Parse($"{TotalCount}2");
                        break;
                    case "3":
                        TotalCount = int.Parse($"{TotalCount}3");
                        break;
                    case "4":
                        TotalCount = int.Parse($"{TotalCount}4");
                        break;
                    case "5":
                        TotalCount = int.Parse($"{TotalCount}5");
                        break;
                    case "6":
                        TotalCount = int.Parse($"{TotalCount}6");
                        break;
                    case "7":
                        TotalCount = int.Parse($"{TotalCount}7");
                        break;
                    case "8":
                        TotalCount = int.Parse($"{TotalCount}8");
                        break;
                    case "9":
                        TotalCount = int.Parse($"{TotalCount}9");
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = Logger.AddException(ex);
            }
        }
    }
}
