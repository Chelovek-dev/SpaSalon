namespace SpaSalon.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int Duration { get; set; } // в минутах

        public string DurationString => $"{Duration / 60}ч {Duration % 60}мин";
        public string CostString => $"{Cost:N0} ₽";
    }
}