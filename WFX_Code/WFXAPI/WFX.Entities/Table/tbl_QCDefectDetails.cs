using System;

namespace WFX.Entities
{
    public class tbl_QCDefectDetails
    {
        public long QCDefectDetailsId { get; set; }
        public long QCMasterId { get; set; }
        public string DefectType { get; set; }
        public string DefectName { get; set; }
        public string OperationName { get; set; }

        public virtual tbl_QCMaster tbl_QCMaster { get; set; }
    }
}
