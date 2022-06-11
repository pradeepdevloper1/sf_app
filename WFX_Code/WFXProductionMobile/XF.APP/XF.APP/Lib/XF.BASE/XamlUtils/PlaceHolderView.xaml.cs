using Xamarin.Forms;

namespace XF.BASE
{
    public partial class PlaceHolderView : ContentView
    {
        public static readonly BindableProperty ViewBorderColorProperty = BindableProperty.Create(nameof(ViewBorderColor), typeof(Color), typeof(PlaceHolderView), Color.FromHex("#F3F3F3"));
        public static readonly BindableProperty ViewBgColorProperty = BindableProperty.Create(nameof(ViewBgColor), typeof(Color), typeof(PlaceHolderView), Color.FromHex("#F9F9F9"));
        public static readonly BindableProperty ViewTextProperty = BindableProperty.Create(nameof(ViewText), typeof(string), typeof(PlaceHolderView), string.Empty);
        public static readonly BindableProperty ViewTextColorProperty = BindableProperty.Create(nameof(ViewTextColor), typeof(Color), typeof(PlaceHolderView), Color.FromHex("#D2D2D2"));

        public string ViewText
        {
            get => (string)GetValue(ViewTextProperty);
            set => SetValue(ViewTextProperty, value);
        }

        public Color ViewTextColor
        {
            get => (Color)GetValue(ViewTextColorProperty);
            set => SetValue(ViewTextColorProperty, value);
        }

        public Color ViewBorderColor
        {
            get => (Color)GetValue(ViewBorderColorProperty);
            set => SetValue(ViewBorderColorProperty, value);
        }

        public Color ViewBgColor
        {
            get => (Color)GetValue(ViewBgColorProperty);
            set => SetValue(ViewBgColorProperty, value);
        }

        public PlaceHolderView()
        {
            InitializeComponent();
        }
    }
}
