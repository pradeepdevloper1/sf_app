using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.DeviceInfo;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
namespace XF.APP.BAL
{
    public class LoginPageViewModel : BaseViewModel, ILoginPageViewModel
    {
        public ICommand LanguageChangeCommand { set; get; }
        public ICommand LoginCommand { set; get; }
        public ICommand ForgotPasswordCommand { set; get; }
        public ICommand CreateAccountCommand { set; get; }
        public ICommand SupportCommand { set; get; }
        List<LanguageViewModel> languages;
        public List<LanguageViewModel> Languages
        {
            get { return languages; }
            set { SetProperty(ref languages, value); }
        }
        LanguageViewModel selectedLanguages;
        public LanguageViewModel SelectedLanguages
        {
            get { return selectedLanguages; }
            set { SetProperty(ref selectedLanguages, value); }
        }
        string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
        string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        string version;
        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }
        string supportEmail;
        public string SupportEmail
        {
            get { return supportEmail; }
            set { SetProperty(ref supportEmail, value); }
        }
        public string DeviceName { get; set; }
        public string DeviceSerialNo { get; set; }
        public LoginPageViewModel()
        {
            Languages = new List<LanguageViewModel>() {
            new LanguageViewModel(){LanguageTitle="EN",LanguageCode="en-US"},
            new LanguageViewModel(){LanguageTitle="HI",LanguageCode="hi-IN"},
            new LanguageViewModel(){LanguageTitle="MR",LanguageCode="mr-IN"},
            new LanguageViewModel(){LanguageTitle="VI",LanguageCode="vi-VN"},
            };

            string langCode = Preferences.Get("SEL_LANG", "en-US");
            SelectedLanguages = string.IsNullOrEmpty(langCode) ? Languages.First() : Languages.Single(l => l.LanguageCode == langCode);
            LanguageChangeCommand = new BaseCommand(async (object obj) => await LanguageChangeCommandClicked());
            LoginCommand = new BaseCommand(async (object obj) => await LoginCommandClicked());
            ForgotPasswordCommand = new BaseCommand(async (object obj) => await ForgotPasswordCommandClicked());
            CreateAccountCommand = new BaseCommand(async (object obj) => await CreateAccountCommandClicked());
            SupportCommand = new BaseCommand(async (object obj) => await SupportCommandClicked(obj));
        }
        public void OnScreenAppearing(string deviceName)
        {
            Username = string.Empty;
            Password = string.Empty;
            DeviceName = deviceName ?? "NA";
            DeviceSerialNo = GetDeviceMacID();
            Preferences.Set("DeviceSerialNo", DeviceSerialNo);
        }
        private async Task LanguageChangeCommandClicked()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                MessagingCenter.Send(Application.Current, "LocalizationChangeNotify", SelectedLanguages.LanguageCode);
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task LoginCommandClicked()
        {
            if (IsBusy || InternetConnectivity.IsNetworkNotAvailable())
                return;
            IsBusy = true;
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    UserDialogs.Instance.Alert("Please fill mandatory fields.");
                    return;
                }
                UserDialogs.Instance.ShowLoading(LoadingText);
                //web service call to validate user login
                var userService = ServiceLocator.Resolve<IUserWebService>();
                var result = await userService.LoginUserAsync<UserLoginOutputDto>(new UsersLoginInputDto()
                {
                    Username = Username,
                    Password = Password,
                    TabletID = DeviceName,
                    DeviceSerialNo = DeviceSerialNo
                });
                UserDialogs.Instance.HideLoading();
                //navigate to home page
                if (result != null && result.status == System.Net.HttpStatusCode.OK)
                {
                    Preferences.Set("AUTH_KEY", result.auth);
                    Preferences.Set("USER_ID", result.data.userID);
                    Preferences.Set("USER_NAME", $"{result.data.userFirstName} {result.data.userLastName}");
                    Preferences.Set("FACTORY_ID", result.data.factoryID);
                    Preferences.Set("LINE_ID", result.data.LineName);
                    Preferences.Set("TabletID", DeviceName);
                    Preferences.Set("Module", result.data.Module);

                    //NativeService.NavigationService.SetRootPage("SalesOrdersPage");
                    NativeService.NavigationService.SetRootPage("RoleSelectionPage");
                    SetDeviceSize();
                }
                else
                {
                    if (result != null)
                    {
                        UserDialogs.Instance.Alert(result.message ?? "Not able to login, Please try again later.");
                    }
                    if (result == null)
                    {
                        UserDialogs.Instance.Alert("Invalid User");
                    }
                }
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert($"Issue - {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public void SetDeviceSize()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                string[] Androidresolution7iArray = { "480 x 800", "600 x 1024", "720 x 1600", "800 x 400", "800 x 480", "800 x 1280", "1024 x 600", "1080 x 2400", "1280 x 720", "1280 x 800", "1340 x 800" };
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    Preferences.Set("TabSize", "10inch");
                    var mainDisplayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
                    var width = mainDisplayInfo.Width;
                    var height = mainDisplayInfo.Height;
                    var resolution = width + " x " + height;
                    foreach (var res in Androidresolution7iArray)
                    {
                        if (res == resolution)
                        {
                            Preferences.Set("TabSize", "7inch");
                        }
                    }
                }
            }
            if (Device.RuntimePlatform == Device.iOS)
            {
                string[] IOSresolution7iArray = { "1536 x 2048", "2048 x 1536" };
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    Preferences.Set("TabSize", "10inch");
                    var mainDisplayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
                    var width = mainDisplayInfo.Width;
                    var height = mainDisplayInfo.Height;
                    var resolution = width + " x " + height;
                    foreach (var res in IOSresolution7iArray)
                    {
                        if (res == resolution)
                        {
                            Preferences.Set("TabSize", "7inch");
                        }
                    }
                }
            }
        }
        private async Task ForgotPasswordCommandClicked()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task CreateAccountCommandClicked()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task SupportCommandClicked(object sender)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                var message = new EmailMessage
                {
                    Subject = "<MENTION_YOUR_SUBJECT_HERE>",
                    Body = "<MENTION_YOUR_CONTENT_HERE>",
                    To = new List<string> { sender.ToString() },
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                UserDialogs.Instance.Alert("Email is not supported on this device");
            }
            catch (Exception ex)
            {
                await Logger.AddException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        async Task<bool> CheckPermissionsAsync()
        {
            var retVal = false;
            try
            {
                var permissions = await Permissions.CheckStatusAsync<Permissions.Phone>();
                if (permissions != PermissionStatus.Granted)
                {
                    permissions = await Permissions.RequestAsync<Permissions.Phone>();
                }
                if (permissions == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (permissions != PermissionStatus.Unknown)
                {
                    UserDialogs.Instance.Alert("Phone permission is denied. please give phone permission, try again.");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.Message);
            }
            return retVal;
        }
        public string GetDeviceMacID()
        {
            string sDeviceMacID = "";
            try
            {
                sDeviceMacID = CrossDeviceInfo.Current.Id;
            }
            catch (Exception ex) { throw ex; }
            return sDeviceMacID;
        }
    }
}