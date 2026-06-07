using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class JournalWindow : Window
    {
        private JournalRepository journalRepository = new JournalRepository();

        public JournalWindow()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Now.AddDays(-7);
            EndDatePicker.SelectedDate = DateTime.Now;
            LoadJournal();
        }

        private void LoadJournal()
        {
            var entries = journalRepository.GetEntries();
            JournalDataGrid.ItemsSource = entries;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate?.AddDays(1).AddSeconds(-1);

            var entries = journalRepository.GetEntries(startDate, endDate);
            JournalDataGrid.ItemsSource = entries;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadJournal();
        }
    }
}