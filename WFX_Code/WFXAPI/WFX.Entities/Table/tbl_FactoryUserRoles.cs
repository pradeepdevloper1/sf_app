using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class tbl_FactoryUserRoles
    {
        public int FactoryUserRolesID { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public int UserRoleID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserRole { get; set; }

        public virtual tbl_Users tbl_Users { get; set; }
    }
}
