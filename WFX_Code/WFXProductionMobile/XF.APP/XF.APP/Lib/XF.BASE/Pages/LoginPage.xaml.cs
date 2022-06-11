using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.BASE.Assets.Localization;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public ILoginPageViewModel context{ get; set; }
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                context = DependencyService.Get<ILoginPageViewModel>();
                context.Version = VersionTracking.CurrentVersion;
                context.LoadingText = AppResources.LoadingText;
                BindingContext = context;
            }
            lblTabletID.Text = string.IsNullOrEmpty(DeviceInfo.Name) ? "NA" : DeviceInfo.Name;
            context.OnScreenAppearing(DeviceInfo.Name);

            MessagingCenter.Subscribe<string>(Application.Current, "LocalizationChangeNotify", (args) => {

                CommonMethods.InitilizeLocalization(args);
            });
        }

    }
}