using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class tbl_ProcessDefinition
    {
        public int ProcessDefinitionID { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string ProcessType { get; set; }
        public int FactoryID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastChangedOn { get; set; }

    }
}
