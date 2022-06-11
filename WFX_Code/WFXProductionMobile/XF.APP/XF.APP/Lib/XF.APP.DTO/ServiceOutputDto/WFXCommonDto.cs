using System;
using System.Collections.Generic;
using System.Text;

namespace XF.APP.DTO
{
    public class WFXCommonDto
    {
    }
    public class UserRoleOutputDto : BaseResponseDto
    {
        public List<UserRole> data { get; set; }
    }
    public class LineOutputDto : BaseResponseDto
    {
        public List<Line> data { get; set; }
    }
    public class UserRole
    {
        public int UserRoleID { get; set; }
        public string RoleName { get; set; }
        public int UserRoleType { get; set; }
    }
    public class Line
    {
        public int LineID { get; set; }
        public string LineName { get; set; }
        public double Opacity { get; set; }

    }
    public class UserModule
    {
        public string Module { get; set; }
    }
}
