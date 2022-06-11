namespace XF.APP.DTO
{
    public class UserLoginOutputDto : BaseResponseDto
    {
        public UserLoginWebDto data { get; set; }
    }
    public class UserLoginWebDto
    {
        public int userID { get; set; }

        public int lineID { get; set; }
        public int factoryID { get; set; }

        public string userFirstName { get; set; }

        public string userLastName { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public int userRoleID { get; set; }

        public int members { get; set; }

        public string userType { get; set; }

        public string userEmail { get; set; }

        public string LineName { get; set; }
        public string Module { get; set; }

    }
}
