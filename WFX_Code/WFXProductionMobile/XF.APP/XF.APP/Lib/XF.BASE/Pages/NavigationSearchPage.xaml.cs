using Xamarin.Forms.Xaml;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationSearchPage : BasePage
    {
        public NavigationSearchPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetTheme(0);
        }
    }
}
