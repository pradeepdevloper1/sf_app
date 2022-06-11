using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System;
using XF.APP.BAL;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionPage : NavigationSearchPage
    {
        public IInspectionViewModel Context { get; set; }
        private PurchaseOrder1 SelectedPo { get; set; }

        public InspectionPage()
        {
            InitializeComponent();
            string TabSize = Preferences.Get("TabSize", string.Empty);
            if (TabSize == "10inch" || TabSize == "")
            {
                lblShiftDropdown.FontSize = 18;
            }
            if (TabSize == "7inch" )
            {
                lblShiftDropdown.FontSize = 14;
            }
        }

        public InspectionPage(PurchaseOrder1 selectedPo)
        {
            InitializeComponent();

            SelectedPo = selectedPo;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext == null)
            {
                Context = DependencyService.Get<IInspectionViewModel>();
                Context.LineID = Preferences.Get("LINE_ID", string.Empty);
                Context.LineMan = Preferences.Get("USER_NAME", string.Empty);
                Context.SelectedPo = SelectedPo;
                BindingContext = Context;
            }
            Context.OnScreenAppearing();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Supresses highlighting of ListItems
            if (e.SelectedItem == null) return;

            ListView list = sender as ListView;

            Context.SelectedListCellId = int.Parse(list.ClassId ?? "0");
            Context.SelectedItem = e.SelectedItem;

            list.SelectedItem = null;
        }
       async private void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {

            var picker = (Picker)sender;
            var selectedOption = picker.SelectedItem;
            if (picker.SelectedIndex != 0)
            {
                DropDownValue s = (DropDownValue)selectedOption;
                double minutes = (s.StartTime - DateTime.Now.TimeOfDay).TotalMinutes;
                if (minutes > 240.00)
                {
                   await DisplayAlert("Alert", "Cannot select future Shift", "OK");
                    picker.SelectedIndex = 0;
                }
                else
                {
                    Preferences.Set("Shift", s.shiftName);
                }
            }
            else
            {
                Preferences.Set("Shift", "");
            }
        }
    }
}
