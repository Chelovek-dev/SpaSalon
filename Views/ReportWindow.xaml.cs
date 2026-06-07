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
    public partial class ReportWindow : Window
    {
        private AppointmentRepository appointmentRepository = new AppointmentRepository();
        private List<Appointment> currentReport;

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

            currentReport = appointmentRepository.GetAppointmentsByDateRange(startDate, endDate);
            ReportDataGrid.ItemsSource = currentReport;

            // Подсчёт итогов
            var completed = currentReport.Where(a => a.Status == "completed").ToList();
            var cancelled = currentReport.Where(a => a.Status == "cancelled").ToList();
            decimal totalRevenue = completed.Sum(a => a.ActualCost);

            TotalCountText.Text = currentReport.Count.ToString();
            CompletedCountText.Text = completed.Count.ToString();
            CancelledCountText.Text = cancelled.Count.ToString();
            TotalRevenueText.Text = $"{totalRevenue:N0} ₽";
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentReport == null || currentReport.Count == 0)
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
                saveFileDialog.FileName = $"Отчёт_записей_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();

                    // Заголовки
                    sb.AppendLine("ID;Дата;Время;Клиент;Телефон;Услуга;Мастер;Статус;Стоимость");

                    // Данные
                    foreach (var a in currentReport)
                    {
                        sb.AppendLine($"{a.Id};{a.DateOnlyString};{a.TimeOnlyString};{a.ClientName};{a.ClientPhone};{a.ServiceName};{a.MasterName};{a.StatusDisplay};{a.ActualCost}");
                    }

                    // Итоги
                    sb.AppendLine();
                    sb.AppendLine($"Всего записей;{currentReport.Count}");
                    sb.AppendLine($"Выполнено;{currentReport.Count(a => a.Status == "completed")}");
                    sb.AppendLine($"Отменено;{currentReport.Count(a => a.Status == "cancelled")}");
                    sb.AppendLine($"Общая выручка;{currentReport.Where(a => a.Status == "completed").Sum(a => a.ActualCost):N0} ₽");

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
}