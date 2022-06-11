using XF.BASE.Assets.Localization;
using System.Globalization;
using Xamarin.Essentials;
using System;

namespace XF.BASE
{
    public static class CommonMethods
    {
        public static void InitilizeLocalization(string language = "en-US")
        {
            var culture = new CultureInfo(language);
            AppResources.Culture = culture;
            CultureInfo.CurrentUICulture = new CultureInfo(language,false);
            Preferences.Set("SEL_LANG", language);
        }

        public static string GetSizeName(string sizeName)
        {
            try
            {
                int index = sizeName.IndexOf("-");
                if (index >= 0)
                    sizeName = sizeName.Substring(0, index);
            }
            catch (Exception)
            {
            }
            return sizeName;
        }
        public static int GetSizeQty(string sizeName)
        {
            int sizeQty = 0;
            try
            {
                int index = sizeName.IndexOf("-");
                if (index >= 0)
                    sizeQty = Convert.ToInt32(sizeName.Split('-')[1]);
            }
            catch (Exception)
            {
                sizeQty = 0;
            }
            return sizeQty;
        }

    }
}
