using Xamarin.Essentials;
using Xamarin.Forms;

namespace XF.BASE
{
    public partial class OutputInfoView : ContentView
    {
        public static readonly BindableProperty ViewTextColorProperty = BindableProperty.Create(nameof(ViewTextColor), typeof(Color), typeof(OutputInfoView), Color.FromHex("#4A4A4A"));
        public static readonly BindableProperty ViewTitleProperty = BindableProperty.Create(nameof(ViewTitle), typeof(string), typeof(OutputInfoView), string.Empty);
        public static readonly BindableProperty ViewText1Property = BindableProperty.Create(nameof(ViewText1), typeof(string), typeof(OutputInfoView), string.Empty);
        public static readonly BindableProperty ViewText2Property = BindableProperty.Create(nameof(ViewText2), typeof(string), typeof(OutputInfoView), string.Empty);
        public static readonly BindableProperty ViewText1ValueProperty = BindableProperty.Create(nameof(ViewText1Value), typeof(string), typeof(OutputInfoView), string.Empty);
        public static readonly BindableProperty ViewText2ValueProperty = BindableProperty.Create(nameof(ViewText2Value), typeof(string), typeof(OutputInfoView), string.Empty);
 
        public Color ViewTextColor
        {
            get => (Color)GetValue(ViewTextColorProperty);
            set => SetValue(ViewTextColorProperty, value);
        }

        public string ViewTitle
        {
            get => (string)GetValue(ViewTitleProperty);
            set => SetValue(ViewTitleProperty, value);
        }

        public string ViewText1
        {
            get => (string)GetValue(ViewText1Property);
            set => SetValue(ViewText1Property, value);
        }

        public string ViewText2
        {
            get => (string)GetValue(ViewText2Property);
            set => SetValue(ViewText2Property, value);
        }

        public string ViewText1Value
        {
            get => (string)GetValue(ViewText1ValueProperty);
            set => SetValue(ViewText1ValueProperty, value);
        }

        public string ViewText2Value
        {
            get => (string)GetValue(ViewText2ValueProperty);
            set => SetValue(ViewText2ValueProperty, value);
        }

        public OutputInfoView()
        {
            InitializeComponent();
            string TabSize = Preferences.Get("TabSize", string.Empty);
            if (TabSize == "10inch" || TabSize == "")
            {
                frmOutputView10i.IsVisible = true;
            }
            if (TabSize == "7inch" )
            {
                frmOutputView7i.IsVisible = true;
            }

        }
    }
}
