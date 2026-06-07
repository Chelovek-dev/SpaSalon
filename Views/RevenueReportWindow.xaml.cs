using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Text;

namespace SpaSalon.Views
{
    public partial class RevenueReportWindow : Window
    {
        private AppointmentRepository appointmentRepository = new AppointmentRepository();
        private List<MasterRevenue> revenueReport;

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
            var completedAppointments = appointments.Where(a => a.Status == "completed").ToList();

            // Группировка по мастерам
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

            // Расчёт доли и среднего чека
            foreach (var item in revenueByMaster)
            {
                item.SharePercent = totalRevenue > 0 ? $"{((double)item.Revenue / (double)totalRevenue) * 100:F1}%" : "0%";
                item.AverageCheck = item.AppointmentsCount > 0 ? $"{item.Revenue / item.AppointmentsCount:N0} ₽" : "0 ₽";
                item.RevenueDisplay = $"{item.Revenue:N0} ₽";
            }

            revenueReport = revenueByMaster;
            RevenueDataGrid.ItemsSource = revenueReport;

            // Общие итоги
            TotalRevenueText.Text = $"{totalRevenue:N0} ₽";
            TotalAppointmentsText.Text = totalAppointments.ToString();
            AverageCheckText.Text = totalAppointments > 0 ? $"{totalRevenue / totalAppointments:N0} ₽" : "0 ₽";
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (revenueReport == null || revenueReport.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "CSV файлы (*.csv)|*.csv|Excel файлы (*.xls)|*.xls";
                saveFileDialog.DefaultExt = ".csv";
                saveFileDialog.FileName = $"Отчёт_выручка_мастеров_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();

                    // Заголовки
                    sb.AppendLine("Мастер;Количество услуг;Выручка;Доля от общей;Средний чек");

                    // Данные
                    foreach (var r in revenueReport)
                    {
                        sb.AppendLine($"{r.MasterName};{r.AppointmentsCount};{r.Revenue};{r.SharePercent};{r.AverageCheck}");
                    }

                    // Итоги
                    sb.AppendLine();
                    sb.AppendLine($"Общая выручка;{TotalRevenueText.Text}");
                    sb.AppendLine($"Всего услуг;{TotalAppointmentsText.Text}");
                    sb.AppendLine($"Средний чек;{AverageCheckText.Text}");

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show($"Отчёт успешно экспортирован!\n{saveFileDialog.FileName}", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class MasterRevenue
    {
        public string MasterName { get; set; }
        public int AppointmentsCount { get; set; }
        public decimal Revenue { get; set; }
        public string RevenueDisplay { get; set; }
        public string SharePercent { get; set; }
        public string AverageCheck { get; set; }
    }
}