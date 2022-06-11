using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XF.APP.ABSTRACTION;
using XF.APP.iOS.Service;

[assembly: Dependency(typeof(StatusBarStyleManager))]
namespace XF.APP.iOS.Service
{
    public class StatusBarStyleManager : IStatusBarStyleManager
    {
        public void SetColoredStatusBar(string hexColor)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    UIView statusBar = new UIView(UIApplication.SharedApplication.KeyWindow.WindowScene.StatusBarManager.StatusBarFrame);
                    statusBar.BackgroundColor = Color.FromHex(hexColor).ToUIColor();
                    UIApplication.SharedApplication.KeyWindow.AddSubview(statusBar);
                }
                else
                {
                    UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
                    if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
                    {
                        statusBar.BackgroundColor = Color.FromHex(hexColor).ToUIColor();
                    }
                }
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
                GetCurrentViewController().SetNeedsStatusBarAppearanceUpdate();
            });
        }

        UIViewController GetCurrentViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }
    }
}