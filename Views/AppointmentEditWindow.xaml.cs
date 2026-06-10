using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SpaSalon.Views
{
    public partial class AppointmentEditWindow : Window
    {
        public SpaSalon.Models.Appointment Appointment { get; private set; }
        private bool isEdit;
        private ClientRepository clientRepository = new ClientRepository();
        private ServiceRepository serviceRepository = new ServiceRepository();
        private UserRepository userRepository = new UserRepository();
        private AppointmentRepository appointmentRepository = new AppointmentRepository();

        private List<Client> clients;
        private List<Service> services;
        private List<User> masters;
        private int selectedServiceDuration = 0;

        public AppointmentEditWindow(SpaSalon.Models.Appointment appointment = null)
        {
            InitializeComponent();
            LoadData();

            // Устанавливаем минимальную дату - сегодня
            DatePicker.DisplayDateStart = DateTime.Today;
            DatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));

            if (appointment != null)
            {
                isEdit = true;
                TitleText.Text = "Редактирование записи";
                Appointment = appointment;
                LoadAppointmentData();
                StatusLabel.Visibility = Visibility.Visible;
                StatusComboBox.Visibility = Visibility.Visible;
            }
            else
            {
                isEdit = false;
                TitleText.Text = "Новая запись";
                Appointment = new SpaSalon.Models.Appointment();
                DatePicker.SelectedDate = DateTime.Today;
                LoadAvailableTimes();
            }
        }

        private void LoadData()
        {
            clients = clientRepository.GetAllClients();
            services = serviceRepository.GetAllServices();
            masters = userRepository.GetAllMasters();

            ClientComboBox.ItemsSource = clients;
            ServiceComboBox.ItemsSource = services;
            MasterComboBox.ItemsSource = masters;
        }

        private void LoadAppointmentData()
        {
            ClientComboBox.SelectedValue = Appointment.ClientId;
            ServiceComboBox.SelectedValue = Appointment.ServiceId;
            MasterComboBox.SelectedValue = Appointment.MasterId;

            // Проверка - нельзя редактировать запись на прошедшую дату
            if (Appointment.DateTime.Date < DateTime.Today && !isEdit)
            {
                MessageBox.Show("Нельзя редактировать запись на прошедшую дату!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DialogResult = false;
                Close();
                return;
            }

            DatePicker.SelectedDate = Appointment.DateTime.Date;

            string timeSlot = Appointment.DateTime.ToString("HH:mm");
            LoadAvailableTimes();
            TimeComboBox.SelectedItem = timeSlot;

            switch (Appointment.Status)
            {
                case "новая":
                    StatusComboBox.SelectedIndex = 0;
                    break;
                case "подтверждена":
                    StatusComboBox.SelectedIndex = 1;
                    break;
                case "выполнена":
                    StatusComboBox.SelectedIndex = 2;
                    break;
                case "отменена":
                    StatusComboBox.SelectedIndex = 3;
                    break;
                default:
                    StatusComboBox.SelectedIndex = 0;
                    break;
            }
        }

        private void LoadAvailableTimes()
        {
            TimeComboBox.Items.Clear();

            // Рабочие часы с 9:00 до 21:00, шаг 30 минут
            for (int hour = 9; hour <= 20; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    if (hour == 20 && minute > 0) continue;
                    string time = $"{hour:D2}:{minute:D2}";
                    TimeComboBox.Items.Add(time);
                }
            }
        }

        private void ServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceComboBox.SelectedItem != null)
            {
                var selectedService = ServiceComboBox.SelectedItem as Service;
                if (selectedService != null)
                {
                    selectedServiceDuration = selectedService.Duration;
                    ServiceDurationText.Text = $"⏱️ Длительность: {selectedService.Duration} минут";
                }
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка при смене даты
            if (DatePicker.SelectedDate.HasValue && DatePicker.SelectedDate.Value.Date < DateTime.Today)
            {
                ErrorTextBlock.Text = "❌ Нельзя записаться на прошедшую дату!";
                DatePicker.SelectedDate = DateTime.Today;
            }
            else
            {
                ErrorTextBlock.Text = "";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения
                if (ClientComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Выберите клиента!";
                    return;
                }
                if (ServiceComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Выберите услугу!";
                    return;
                }
                if (MasterComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Выберите мастера!";
                    return;
                }
                if (!DatePicker.SelectedDate.HasValue)
                {
                    ErrorTextBlock.Text = "Выберите дату!";
                    return;
                }
                if (TimeComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Выберите время!";
                    return;
                }

                // ========== ГЛАВНАЯ ПРОВЕРКА: НЕЛЬЗЯ ЗАПИСЫВАТЬСЯ НА ПРОШЕДШУЮ ДАТУ ==========
                DateTime selectedDate = DatePicker.SelectedDate.Value;
                if (selectedDate.Date < DateTime.Today.Date)
                {
                    ErrorTextBlock.Text = "❌ Нельзя записаться на прошедшую дату! Выберите сегодня или позже.";
                    return;
                }

                var client = ClientComboBox.SelectedItem as Client;
                var service = ServiceComboBox.SelectedItem as Service;
                var master = MasterComboBox.SelectedItem as User;

                string selectedTime = TimeComboBox.SelectedItem.ToString();
                DateTime dateTime = selectedDate;
                dateTime = dateTime.AddHours(int.Parse(selectedTime.Substring(0, 2)));
                dateTime = dateTime.AddMinutes(int.Parse(selectedTime.Substring(3, 2)));

                // Проверка - если выбранная дата/время уже прошли
                if (dateTime < DateTime.Now)
                {
                    ErrorTextBlock.Text = "❌ Нельзя записаться на прошедшее время! Выберите будущее время.";
                    return;
                }

                // Проверка занятости мастера
                bool isAvailable = appointmentRepository.CheckMasterAvailability(
                    master.Id, dateTime, service.Duration, isEdit ? (int?)Appointment.Id : null);

                if (!isAvailable)
                {
                    ErrorTextBlock.Text = "Мастер уже занят в это время! Выберите другое время.";
                    return;
                }

                Appointment.ClientId = client.Id;
                Appointment.ClientName = client.FullName;
                Appointment.ClientPhone = client.Phone;
                Appointment.ServiceId = service.Id;
                Appointment.ServiceName = service.Name;
                Appointment.ServiceCost = service.Cost;
                Appointment.ServiceDuration = service.Duration;
                Appointment.MasterId = master.Id;
                Appointment.MasterName = master.FullName;
                Appointment.DateTime = dateTime;
                Appointment.ActualCost = service.Cost;

                if (isEdit)
                {
                    if (StatusComboBox.SelectedItem != null)
                    {
                        var selectedStatus = StatusComboBox.SelectedItem as ComboBoxItem;
                        if (selectedStatus != null && selectedStatus.Tag != null)
                        {
                            Appointment.Status = selectedStatus.Tag.ToString();
                        }
                        else
                        {
                            Appointment.Status = "новая";
                        }
                    }
                }
                else
                {
                    Appointment.Status = "новая";
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }
    }
}