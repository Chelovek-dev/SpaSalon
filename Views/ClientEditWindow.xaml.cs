using SpaSalon.Models;
using System;
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
                BirthDatePicker.SelectedDate = client.BirthDate;
            }
            else
            {
                isEdit = false;
                Client = new Client();
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

            Client.FullName = FullNameBox.Text.Trim();
            Client.Phone = PhoneBox.Text.Trim();
            Client.BirthDate = BirthDatePicker.SelectedDate;
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