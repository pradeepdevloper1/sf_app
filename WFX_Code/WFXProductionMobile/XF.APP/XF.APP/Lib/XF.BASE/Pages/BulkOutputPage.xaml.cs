using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using XF.BASE.Assets.Localization;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BulkOutputPage : BasePage
    {
        public IBulkOutputViewModel context { get; set; }
        PostQCInputDto PostQCInput { get; set; }

        public BulkOutputPage()
        {
            InitializeComponent();
            new PostQCInputDto().QCStatus = 1;
        }

        public BulkOutputPage(PostQCInputDto postQCInput)
        {
            InitializeComponent();

            PostQCInput = postQCInput;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetTheme(PostQCInput.QCStatus);

            if (BindingContext == null)
            {
                context = DependencyService.Get<IBulkOutputViewModel>();

                switch (PostQCInput.QCStatus)
                {
                    case 1:
                        context.BulkOutputType = AppResources.PassText;
                        context.SmileType = "ic_smile_pass";
                        context.RoundedBtnBgColor = (Color)Application.Current.Resources["RoundedBtnPassBgColor"];
                        break;
                    case 2:
                        context.BulkOutputType = AppResources.DefectText;
                        context.SmileType = "ic_smile_defect";
                        context.RoundedBtnBgColor = (Color)Application.Current.Resources["RoundedBtnDefectBgColor"];
                        break;
                    case 3:
                        context.BulkOutputType = AppResources.RejectText;
                        context.SmileType = "ic_smile_reject";
                        context.RoundedBtnBgColor = (Color)Application.Current.Resources["RoundedBtnRejectBgColor"];
                        break;
                }
                context.PostQCInput = PostQCInput;
                BindingContext = context;
            }
            context.OnScreenAppearing();
        }
    }
}