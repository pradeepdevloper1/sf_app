using System;

namespace XF.APP.DTO
{
    public class BaseModelDto
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedDate { get; set; }
        public long UpdatedDate { get; set; }
    }
}
