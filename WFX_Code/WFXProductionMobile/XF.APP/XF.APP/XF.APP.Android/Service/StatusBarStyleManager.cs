using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.Droid.Service;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarStyleManager))]
namespace XF.APP.Droid.Service
{
    public class StatusBarStyleManager : IStatusBarStyleManager
    {
        public void SetColoredStatusBar(string hexColor)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var currentWindow = GetCurrentWindow();
                    currentWindow.DecorView.SystemUiVisibility = 0;
                    currentWindow.SetStatusBarColor(Android.Graphics.Color.ParseColor(hexColor));
                });
            }
        }

        Window GetCurrentWindow()
        {
            var window = CrossCurrentActivity.Current.Activity.Window;

            // clear FLAG_TRANSLUCENT_STATUS flag:
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            // add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            return window;
        }
    }
}