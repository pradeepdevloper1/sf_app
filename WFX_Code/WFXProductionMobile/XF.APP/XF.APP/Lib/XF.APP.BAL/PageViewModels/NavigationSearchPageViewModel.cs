using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Essentials;
using XF.APP.ABSTRACTION;

namespace XF.APP.BAL
{
    public class NavigationSearchPageViewModel : BaseViewModel, INavigationSearchViewModel
    {
        public ICommand LogoutCommand { set; get; }
        public ICommand SettingCommand { set; get; }

        public ICommand SearchCommand { set; get; }
        public ICommand BackButtonCommand { set; get; }
        public ICommand CloseCommand { set; get; }

        string lineID;
        public string LineID
        {
            get { return lineID; }
            set { SetProperty(ref lineID, value); }
        }

        string lineMan;
        public string LineMan
        {
            get { return lineMan; }
            set { SetProperty(ref lineMan, value); }
        }

        bool isSearchBarVisible;
        public bool IsSearchBarVisible
        {
            get { return isSearchBarVisible; }
            set { SetProperty(ref isSearchBarVisible, value); }
        }

        int userID;
        public int UserID
        {
            get { return userID; }
            set { SetProperty(ref userID, value); }
        }

        public NavigationSearchPageViewModel()
        {
            LogoutCommand = new BaseCommand((object obj) => LogoutCommandClicked());
            SettingCommand = new BaseCommand((object obj) => SettingCommandClicked());
        }

        private async void LogoutCommandClicked()
        {
            bool isConfirmed = await UserDialogs.Instance.ConfirmAsync("Are you sure you want to log out?", "Confirmation", "Yes", "No");

            if (isConfirmed)
                ClearData();
        }
        private async void SettingCommandClicked()
        {
            bool isConfirmed = await UserDialogs.Instance.ConfirmAsync("Are you sure you want to change role?", "Confirmation", "Yes", "No");

            if (isConfirmed)
                ChangeRole();
        }
        protected void ChangeRole()
        {
            Preferences.Set("RoleName", "");
            Preferences.Set("LINE_ID", "");
            NativeService.NavigationService.SetRootPage("RoleSelectionPage");
        }

        protected void ClearData()
        {
            // Clear data
            Preferences.Clear();
            SalesOrdersPageViewModel.ChangeToSoView = true;
            NativeService.NavigationService.SetRootPage("LoginPage");
        }
    }
}
