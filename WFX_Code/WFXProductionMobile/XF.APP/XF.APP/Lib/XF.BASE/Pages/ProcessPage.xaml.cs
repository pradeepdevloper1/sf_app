using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessPage : BasePage
    {
        public IProcessPageViewModel Context { get; set; }
        private ITransactionService transactionService;
        public ProcessPage()
        {
            InitializeComponent();
            BindingContext = null;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                Context = DependencyService.Get<IProcessPageViewModel>();

                BindingContext = Context;
            }
            Context.OnScreenAppearing();
        }

        void Frame_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Context != null)
            {
               
            }

        }

        void Frame_PropertyChanged_1(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Context != null)
            {
              
            }
        }

      


    }


}

