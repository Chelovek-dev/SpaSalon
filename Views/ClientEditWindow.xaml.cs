using SpaSalon.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class ClientEditWindow : Window
    {
        public Client Client { get; private set; }
        private bool isEdit;

        public ClientEditWindow(Client client = null)
        {
            InitializeComponent();

            if (client != null)
            {
                isEdit = true;
                TitleText.Text = "Редактирование клиента";
                Client = client;
                FullNameBox.Text = client.FullName;
                PhoneBox.Text = client.Phone;
                if (client.BirthDate.HasValue)
                {
                    BirthDateBox.Text = client.BirthDate.Value.ToString("dd.MM.yyyy");
                }
            }
            else
            {
                isEdit = false;
                Client = new Client();
            }
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string cleaned = Regex.Replace(phone, @"[\s\-\(\)\+]", "");

            if (!Regex.IsMatch(cleaned, @"^\d{11}$"))
                return false;

            return true;
        }

        private string FormatPhone(string phone)
        {
            string cleaned = Regex.Replace(phone, @"[\s\-\(\)\+]", "");
            if (cleaned.Length == 11)
            {
                return $"+7 ({cleaned.Substring(1, 3)}) {cleaned.Substring(4, 3)}-{cleaned.Substring(7, 2)}-{cleaned.Substring(9, 2)}";
            }
            return phone;
        }

        private void PhoneBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string allowedChars = "0123456789+() -";
            foreach (char c in e.Text)
            {
                if (!allowedChars.Contains(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО клиента!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Введите телефон клиента!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPhone(PhoneBox.Text))
            {
                MessageBox.Show("Неверный формат телефона!\n\n" +
                    "Телефон должен содержать 11 цифр.\n" +
                    "Пример: +7 (999) 123-45-67 или 89991234567",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client.FullName = FullNameBox.Text.Trim();
            Client.Phone = FormatPhone(PhoneBox.Text);

            if (!string.IsNullOrWhiteSpace(BirthDateBox.Text))
            {
                DateTime parsedDate;
                if (DateTime.TryParseExact(BirthDateBox.Text.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    if (parsedDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата рождения не может быть в будущем!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    Client.BirthDate = parsedDate;
                }
                else
                {
                    MessageBox.Show("Неверный формат даты! Используйте ДД.ММ.ГГГГ\nПример: 15.05.1990", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                Client.BirthDate = null;
            }

            Client.RegistrationDate = DateTime.Now;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}