using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;

namespace SpaSalon.Views
{
    public partial class MaterialConsumptionWindow : Window
    {
        private Appointment appointment;
        private MaterialRepository materialRepository = new MaterialRepository();
        private List<Material> allMaterials;
        private List<ServiceMaterial> serviceMaterials;
        private List<MaterialConsumption> consumedMaterials = new List<MaterialConsumption>();

        public MaterialConsumptionWindow(Appointment appointment)
        {
            InitializeComponent();
            this.appointment = appointment;
            AppointmentInfoText.Text = $"{appointment.ClientName} - {appointment.ServiceName} ({appointment.DateTimeString})";
            LoadMaterials();
            LoadServiceMaterials();
        }

        private void LoadMaterials()
        {
            allMaterials = materialRepository.GetAllMaterials();
            MaterialComboBox.ItemsSource = allMaterials;
        }

        private void LoadServiceMaterials()
        {
            serviceMaterials = materialRepository.GetServiceMaterials(appointment.ServiceId);
            var serviceMaterialsWithNames = new List<dynamic>();

            foreach (var sm in serviceMaterials)
            {
                var material = allMaterials.FirstOrDefault(m => m.Id == sm.MaterialId);
                serviceMaterialsWithNames.Add(new { MaterialName = material?.Name ?? "Неизвестно", sm.QuantityNeeded });
            }

            ServiceMaterialsGrid.ItemsSource = serviceMaterialsWithNames;
        }

        private void MaterialComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MaterialComboBox.SelectedItem != null)
            {
                var material = MaterialComboBox.SelectedItem as Material;
                UnitText.Text = material.Unit;
                AvailableText.Text = $"Доступно: {material.Quantity}";

                // Проверяем, есть ли этот материал в нормативах услуги
                var serviceMaterial = serviceMaterials.FirstOrDefault(sm => sm.MaterialId == material.Id);
                if (serviceMaterial != null)
                {
                    QuantityBox.Text = serviceMaterial.QuantityNeeded.ToString();
                }
                else
                {
                    QuantityBox.Text = "1";
                }
            }
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MaterialComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Выберите материал!";
                    return;
                }

                if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
                {
                    ErrorTextBlock.Text = "Введите корректное количество!";
                    return;
                }

                var material = MaterialComboBox.SelectedItem as Material;

                if (quantity > material.Quantity)
                {
                    ErrorTextBlock.Text = $"Недостаточно материала! Доступно: {material.Quantity} {material.Unit}";
                    return;
                }

                // Записываем расход
                if (materialRepository.RecordConsumption(appointment.Id, material.Id, quantity))
                {
                    // Обновляем локальный список
                    consumedMaterials.Add(new MaterialConsumption
                    {
                        MaterialId = material.Id,
                        MaterialName = material.Name,
                        QuantityUsed = quantity,
                        ConsumptionDate = DateTime.Now
                    });

                    ConsumedMaterialsList.ItemsSource = null;
                    ConsumedMaterialsList.ItemsSource = consumedMaterials;

                    // Обновляем доступное количество
                    material.Quantity -= quantity;
                    AvailableText.Text = $"Доступно: {material.Quantity}";

                    ErrorTextBlock.Text = "Расход добавлен!";
                    ErrorTextBlock.Foreground = System.Windows.Media.Brushes.Green;

                    QuantityBox.Text = "1";
                }
                else
                {
                    ErrorTextBlock.Text = "Ошибка при списании материала!";
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
                ErrorTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}