using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class LoginWindow : Window
    {
        private UserRepository userRepository = new UserRepository();

        public LoginWindow()
        {
            InitializeComponent();
            LoginTextBox.Text = "+7 (999) 111-22-33";
            PasswordBox.Password = "admin";  // Исправленный пароль из БД
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string phone = LoginTextBox.Text.Trim();
                string password = PasswordBox.Password;

                LoginButton.IsEnabled = false;
                LoginButton.Content = "Вход...";

                var user = userRepository.Authenticate(phone, password);

                if (user != null)
                {
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorTextBlock.Text = "Неверный телефон или пароль!";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
                ErrorTextBlock.Visibility = Visibility.Visible;
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Войти";
            }
        }
    }
}