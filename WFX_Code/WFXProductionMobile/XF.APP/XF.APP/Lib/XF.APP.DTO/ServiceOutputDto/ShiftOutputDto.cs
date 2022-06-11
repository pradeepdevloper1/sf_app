using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class ShiftOutputDto : BaseResponseDto
    {
        public List<Shift> data { get; set; }
    }

    public class Shift
    {
        public int shiftID { get; set; }
        public int moduleID { get; set; }
        public string shiftName { get; set; }
        public TimeSpan shiftStartTime { get; set; }
        public TimeSpan shiftEndTime { get; set; }
        public int factoryID { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
