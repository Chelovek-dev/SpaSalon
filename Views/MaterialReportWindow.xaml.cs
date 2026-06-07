using SpaSalon.Models;
using SpaSalon.Repositories;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class MaterialReportWindow : Window
    {
        private MaterialRepository materialRepository = new MaterialRepository();

        public MaterialReportWindow()
        {
            InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            var materials = materialRepository.GetAllMaterials();

            // Добавляем статус для каждого материала
            foreach (var m in materials)
            {
                if (m.Quantity <= 5)
                    m.StockStatus = "Критический!";
                else if (m.Quantity <= 10)
                    m.StockStatus = "Мало";
                else
                    m.StockStatus = "Норма";
            }

            MaterialsDataGrid.ItemsSource = materials;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMaterials();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}