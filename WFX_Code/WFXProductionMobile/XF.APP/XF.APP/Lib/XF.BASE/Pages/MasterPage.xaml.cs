using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.BAL;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [Obsolete]
    public partial class MasterPage : MasterDetailPage
    {
        public IMasterPageViewModel Context { get; set; }
        public MasterPage()
        {
            
            InitializeComponent();

            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(SalesOrdersPage))) { BarBackgroundColor = Color.FromHex("#F5F5F5"), BarTextColor = Color.Black };
        }


       

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    if (BindingContext == null)
        //    {

        //        Context = DependencyService.Get<IMasterPageViewModel>();

        //        BindingContext = Context;
        //    }

        //}

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            NativeService.NavigationService.NavigateAsync("ProcessPage");
        }
    }
}
