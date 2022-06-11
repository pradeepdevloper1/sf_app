using System;
using System.Collections.Generic;
using System.Linq;

namespace XF.APP.BAL
{
    public class CommonMethods
    {
        protected CommonMethods()
        {
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
