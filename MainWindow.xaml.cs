using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class MainWindow : Window
    {
        private User currentUser;
        private ClientRepository clientRepository = new ClientRepository();
        private ServiceRepository serviceRepository = new ServiceRepository();
        private JournalRepository journalRepository = new JournalRepository();
        private AppointmentRepository appointmentRepository = new AppointmentRepository();

        public MainWindow(User user)
        {
            InitializeComponent();
            currentUser = user;

            // Отображаем информацию о пользователе
            string roleDisplay = currentUser.Role == "Admin" ? "Администратор" : "Мастер";
            UserInfoText.Text = $"{currentUser.FullName} ({roleDisplay})";

            // НАСТРОЙКА ПРАВ ДОСТУПА
            ConfigureAccessRights();

            LoadClients();
            LoadServices();
            LoadAppointments(null);
            ShowClientsPanel(); // Показываем клиентов по умолчанию

            // Записываем в журнал
            journalRepository.AddEntry(user.Id, user.FullName, "Вход в систему",
                $"Пользователь {user.FullName} открыл главное окно. Роль: {roleDisplay}");
        }

        /// <summary>
        /// Настройка видимости элементов в зависимости от роли пользователя
        /// </summary>
        private void ConfigureAccessRights()
        {
            if (currentUser.Role == "Admin")
            {
                // Администратор - полный доступ ко всему
                AddClientButton.Visibility = Visibility.Visible;
                EditClientButton.Visibility = Visibility.Visible;
                DeleteClientButton.Visibility = Visibility.Visible;
                RefreshClientsButton.Visibility = Visibility.Visible;
                AddServiceButton.Visibility = Visibility.Visible;
                RefreshServicesButton.Visibility = Visibility.Visible;
                AddAppointmentButton.Visibility = Visibility.Visible;
                EditAppointmentButton.Visibility = Visibility.Visible;
                CancelAppointmentButton.Visibility = Visibility.Visible;
                ConfirmAppointmentButton.Visibility = Visibility.Visible;
                CompleteAppointmentButton.Visibility = Visibility.Visible;
                RefreshAppointmentsButton.Visibility = Visibility.Visible;
                SuppliersButton.Visibility = Visibility.Visible;
                PurchasesButton.Visibility = Visibility.Visible;
                RevenueReportButton.Visibility = Visibility.Visible;
                ScheduleButton.Visibility = Visibility.Visible;
                MaterialReportButton.Visibility = Visibility.Visible;
            }
            else // Master
            {
                // Мастер - только просмотр и отметка выполнения
                AddClientButton.Visibility = Visibility.Collapsed;
                EditClientButton.Visibility = Visibility.Collapsed;
                DeleteClientButton.Visibility = Visibility.Collapsed;
                RefreshClientsButton.Visibility = Visibility.Visible;
                AddServiceButton.Visibility = Visibility.Collapsed;
                RefreshServicesButton.Visibility = Visibility.Visible;
                AddAppointmentButton.Visibility = Visibility.Collapsed;
                EditAppointmentButton.Visibility = Visibility.Collapsed;
                CancelAppointmentButton.Visibility = Visibility.Collapsed;
                ConfirmAppointmentButton.Visibility = Visibility.Collapsed;
                CompleteAppointmentButton.Visibility = Visibility.Visible;
                RefreshAppointmentsButton.Visibility = Visibility.Visible;
                SuppliersButton.Visibility = Visibility.Collapsed;
                PurchasesButton.Visibility = Visibility.Collapsed;
                RevenueReportButton.Visibility = Visibility.Collapsed;
                ScheduleButton.Visibility = Visibility.Visible;
                MaterialReportButton.Visibility = Visibility.Visible;
            }
        }

        // ==================== НАВИГАЦИЯ ====================

        private void ShowClientsPanel()
        {
            CurrentTabTitle.Text = "Клиенты";
            ClientsPanel.Visibility = Visibility.Visible;
            ServicesPanel.Visibility = Visibility.Collapsed;
            AppointmentsPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowServicesPanel()
        {
            CurrentTabTitle.Text = "Услуги";
            ClientsPanel.Visibility = Visibility.Collapsed;
            ServicesPanel.Visibility = Visibility.Visible;
            AppointmentsPanel.Visibility = Visibility.Collapsed;
            LoadServices();
        }

        private void ShowAppointmentsPanel()
        {
            CurrentTabTitle.Text = "Записи";
            ClientsPanel.Visibility = Visibility.Collapsed;
            ServicesPanel.Visibility = Visibility.Collapsed;
            AppointmentsPanel.Visibility = Visibility.Visible;
            LoadAppointments(FilterDatePicker.SelectedDate);
        }

        private void NavClientsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowClientsPanel();
        }

        private void NavServicesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowServicesPanel();
        }

        private void NavAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAppointmentsPanel();
        }

        // ==================== КЛИЕНТЫ ====================

        private void LoadClients()
        {
            var clients = clientRepository.GetAllClients();
            ClientsDataGrid.ItemsSource = clients;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadClients();
            }
            else
            {
                var clients = clientRepository.SearchClients(searchText);
                ClientsDataGrid.ItemsSource = clients;
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления клиентов!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ClientEditWindow();
            if (dialog.ShowDialog() == true)
            {
                if (clientRepository.AddClient(dialog.Client))
                {
                    MessageBox.Show("Клиент успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClients();
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Добавление клиента", $"Добавлен клиент: {dialog.Client.FullName}");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении клиента!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования клиентов!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = ClientsDataGrid.SelectedItem as Client;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ClientEditWindow(selected);
            if (dialog.ShowDialog() == true)
            {
                if (clientRepository.UpdateClient(dialog.Client))
                {
                    MessageBox.Show("Клиент успешно обновлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClients();
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Редактирование клиента", $"Отредактирован клиент: {dialog.Client.FullName}");
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении клиента!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления клиентов!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = ClientsDataGrid.SelectedItem as Client;
            if (selected == null)
            {
                MessageBox.Show("Выберите клиента для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить клиента \"{selected.FullName}\"?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (clientRepository.DeleteClient(selected.Id))
                {
                    MessageBox.Show("Клиент удален!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClients();
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Удаление клиента", $"Удален клиент: {selected.FullName}");
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении клиента!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshClientsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
            SearchTextBox.Text = "";
        }

        // ==================== УСЛУГИ ====================

        private void LoadServices()
        {
            var services = serviceRepository.GetAllServices();
            ServicesDataGrid.ItemsSource = services;
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления услуг!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ServiceEditWindow();
            if (dialog.ShowDialog() == true)
            {
                if (serviceRepository.AddService(dialog.Service))
                {
                    MessageBox.Show("Услуга успешно добавлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServices();
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Добавление услуги", $"Добавлена услуга: {dialog.Service.Name}");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении услуги!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshServicesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }

        // ==================== ЗАПИСИ ====================

        private void LoadAppointments(DateTime? filterDate = null)
        {
            if (filterDate.HasValue)
            {
                var appointments = appointmentRepository.GetAppointmentsByDate(filterDate.Value);
                AppointmentsDataGrid.ItemsSource = appointments;
            }
            else
            {
                if (currentUser.Role == "Master")
                {
                    var appointments = appointmentRepository.GetAppointmentsByMaster(currentUser.Id);
                    AppointmentsDataGrid.ItemsSource = appointments;
                }
                else
                {
                    var appointments = appointmentRepository.GetAllAppointments();
                    AppointmentsDataGrid.ItemsSource = appointments;
                }
            }
        }

        private void AddAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для создания записей!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AppointmentEditWindow();
            if (dialog.ShowDialog() == true)
            {
                if (appointmentRepository.AddAppointment(dialog.Appointment))
                {
                    MessageBox.Show("Запись успешно создана!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAppointments(FilterDatePicker.SelectedDate);
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Создание записи", $"Создана запись для {dialog.Appointment.ClientName} на {dialog.Appointment.DateTimeString}");
                }
                else
                {
                    MessageBox.Show("Ошибка при создании записи!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования записей!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = AppointmentsDataGrid.SelectedItem as SpaSalon.Models.Appointment;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AppointmentEditWindow(selected);
            if (dialog.ShowDialog() == true)
            {
                if (appointmentRepository.UpdateAppointment(dialog.Appointment))
                {
                    MessageBox.Show("Запись успешно обновлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAppointments(FilterDatePicker.SelectedDate);
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Редактирование записи", $"Отредактирована запись #{selected.Id}");
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении записи!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = AppointmentsDataGrid.SelectedItem as SpaSalon.Models.Appointment;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для отмены!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Status == "cancelled")
            {
                MessageBox.Show("Запись уже отменена!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Отменить запись клиента {selected.ClientName} на {selected.DateTimeString}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (appointmentRepository.CancelAppointment(selected.Id, "Отмена по просьбе клиента"))
                {
                    MessageBox.Show("Запись отменена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAppointments(FilterDatePicker.SelectedDate);
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Отмена записи", $"Отменена запись #{selected.Id} для {selected.ClientName}");
                }
                else
                {
                    MessageBox.Show("Ошибка при отмене записи!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConfirmAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для подтверждения записей!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = AppointmentsDataGrid.SelectedItem as SpaSalon.Models.Appointment;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для подтверждения!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Status != "new")
            {
                MessageBox.Show("Подтвердить можно только новую запись!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (appointmentRepository.ConfirmAppointment(selected.Id))
            {
                MessageBox.Show("Запись подтверждена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAppointments(FilterDatePicker.SelectedDate);
                journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                    "Подтверждение записи", $"Подтверждена запись #{selected.Id} для {selected.ClientName}");
            }
            else
            {
                MessageBox.Show("Ошибка при подтверждении записи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = AppointmentsDataGrid.SelectedItem as SpaSalon.Models.Appointment;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для отметки выполнения!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Status == "completed")
            {
                MessageBox.Show("Запись уже отмечена как выполненная!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Status == "cancelled")
            {
                MessageBox.Show("Отмененную запись нельзя отметить как выполненную!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Отметить запись клиента {selected.ClientName} как выполненную?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (appointmentRepository.CompleteAppointment(selected.Id, selected.ServiceCost))
                {
                    MessageBox.Show("Запись отмечена как выполненная!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAppointments(FilterDatePicker.SelectedDate);
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Выполнение записи", $"Выполнена запись #{selected.Id} для {selected.ClientName}");

                    var materialWindow = new MaterialConsumptionWindow(selected);
                    materialWindow.Owner = this;
                    materialWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка при отметке выполнения!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAppointments(FilterDatePicker.SelectedDate);
        }

        private void ShowAllAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDatePicker.SelectedDate = null;
            LoadAppointments(null);
        }

        // ==================== ОТЧЁТЫ И ДОПОЛНИТЕЛЬНЫЕ ОКНА ====================

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow();
            reportWindow.Owner = this;
            reportWindow.ShowDialog();
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            var journalWindow = new JournalWindow();
            journalWindow.Owner = this;
            journalWindow.ShowDialog();
        }

        private void SuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для управления поставщиками!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var supplierWindow = new SupplierWindow();
            supplierWindow.Owner = this;
            supplierWindow.ShowDialog();
        }

        private void PurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для управления закупками!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var purchaseWindow = new PurchaseWindow();
            purchaseWindow.Owner = this;
            purchaseWindow.ShowDialog();
        }

        private void MaterialReportButton_Click(object sender, RoutedEventArgs e)
        {
            var materialReportWindow = new MaterialReportWindow();
            materialReportWindow.Owner = this;
            materialReportWindow.ShowDialog();
        }

        private void RevenueReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для просмотра отчёта по выручке!", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var revenueReportWindow = new RevenueReportWindow();
            revenueReportWindow.Owner = this;
            revenueReportWindow.ShowDialog();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new MasterScheduleWindow();
            scheduleWindow.Owner = this;
            scheduleWindow.ShowDialog();
        }

        // ==================== ПРОЧЕЕ ====================

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow(currentUser);
            changePasswordWindow.Owner = this;
            changePasswordWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                    "Выход из системы", $"Пользователь {currentUser.FullName} вышел");

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}