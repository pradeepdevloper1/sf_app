using Acr.UserDialogs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.APP.BAL
{
    public class RoleSelectionPageViewModel : NavigationSearchPageViewModel, IRoleSelectionPageViewModel
    {
        public ICommand StartButtonCommand { set; get; }
        public ICommand RefreshCommand { set; get; }

        ObservableCollection<UserRole> userRoles = new ObservableCollection<UserRole>();
        public ObservableCollection<UserRole> UserRoles { get { return userRoles; } }
        ObservableCollection<Line> lineNames = new ObservableCollection<Line>();
        public ObservableCollection<Line> LineNames { get { return lineNames; } }


        UserRole selectedRole;
        public UserRole SelectedRole
        {
            get { return selectedRole; }
            set
            {
                SetProperty(ref selectedRole, value);            
            }
        }
        Line selectedLine;

        public Line SelectedLine
        {
            get { return selectedLine; }
            set
            {
                SetProperty(ref selectedLine, value);
            }
        }
        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        ObservableCollection<UserRole> roleList;
        public ObservableCollection<UserRole> RoleList
        {
            get { return roleList; }
            set { SetProperty(ref roleList, value); }
        }
        ObservableCollection<Line> lineList;
        public ObservableCollection<Line> LineList
        {
            get { return lineList; }
            set { SetProperty(ref lineList, value); }
        }
        InspDefectViewModel selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value as InspDefectViewModel);

            }
        }
        int selectedListCellId;
        public int SelectedListCellId
        {
            get { return selectedListCellId; }
            set { SetProperty(ref selectedListCellId, value); }
        }
        ObservableCollection<InspDefectViewModel> roles;
        public ObservableCollection<InspDefectViewModel> Roles
        {
            get { return roles; }
            set { SetProperty(ref roles, value); }
        }
        bool startBtnVilibility;
        public bool StartBtnVilibility
        {
            get { return startBtnVilibility; }
            set { SetProperty(ref startBtnVilibility, value); }
        }
        
        public RoleSelectionPageViewModel()
        {
            BackButtonCommand = new BaseCommand((object obj) => BackButtonCommandClicked());
            StartButtonCommand = new BaseCommand((object obj) => StartCommandClicked());
            RefreshCommand = new BaseCommand((object obj) => RefreshCommandClicked());
        }
        public void OnScreenAppearing()
        {
            userRoles.Clear();
            lineNames.Clear();
            Task.Run(async () => { await FetchRoleList(); });
            StartBtnVilibility = false;
            //Task.Run(async () => { await FetchLineList(); });
        }
       public void fetchLineList()
        {
            lineNames.Clear();
            Task.Run(async () => { await FetchLineList(); });
            StartBtnVilibility = true;
        }
        private void BackButtonCommandClicked()
        {
            Preferences.Clear();
            userRoles.Clear();
            lineNames.Clear();
            NativeService.NavigationService.SetRootPage("LoginPage");
        }
        private void StartCommandClicked()
        {
            string RoleName = Preferences.Get("RoleName", string.Empty);
            if (RoleName == "")
            {
                UserDialogs.Instance.Alert("Please Select Role.");
                return;
            }
            string Line = Preferences.Get("LINE_ID", string.Empty);
            if (Line == "")
            {
                UserDialogs.Instance.Alert("Please Select Line.");
                return;
            }
            
            NativeService.NavigationService.SetRootPage("MasterPage");
        }
        private async void RefreshCommandClicked()
        {
            IsRefreshing = true;

            await FetchRoleList();

            IsRefreshing = false;
        }
        public async Task FetchRoleList()
        {
            var userService  = ServiceLocator.Resolve<IUserWebService>();
            userRoles.Clear();
            var result = await userService.GetUserRoleListAsync<UserRoleOutputDto>(Preferences.Get("AUTH_KEY", null));
            if (result.status == System.Net.HttpStatusCode.OK)
            {
                RoleList = new ObservableCollection<UserRole>();
                foreach (UserRole role in result.data)
                {
                    userRoles.Add(new UserRole { RoleName = role.RoleName,UserRoleID=role.UserRoleID,UserRoleType=role.UserRoleType });
                }
            }
            //string[] userRolesArray = { "Cutting QC", "Embroidery QC", "Washing QC", "Finishing QC", "Packing QC", "Outsourcing QC", "Floating QC", "Endline QC", "Inline QC" };
            //userRoles.Clear();
            //foreach (string rn in userRolesArray)
            //{
            //    userRoles.Add(new UserRole { RoleName = rn });
            //}
        }
        public async Task FetchLineList()
        {
            var userService = ServiceLocator.Resolve<IUserWebService>();
            lineNames.Clear();
            var Module = Preferences.Get("Module", string.Empty);
            var result = await userService.GetUserLineListAsync<LineOutputDto>(new UserModule() { Module = Module }, Preferences.Get("AUTH_KEY", null));
            if (result.status == System.Net.HttpStatusCode.OK)
            {
                LineList = new ObservableCollection<Line>();
                foreach (Line line in result.data)
                {
                    lineNames.Add(new Line { LineID = line.LineID, LineName = line.LineName,Opacity=1.0 });
                }
            }
            //string[] lineNameArray = { "Line 1", "Line 2", "Line 3", "Line 4", "Line 5", "Line 6" };
            //lineNames.Clear();
            //foreach (string ln in lineNameArray)
            //{
            //    lineNames.Add(new Line { LineName = ln });
            //}
        }

        private void ModifyCellItem(RoleViewModel selectedItem)
        {
            RoleViewModel item = SelectedItem as RoleViewModel;
            bool isSelected = !item.IsSelected;
            switch (SelectedListCellId)
            {
                case 1: // Role
                    break;
                case 2: // Line
                    break;
            }
        }
    }
}
