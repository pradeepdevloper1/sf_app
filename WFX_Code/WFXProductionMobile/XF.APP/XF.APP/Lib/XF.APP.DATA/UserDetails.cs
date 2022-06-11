using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XF.APP
{
    [Table("UserDetails")]
    public class UserDetails
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int LineID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public int UserRoleID { get; set; }

        public int Members { get; set; }

        public string UserType { get; set; }
    }
}
