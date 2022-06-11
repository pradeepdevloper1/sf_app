using System.Collections.Generic;

namespace XF.APP.ABSTRACTION
{
    public static class Constants
    {
        public static string DbName { get; set; } = "E2E.db";
        public static string Preferences_UserDetails { get; set; } = "UserDetails";
        public static Queue<KeyValuePair<float, float>> TouchCoOrdinates { get; set; } = new Queue<KeyValuePair<float, float>>();
    }
}
