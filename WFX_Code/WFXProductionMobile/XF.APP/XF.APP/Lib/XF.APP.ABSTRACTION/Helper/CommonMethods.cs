using System;
using System.Collections.Generic;
using System.Linq;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public static class CommonMethods
    {
        public static string GetCurrentShift(List<Shift> shiftDetails, int factoryId)
        {
            try
            {
                var shift = shiftDetails.Last(s => s.factoryID == factoryId && s.shiftStartTime <= DateTime.Now.TimeOfDay && s.shiftEndTime >= DateTime.Now.TimeOfDay );
                //return "S1K";
                return shift.shiftName;
            }
            catch (Exception)
            {
                return "S1K";
            }
        }

        public static List<string> GetShiftList(List<Shift> shiftDetails, int factoryId)
        {
            try
            {
                return shiftDetails.Where(s => s.factoryID == factoryId)?.Select(s => s.shiftName)?.Distinct()?.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
