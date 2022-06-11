using System;
using System.Globalization;
using Xamarin.Forms;
using XF.BASE.Assets.Localization;

namespace XF.BASE
{
    public class SegmentTextChanger : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? AppResources.NewGarmentText : AppResources.ReworkGarmentText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
