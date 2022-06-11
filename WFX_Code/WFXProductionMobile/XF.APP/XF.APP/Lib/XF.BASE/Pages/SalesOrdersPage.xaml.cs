using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesOrdersPage : NavigationSearchPage
    {
        public ISalesOrdersViewModel context { get; set; }

        public SalesOrdersPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                context = DependencyService.Get<ISalesOrdersViewModel>();
                context.LineID = Preferences.Get("LINE_ID", string.Empty);
                context.LineMan = Preferences.Get("USER_NAME", string.Empty);
                BindingContext = context;
            }
            context.OnScreenAppearing();
        }

        void ListView_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            //Supresses highlighting of ListItems
            if (e.SelectedItem == null) return;

            ListView list = sender as ListView;

            switch (list.ClassId)
            {
                case "SoList":
                    context.SelectedSo = (SalesOrder)e.SelectedItem;
                    break;
                case "PoList":
                    context.SelectedPo = (PurchaseOrder1)e.SelectedItem;
                    break;
            }
            list.SelectedItem = null;
        }
    }
}
