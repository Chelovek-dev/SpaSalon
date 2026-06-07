using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SpaSalon.Views
{
    public partial class SupplierWindow : Window
    {
        private SupplierRepository supplierRepository = new SupplierRepository();
        private Supplier selectedSupplier = null;

        public SupplierWindow()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            var suppliers = supplierRepository.GetAllSuppliers();
            SuppliersDataGrid.ItemsSource = suppliers;
        }

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите наименование поставщика!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Введите телефон поставщика!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var supplier = new Supplier
            {
                Name = NameBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim()
            };

            if (supplierRepository.AddSupplier(supplier))
            {
                MessageBox.Show("Поставщик добавлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadSuppliers();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении поставщика!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSupplier == null) return;

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите наименование поставщика!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedSupplier.Name = NameBox.Text.Trim();
            selectedSupplier.Phone = PhoneBox.Text.Trim();

            if (supplierRepository.UpdateSupplier(selectedSupplier))
            {
                MessageBox.Show("Поставщик обновлён!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadSuppliers();
                AddSupplierButton.Visibility = Visibility.Visible;
                UpdateSupplierButton.Visibility = Visibility.Collapsed;
                CancelEditButton.Visibility = Visibility.Collapsed;
                selectedSupplier = null;
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении поставщика!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            AddSupplierButton.Visibility = Visibility.Visible;
            UpdateSupplierButton.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            selectedSupplier = null;
        }

        private void SuppliersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuppliersDataGrid.SelectedItem != null)
            {
                selectedSupplier = SuppliersDataGrid.SelectedItem as Supplier;
                if (selectedSupplier != null)
                {
                    NameBox.Text = selectedSupplier.Name;
                    PhoneBox.Text = selectedSupplier.Phone;
                    AddSupplierButton.Visibility = Visibility.Collapsed;
                    UpdateSupplierButton.Visibility = Visibility.Visible;
                    CancelEditButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var supplier = button?.Tag as Supplier;

            if (supplier == null) return;

            var result = MessageBox.Show($"Удалить поставщика \"{supplier.Name}\"?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (supplierRepository.DeleteSupplier(supplier.Id))
                {
                    MessageBox.Show("Поставщик удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadSuppliers();
                    if (selectedSupplier?.Id == supplier.Id)
                    {
                        ClearForm();
                        AddSupplierButton.Visibility = Visibility.Visible;
                        UpdateSupplierButton.Visibility = Visibility.Collapsed;
                        CancelEditButton.Visibility = Visibility.Collapsed;
                        selectedSupplier = null;
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении поставщика!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearForm()
        {
            NameBox.Text = "";
            PhoneBox.Text = "";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSuppliers();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}