using Acr.UserDialogs;
using Xamarin.Essentials;

namespace XF.APP.BAL
{
    public class InternetConnectivity
    {
        public static bool IsNetworkNotAvailable()
        {
            bool isNetworkNotAvailable = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                isNetworkNotAvailable = true;
                UserDialogs.Instance.Alert("Make sure that Wi-Fi or mobile data is turned on, then try again.", "No Internet connection");
            }

            return isNetworkNotAvailable;
        }
    }
}
