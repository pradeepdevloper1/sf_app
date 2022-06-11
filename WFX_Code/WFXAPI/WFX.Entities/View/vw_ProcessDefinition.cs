using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class vw_ProcessDefinition
    {
        public int ProcessDefinitionID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessCode { get; set; }
        public int FactoryID { get; set; }
        public string FactoryName { get; set; }
    }
}
