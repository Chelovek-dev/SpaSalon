using System;

namespace SpaSalon.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public int? ExpirationDays { get; set; }
        public int CriticalThreshold { get; set; } = 5;
        public decimal Price { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        public bool IsCritical => Quantity <= CriticalThreshold;

        public string StatusColor => IsCritical ? "Red" : "Green";

        public string QuantityDisplay => $"{Quantity} {Unit}";

        public string PriceDisplay => $"{Price:N2} ₽";

        private string _stockStatus = "Норма";
        public string StockStatus
        {
            get
            {
                if (Quantity <= 5) return "Критический!";
                if (Quantity <= 10) return "Мало";
                return "Норма";
            }
            set { _stockStatus = value; }
        }
    }

    public class MaterialConsumption
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int QuantityUsed { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string Unit { get; set; }  // <--- ДОБАВЛЕНО ЭТО СВОЙСТВО

        public string QuantityDisplay => $"{QuantityUsed} {Unit ?? "шт"}";
        public string DateString => ConsumptionDate.ToString("dd.MM.yyyy HH:mm");
    }

    public class ServiceMaterial
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int QuantityNeeded { get; set; }

        public string QuantityDisplay => $"{QuantityNeeded} шт.";
    }
}