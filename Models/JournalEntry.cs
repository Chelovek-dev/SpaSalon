using System;

namespace SpaSalon.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }

        public string TimestampString => Timestamp.ToString("dd.MM.yyyy HH:mm:ss");
    }
}