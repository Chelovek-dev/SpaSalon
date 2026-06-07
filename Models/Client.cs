using System;

namespace SpaSalon.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int Age
        {
            get
            {
                if (!BirthDate.HasValue) return 0;
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Value.Year;
                if (BirthDate.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public string BirthDateString => BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана";
        public string RegistrationDateString => RegistrationDate.ToString("dd.MM.yyyy");
    }
}