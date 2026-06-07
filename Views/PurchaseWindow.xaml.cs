using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class PurchaseWindow : Window
    {
        private SupplierRepository supplierRepository = new SupplierRepository();
        private MaterialRepository materialRepository = new MaterialRepository();
        private List<Supplier> suppliers;
        private List<Material> materials;

        public PurchaseWindow()
        {
            InitializeComponent();
            LoadData();
            LoadPurchases();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadData()
        {
            suppliers = supplierRepository.GetAllSuppliers();
            materials = materialRepository.GetAllMaterials();

            SupplierComboBox.ItemsSource = suppliers;
            MaterialComboBox.ItemsSource = materials;
        }

        private void LoadPurchases()
        {
            var purchases = supplierRepository.GetAllPurchases();
            PurchasesDataGrid.ItemsSource = purchases;
        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SupplierComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите поставщика!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MaterialComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите материал!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!DatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату закупки!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var supplier = SupplierComboBox.SelectedItem as Supplier;
                var material = MaterialComboBox.SelectedItem as Material;

                var purchase = new Purchase
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.Name,
                    MaterialId = material.Id,
                    MaterialName = material.Name,
                    Quantity = quantity,
                    Price = price,
                    PurchaseDate = DatePicker.SelectedDate.Value
                };

                if (supplierRepository.AddPurchase(purchase))
                {
                    MessageBox.Show($"Закупка оформлена!\nМатериал: {material.Name}\nКоличество: {quantity}\nСумма: {quantity * price:N0} ₽",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    QuantityBox.Text = "1";
                    PriceBox.Text = "0";
                    LoadPurchases();
                    LoadData(); // Обновляем остатки материалов
                }
                else
                {
                    MessageBox.Show("Ошибка при оформлении закупки!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPurchases();
            LoadData();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}