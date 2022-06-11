﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class tbl_UserModules
    {
        public int UserModulesID { get; set; }
        public int UserID { get; set; }
        public int FactoryID { get; set; }
        public int ModuleID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModuleName { get; set; }
        public virtual tbl_Users tbl_Users { get; set; }

    }
}
