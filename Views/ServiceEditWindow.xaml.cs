using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class ServiceEditWindow : Window
    {
        public Service Service { get; private set; }
        private bool isEdit;
        private MaterialRepository materialRepository = new MaterialRepository();
        private List<Material> allMaterials;
        private List<ServiceMaterial> serviceMaterials = new List<ServiceMaterial>();

        public ServiceEditWindow(Service service = null)
        {
            InitializeComponent();
            LoadMaterials();

            if (service != null)
            {
                isEdit = true;
                TitleText.Text = "Редактирование услуги";
                Service = service;
                NameBox.Text = service.Name;
                CostBox.Text = service.Cost.ToString();
                DurationBox.Text = service.Duration.ToString();
                LoadServiceMaterials();
            }
            else
            {
                isEdit = false;
                Service = new Service();
            }
        }

        private void LoadMaterials()
        {
            allMaterials = materialRepository.GetAllMaterials();
            MaterialComboBox.ItemsSource = allMaterials;
        }

        private void LoadServiceMaterials()
        {
            serviceMaterials = materialRepository.GetServiceMaterials(Service.Id);
            ServiceMaterialsGrid.ItemsSource = serviceMaterials.Select(sm => new
            {
                MaterialName = allMaterials.FirstOrDefault(m => m.Id == sm.MaterialId)?.Name ?? "Неизвестно",
                sm.QuantityNeeded
            }).ToList();
        }

        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MaterialQuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var material = MaterialComboBox.SelectedItem as Material;

            if (isEdit)
            {
                // Добавляем связь в БД
                materialRepository.AddServiceMaterial(Service.Id, material.Id, quantity);
            }

            // Обновляем локальный список
            serviceMaterials.Add(new ServiceMaterial
            {
                ServiceId = Service.Id,
                MaterialId = material.Id,
                QuantityNeeded = quantity
            });

            LoadServiceMaterials();
            MaterialQuantityBox.Text = "1";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите наименование услуги!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(CostBox.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DurationBox.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Введите корректную длительность!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Service.Name = NameBox.Text.Trim();
            Service.Cost = cost;
            Service.Duration = duration;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}