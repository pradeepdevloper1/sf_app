using System;
using System.Collections.Generic;

namespace WFX.Entities
{
    public class tbl_Users
    {
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserRoleID { get; set; }
        public int Members { get; set; }
        public string UserType { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ICollection<tbl_FactoryUserRoles> tbl_FactoryUserRoles { get; set; }
        public virtual ICollection<tbl_UserModules> tbl_UserModules { get; set; }


    }
}
