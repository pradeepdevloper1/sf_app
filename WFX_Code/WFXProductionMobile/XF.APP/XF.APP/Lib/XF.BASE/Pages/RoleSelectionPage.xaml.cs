using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoleSelectionPage : ContentPage
    {     
        public IRoleSelectionPageViewModel context { get; set; }

        public RoleSelectionPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                context = DependencyService.Get<IRoleSelectionPageViewModel>();
                BindingContext = context;
            }
            context.OnScreenAppearing();
        }
       
        private void RoleList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            context.SelectedRole = (UserRole)e.SelectedItem;
            Preferences.Set("RoleName", context.SelectedRole.RoleName);
            //UserDialogs.Instance.Alert("Item Selected  " + context.SelectedRole.RoleName);
            context.fetchLineList();
        }
        private void LineList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string RoleName = Preferences.Get("RoleName", "");
            if (RoleName == "")
            {
                UserDialogs.Instance.Alert("Please Select Role.");
                return;
            }
            if (e.SelectedItem == null) return;
            context.SelectedLine = (Line)e.SelectedItem;
            Preferences.Set("LINE_ID", context.SelectedLine.LineName);
            //UserDialogs.Instance.Alert("Item Selected  " + context.SelectedLine.LineName);
        }
    }
}