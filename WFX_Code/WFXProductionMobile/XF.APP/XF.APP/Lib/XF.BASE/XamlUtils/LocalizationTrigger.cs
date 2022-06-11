using Xamarin.Forms;
using XF.APP.BAL;

namespace XF.BASE
{
    public class LocalizationTrigger : TriggerAction<Picker>
    {
        protected override void Invoke(Picker sender)
        {
            CommonMethods.InitilizeLocalization(sender.ClassId);
            NativeService.NavigationService.SetRootPage("LoginPage");
        }
    }
}
