using System;
using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class DefectOutputDto
    {
        public int defectID { get; set; }
        public int departmentID { get; set; }
        public string defectCode { get; set; }
        public string defectName { get; set; }
        public string defectType { get; set; }
        public string defectLevel { get; set; }
    }

    public class OperationOutputDto : BaseResponseDto
    {
        public List<Operation> data { get; set; }
    }

    public class Operation
    {
        public int obid { get; set; }
        public string poNo { get; set; }
        public int sNo { get; set; }
        public string operationCode { get; set; }
        public string operationName { get; set; }
        public string section { get; set; }
        public double smv { get; set; }
        public int userID { get; set; }
        public DateTime entryDate { get; set; }
        public string obLocation { get; set; }
    }
}
