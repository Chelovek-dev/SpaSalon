using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Linq;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class ReportWindow : Window
    {
        private AppointmentRepository appointmentRepository = new AppointmentRepository();

        public ReportWindow()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Now.AddDays(-30);
            EndDatePicker.SelectedDate = DateTime.Now;
            GenerateReport();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период для отчёта!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value.AddDays(1).AddSeconds(-1);

            var appointments = appointmentRepository.GetAppointmentsByDateRange(startDate, endDate);
            ReportDataGrid.ItemsSource = appointments;

            // Подсчёт итогов
            var completed = appointments.Where(a => a.Status == "выполнена").ToList();
            var cancelled = appointments.Where(a => a.Status == "отменена").ToList();
            decimal totalRevenue = completed.Sum(a => a.ActualCost);

            TotalCountText.Text = appointments.Count.ToString();
            CompletedCountText.Text = completed.Count.ToString();
            CancelledCountText.Text = cancelled.Count.ToString();
            TotalRevenueText.Text = $"{totalRevenue:N0} ₽";
        }
    }
}