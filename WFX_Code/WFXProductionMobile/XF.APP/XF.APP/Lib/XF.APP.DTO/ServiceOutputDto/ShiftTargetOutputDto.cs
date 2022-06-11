using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class ShiftTargetOutputDto : BaseResponseDto
    {
        public List<ShiftTarget> data { get; set; }
    }

    public class ShiftTarget
    {
        public int lineTargetID { get; set; }
        public string module { get; set; }
        public string section { get; set; }
        public string line { get; set; }
        public string style { get; set; }
        public string soNo { get; set; }
        public string poNo { get; set; }
        public string part { get; set; }
        public string color { get; set; }
        public double smv { get; set; }
        public int operators { get; set; }
        public int helpers { get; set; }
        public string shiftName { get; set; }
        public double shiftHours { get; set; }
        public DateTime date { get; set; }
        public int plannedEffeciency { get; set; }
        public int plannedTarget { get; set; }
        public int userID { get; set; }
        public DateTime entryDate { get; set; }
    }
}
