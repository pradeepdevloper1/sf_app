using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XF.APP.DATA
{
    [Table("LookupKey")]
    public class LookupKey
    {
        [Key]
        public long LookupKeyID { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string FilterType { get; set; }

        public string FilterValue { get; set; }

        [Required]
        public bool IsSurveyKey { get; set; }

        [Required]
        public string Field1DisplayName { get; set; }

        [Required]
        public string Field2DisplayName { get; set; }

        [Required]
        public string Field3DisplayName { get; set; }

        [Required]
        public string Field4DisplayName { get; set; }

        [Required]
        public string Field5DisplayName { get; set; }

        [Required]
        public string Field6DisplayName { get; set; }

        [Required]
        public string Field7DisplayName { get; set; }

        [Required]
        public string Field8DisplayName { get; set; }

        [Required]
        public string Field9DisplayName { get; set; }

        [Required]
        public string Field10DisplayName { get; set; }

        [Required]
        public string Field11DisplayName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDeleted { get; set; } 
    }
}
