using System;

namespace SpaSalon.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal ServiceCost { get; set; }
        public int ServiceDuration { get; set; } // длительность услуги в минутах
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } // new, confirmed, completed, cancelled
        public string CancellationReason { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal ActualCost { get; set; }

        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "new": return "Новая";
                    case "confirmed": return "Подтверждена";
                    case "completed": return "Выполнена";
                    case "cancelled": return "Отменена";
                    default: return Status;
                }
            }
        }

        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case "new": return "#FF9800";
                    case "confirmed": return "#4CAF50";
                    case "completed": return "#2196F3";
                    case "cancelled": return "#F44336";
                    default: return "#999999";
                }
            }
        }

        public string DateTimeString => DateTime.ToString("dd.MM.yyyy HH:mm");
        public string DateOnlyString => DateTime.ToString("dd.MM.yyyy");
        public string TimeOnlyString => DateTime.ToString("HH:mm");
        public string CostString => $"{ActualCost:N0} ₽";
        public string EndTimeString => DateTime.AddMinutes(ServiceDuration).ToString("HH:mm");
    }
}