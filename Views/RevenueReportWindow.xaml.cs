using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class RevenueReportWindow : Window
    {
        private AppointmentRepository appointmentRepository = new AppointmentRepository();

        public RevenueReportWindow()
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
            var completedAppointments = appointments.Where(a => a.Status == "выполнена").ToList();

            if (completedAppointments.Count == 0)
            {
                MessageBox.Show("Нет выполненных записей за выбранный период!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                RevenueDataGrid.ItemsSource = null;
                TotalRevenueText.Text = "0 ₽";
                TotalAppointmentsText.Text = "0";
                AverageCheckText.Text = "0 ₽";
                return;
            }

            var revenueByMaster = completedAppointments
                .GroupBy(a => a.MasterName)
                .Select(g => new MasterRevenue
                {
                    MasterName = g.Key,
                    AppointmentsCount = g.Count(),
                    Revenue = g.Sum(a => a.ActualCost)
                })
                .OrderByDescending(r => r.Revenue)
                .ToList();

            decimal totalRevenue = revenueByMaster.Sum(r => r.Revenue);
            int totalAppointments = revenueByMaster.Sum(r => r.AppointmentsCount);

            foreach (var item in revenueByMaster)
            {
                item.AverageCheck = item.AppointmentsCount > 0 ? $"{item.Revenue / item.AppointmentsCount:N0} ₽" : "0 ₽";
                item.RevenueDisplay = $"{item.Revenue:N0} ₽";
            }

            RevenueDataGrid.ItemsSource = revenueByMaster;

            TotalRevenueText.Text = $"{totalRevenue:N0} ₽";
            TotalAppointmentsText.Text = totalAppointments.ToString();
            AverageCheckText.Text = totalAppointments > 0 ? $"{totalRevenue / totalAppointments:N0} ₽" : "0 ₽";
        }
    }

    public class MasterRevenue
    {
        public string MasterName { get; set; }
        public int AppointmentsCount { get; set; }
        public decimal Revenue { get; set; }
        public string RevenueDisplay { get; set; }
        public string AverageCheck { get; set; }
    }
}