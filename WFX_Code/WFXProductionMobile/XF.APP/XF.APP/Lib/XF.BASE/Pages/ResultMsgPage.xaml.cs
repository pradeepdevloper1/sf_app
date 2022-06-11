using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.BASE.Assets.Localization;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultMsgPage : BasePage
    {
        public IResultMsgViewModel context { get; set; }
        int MessageType { get; set; }

        public ResultMsgPage()
        {
            InitializeComponent();
            MessageType = 1;
        }

        public ResultMsgPage(int messageType)
        {
            InitializeComponent();

             MessageType = messageType;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetTheme(MessageType);

            if (BindingContext == null)
            {
                context = DependencyService.Get<IResultMsgViewModel>();

                switch (MessageType)
                {
                    case 1:
                        context.MessageText = AppResources.SuccessMsgText;
                        context.SmileType = "ic_smile_pass";
                        break;
                    case 2:
                        context.MessageText = AppResources.FailureMsgText;
                        context.SmileType = "ic_smile_reject";
                        break;
                }

                BindingContext = context;
            }
        }
    }
}
