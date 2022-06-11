using Xamarin.Essentials;
using Xamarin.Forms;

namespace XF.BASE
{
    public partial class OutputButtonView : ContentView
    {
        public static readonly BindableProperty ViewBgColorProperty = BindableProperty.Create(nameof(ViewBgColor), typeof(Color), typeof(OutputButtonView), Color.FromHex("#2ABB9B"));
        public static readonly BindableProperty ViewTitleProperty = BindableProperty.Create(nameof(ViewTitle), typeof(string), typeof(OutputButtonView), string.Empty);
        public static readonly BindableProperty ViewTextProperty = BindableProperty.Create(nameof(ViewText), typeof(string), typeof(OutputButtonView), string.Empty);
        public static readonly BindableProperty ViewIconProperty = BindableProperty.Create(nameof(ViewIcon), typeof(string), typeof(OutputButtonView), "ic_smile_pass");

        public string ViewTitle
        {
            get => (string)GetValue(ViewTitleProperty);
            set => SetValue(ViewTitleProperty, value);
        }

        public string ViewText
        {
            get => (string)GetValue(ViewTextProperty);
            set => SetValue(ViewTextProperty, value);
        }

        public string ViewIcon
        {
            get => (string)GetValue(ViewIconProperty);
            set => SetValue(ViewIconProperty, value);
        }

        public Color ViewBgColor
        {
            get => (Color)GetValue(ViewBgColorProperty);
            set => SetValue(ViewBgColorProperty, value);
        }

        public OutputButtonView()
        {
            InitializeComponent();
            string TabSize = Preferences.Get("TabSize", string.Empty);
            if (TabSize == "10inch" || TabSize == "")
            {
                lblBigHeaderText.IsVisible = true;
                lblBigHeaderValue.IsVisible = true;
            }
            if (TabSize == "7inch")
            {
                lblBigHeaderText7i.IsVisible = true;
                lblBigHeaderValue7i.IsVisible = true;
            }
        }

    }
}
