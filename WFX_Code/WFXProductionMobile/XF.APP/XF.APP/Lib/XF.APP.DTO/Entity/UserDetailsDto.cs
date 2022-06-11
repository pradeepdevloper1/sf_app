namespace XF.APP.DTO
{
    public class UserDetailsDto
    {
        public long Id { get; set; }
        public int UserID { get; set; }
        public int LineID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int UserRoleID { get; set; }
        public int Members { get; set; }
        public string UserType { get; set; }
    }
}
