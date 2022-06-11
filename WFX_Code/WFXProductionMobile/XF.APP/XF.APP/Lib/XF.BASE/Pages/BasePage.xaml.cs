using Xamarin.Forms;
using XF.APP.ABSTRACTION;

namespace XF.BASE.Pages
{
    public partial class BasePage : ContentPage
    {
        public void SetTheme(int themeType)
        {
            switch (themeType)
            {
                case 1:
                    BackgroundColor = (Color)Application.Current.Resources["PassBackgroundColor"];
                    break;
                case 2:
                    BackgroundColor = (Color)Application.Current.Resources["RejectBackgroundColor"];
                    break;
                case 3:
                    BackgroundColor = (Color)Application.Current.Resources["DefectBackgroundColor"];
                    break;
                default:
                    BackgroundColor = (Color)Application.Current.Resources["BaseBackgroundColor"];
                    break;
            }

            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = BackgroundColor;
            DependencyService.Get<IStatusBarStyleManager>().SetColoredStatusBar(BackgroundColor.ToHex());
        }
    }
}