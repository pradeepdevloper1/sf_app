using System;

namespace WFX.Data
{
    public class DbInitializer
    {
        private static DBContext context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = (DBContext)serviceProvider.GetService(typeof(DBContext));
            InitializeSchedules();
        }

        private static void InitializeSchedules()
        {
            //if(!context.Users.Any())
            //{
            //    User user_01 = new User { Name = "Chris Sakellarios", Profession = "Developer", Avatar = "avatar_02.png" };

            //    context.Users.Add(user_01);
            //    context.SaveChanges();
            //}

        }
    }
}
