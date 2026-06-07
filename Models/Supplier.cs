using System;

namespace SpaSalon.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class Purchase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }

        public string TotalPriceString => $"{Quantity * Price:N0} ₽";
        public string PriceString => $"{Price:N2} ₽";
        public string DateString => PurchaseDate.ToString("dd.MM.yyyy");
    }
}