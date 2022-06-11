using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class ProcessDataViewModel
    {
        public IEnumerable<string> ProductionOrderNo { get; set; }
        public IEnumerable<string> WorkOrderNo { get; set; }
        public IEnumerable<string> Parts { get; set; }
        public Dictionary<string, string> Colors { get; set; }
        public IEnumerable<string> IssueToWorkOrderNo { get; set; }
        public IEnumerable<string> IssueToLineNo { get; set; }
        public PurchaseOrder1 SelectedPo { get; set; }
        public IEnumerable<string> ColorsCode { get; set; }


        public string SelectedProductionOrderNo { get; set; }
        public Dictionary<string, string> SelectedColors { get; set; }
        public string SelectedWorkOrderNo { get; set; }
        public string SelectedParts { get; set; }
        public string SelectedIssueToWorkOrderNo { get; set; }
        public string SelectedIssueToLineNo { get; set; }
        public string SelectedColorsCode { get; set; }
        public string SelectedSizes { get; set; }

    }

    public class PONoDto {
        public IEnumerable<string> PONo { get; set; }
        public IEnumerable<string> Line { get; set; }
    }
   
}
