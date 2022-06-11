using System;
using System.Windows.Input;
using XF.APP.ABSTRACTION;

namespace XF.APP.BAL
{
    public class MasterPageViewModel : BaseViewModel, IMasterPageViewModel
    {
        public ICommand TapCommand { set; get; }
        public MasterPageViewModel()
        {
            TapCommand = new BaseCommand((object obj) => TapCommandClicked());
        }

        public void OnScreenAppearing()
        {
            
        }
        private void TapCommandClicked()
        {
            NativeService.NavigationService.NavigateAsync("ProcessPage");
            
        }
    }
}
