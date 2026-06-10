using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SpaSalon.Views
{
    public partial class MasterScheduleWindow : Window
    {
        private AppointmentRepository appointmentRepository = new AppointmentRepository();
        private UserRepository userRepository = new UserRepository();
        private List<User> masters;
        private DateTime currentWeekStart;
        private List<TimeSlotViewModel> timeSlots;

        public MasterScheduleWindow()
        {
            InitializeComponent();
            LoadMasters();
            CurrentDatePicker.SelectedDate = DateTime.Today;
            currentWeekStart = GetWeekStart(DateTime.Today);
            UpdateDayHeaders();
            LoadSchedule();
        }

        private void LoadMasters()
        {
            masters = userRepository.GetAllMasters();
            var allMasters = new List<User> { new User { Id = 0, FullName = "Все мастера" } };
            allMasters.AddRange(masters);
            MasterFilterComboBox.ItemsSource = allMasters;
            MasterFilterComboBox.SelectedIndex = 0;
        }

        private DateTime GetWeekStart(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private void UpdateDayHeaders()
        {
            Day0Header.Text = $"{currentWeekStart:dd.MM}\nПН";
            Day1Header.Text = $"{currentWeekStart.AddDays(1):dd.MM}\nВТ";
            Day2Header.Text = $"{currentWeekStart.AddDays(2):dd.MM}\nСР";
            Day3Header.Text = $"{currentWeekStart.AddDays(3):dd.MM}\nЧТ";
            Day4Header.Text = $"{currentWeekStart.AddDays(4):dd.MM}\nПТ";
            Day5Header.Text = $"{currentWeekStart.AddDays(5):dd.MM}\nСБ";
            Day6Header.Text = $"{currentWeekStart.AddDays(6):dd.MM}\nВС";
        }

        private void LoadSchedule()
        {
            DateTime endDate = currentWeekStart.AddDays(7);
            int selectedMasterId = MasterFilterComboBox.SelectedValue as int? ?? 0;

            List<Appointment> appointments;
            if (selectedMasterId == 0)
            {
                appointments = appointmentRepository.GetAppointmentsByDateRange(currentWeekStart, endDate);
            }
            else
            {
                var allAppointments = appointmentRepository.GetAppointmentsByDateRange(currentWeekStart, endDate);
                appointments = allAppointments.Where(a => a.MasterId == selectedMasterId).ToList();
            }

            // Создаём слоты времени с 9:00 до 21:00 с шагом 60 минут (для простоты)
            timeSlots = new List<TimeSlotViewModel>();

            for (int hour = 9; hour <= 20; hour++)
            {
                var timeSlot = new TimeSlotViewModel
                {
                    TimeSlot = $"{hour:D2}:00 - {hour + 1:D2}:00"
                };

                // Для каждого дня недели
                for (int day = 0; day < 7; day++)
                {
                    DateTime slotDateTime = currentWeekStart.AddDays(day).AddHours(hour);
                    var appointment = appointments.FirstOrDefault(a =>
                        a.DateTime.Date == slotDateTime.Date &&
                        a.DateTime.Hour >= hour &&
                        a.DateTime.Hour < hour + 1);

                    string color = "#E0E0E0";
                    string text = "Свободно";

                    if (appointment != null)
                    {
                        switch (appointment.Status)
                        {
                            case "новая":
                                color = "#FF9800";
                                text = $"{appointment.ClientName}\n{appointment.ServiceName}";
                                break;
                            case "подтверждена":
                                color = "#4CAF50";
                                text = $"{appointment.ClientName}\n{appointment.ServiceName}";
                                break;
                            case "выполнена":
                                color = "#2196F3";
                                text = $"{appointment.ClientName}\n{appointment.ServiceName}";
                                break;
                            case "отменена":
                                color = "#F44336";
                                text = "ОТМЕНЕНА";
                                break;
                        }
                    }

                    timeSlot.SetDayData(day, color, text);
                }

                timeSlots.Add(timeSlot);
            }

            ScheduleItemsControl.ItemsSource = timeSlots;
        }

        private void PrevWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(-7);
            CurrentDatePicker.SelectedDate = currentWeekStart.AddDays(3);
            UpdateDayHeaders();
            LoadSchedule();
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(7);
            CurrentDatePicker.SelectedDate = currentWeekStart.AddDays(3);
            UpdateDayHeaders();
            LoadSchedule();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = GetWeekStart(DateTime.Today);
            CurrentDatePicker.SelectedDate = DateTime.Today;
            UpdateDayHeaders();
            LoadSchedule();
        }

        private void CurrentDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CurrentDatePicker.SelectedDate.HasValue)
            {
                currentWeekStart = GetWeekStart(CurrentDatePicker.SelectedDate.Value);
                UpdateDayHeaders();
                LoadSchedule();
            }
        }

        private void MasterFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadSchedule();
        }
    }

    public class TimeSlotViewModel : DependencyObject
    {
        public string TimeSlot { get; set; }

        public string Day0Text { get; set; } = "Свободно";
        public string Day1Text { get; set; } = "Свободно";
        public string Day2Text { get; set; } = "Свободно";
        public string Day3Text { get; set; } = "Свободно";
        public string Day4Text { get; set; } = "Свободно";
        public string Day5Text { get; set; } = "Свободно";
        public string Day6Text { get; set; } = "Свободно";

        public Brush Day0Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day1Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day2Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day3Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day4Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day5Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        public Brush Day6Color { get; set; } = new SolidColorBrush(Color.FromRgb(224, 224, 224));

        public void SetDayData(int day, string colorHex, string text)
        {
            var color = (SolidColorBrush)new BrushConverter().ConvertFromString(colorHex);

            switch (day)
            {
                case 0:
                    Day0Text = text;
                    Day0Color = color;
                    break;
                case 1:
                    Day1Text = text;
                    Day1Color = color;
                    break;
                case 2:
                    Day2Text = text;
                    Day2Color = color;
                    break;
                case 3:
                    Day3Text = text;
                    Day3Color = color;
                    break;
                case 4:
                    Day4Text = text;
                    Day4Color = color;
                    break;
                case 5:
                    Day5Text = text;
                    Day5Color = color;
                    break;
                case 6:
                    Day6Text = text;
                    Day6Color = color;
                    break;
            }
        }
    }
}