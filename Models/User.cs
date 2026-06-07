using System;

namespace SpaSalon.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Position { get; set; }
        public string Role { get; set; } // Admin, Storekeeper, Master

        public string GetRoleDisplayName()
        {
            switch (Role)
            {
                case "Admin": return "Администратор";
                case "Storekeeper": return "Кладовщик";
                case "Master": return "Мастер";
                default: return Role;
            }
        }
    }
}