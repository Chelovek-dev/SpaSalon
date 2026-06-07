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

            // Создаём слоты времени с 9:00 до 21:00 с шагом 30 минут
            timeSlots = new List<TimeSlotViewModel>();

            for (int hour = 9; hour <= 20; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    if (hour == 20 && minute > 0) continue;

                    var timeSlot = new TimeSlotViewModel
                    {
                        TimeSlot = $"{hour:D2}:{minute:D2}",
                        Appointments = new List<AppointmentDisplay>()
                    };

                    // Для каждого дня недели
                    for (int i = 0; i < 7; i++)
                    {
                        DateTime slotDateTime = currentWeekStart.AddDays(i).AddHours(hour).AddMinutes(minute);
                        var appointment = appointments.FirstOrDefault(a =>
                            a.DateTime.Date == slotDateTime.Date &&
                            a.DateTime.Hour == hour &&
                            a.DateTime.Minute >= minute &&
                            a.DateTime.Minute < minute + 30);

                        if (appointment != null)
                        {
                            timeSlot.Appointments.Add(new AppointmentDisplay
                            {
                                MasterName = appointment.MasterName,
                                ClientName = appointment.ClientName,
                                ServiceName = appointment.ServiceName,
                                Status = appointment.Status,
                                Color = GetStatusColor(appointment.Status),
                                DisplayText = $"{appointment.MasterName}: {appointment.ClientName}",
                                Tooltip = $"{appointment.TimeOnlyString} | {appointment.ClientName} | {appointment.ServiceName} | {appointment.StatusDisplay}"
                            });
                        }
                        else if (i == 0) // Показываем свободное время только для понедельника для наглядности
                        {
                            timeSlot.Appointments.Add(new AppointmentDisplay
                            {
                                MasterName = "Свободно",
                                DisplayText = "Свободно",
                                Color = "#E0E0E0"
                            });
                        }
                    }

                    timeSlots.Add(timeSlot);
                }
            }

            ScheduleItemsControl.ItemsSource = timeSlots;
        }

        private string GetStatusColor(string status)
        {
            switch (status)
            {
                case "new": return "#FF9800";
                case "confirmed": return "#4CAF50";
                case "completed": return "#2196F3";
                case "cancelled": return "#F44336";
                default: return "#E0E0E0";
            }
        }

        private void PrevWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(-7);
            CurrentDatePicker.SelectedDate = currentWeekStart.AddDays(3);
            LoadSchedule();
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(7);
            CurrentDatePicker.SelectedDate = currentWeekStart.AddDays(3);
            LoadSchedule();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = GetWeekStart(DateTime.Today);
            CurrentDatePicker.SelectedDate = DateTime.Today;
            LoadSchedule();
        }

        private void CurrentDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CurrentDatePicker.SelectedDate.HasValue)
            {
                currentWeekStart = GetWeekStart(CurrentDatePicker.SelectedDate.Value);
                LoadSchedule();
            }
        }

        private void MasterFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadSchedule();
        }
    }

    public class TimeSlotViewModel
    {
        public string TimeSlot { get; set; }
        public List<AppointmentDisplay> Appointments { get; set; }
    }

    public class AppointmentDisplay
    {
        public string MasterName { get; set; }
        public string ClientName { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public string DisplayText { get; set; }
        public string Tooltip { get; set; }
    }
}