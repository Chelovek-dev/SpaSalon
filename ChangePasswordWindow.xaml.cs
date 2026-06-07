using SpaSalon.Models;
using SpaSalon.Repositories;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SpaSalon.Views
{
    public partial class ChangePasswordWindow : Window
    {
        private User currentUser;
        private UserRepository userRepository = new UserRepository();
        private JournalRepository journalRepository = new JournalRepository();

        public ChangePasswordWindow(User user)
        {
            InitializeComponent();
            currentUser = user;

            // Добавляем проверку сложности пароля
            NewPasswordBox.PasswordChanged += (s, e) => UpdatePasswordStrength();
        }

        private void UpdatePasswordStrength()
        {
            string password = NewPasswordBox.Password;
            string strength = GetPasswordStrength(password);
            PasswordStrengthText.Text = strength;

            if (strength.Contains("Слабый"))
                PasswordStrengthText.Foreground = System.Windows.Media.Brushes.Red;
            else if (strength.Contains("Средний"))
                PasswordStrengthText.Foreground = System.Windows.Media.Brushes.Orange;
            else
                PasswordStrengthText.Foreground = System.Windows.Media.Brushes.Green;
        }

        private string GetPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Введите пароль";

            int score = 0;

            // Длина
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // Цифры
            if (Regex.IsMatch(password, @"\d")) score++;

            // Заглавные буквы
            if (Regex.IsMatch(password, @"[A-ZА-Я]")) score++;

            // Специальные символы
            if (Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]")) score++;

            if (score <= 2) return "Слабый пароль";
            if (score <= 4) return "Средний пароль";
            return "Надежный пароль";
        }

        private async void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentPassword = CurrentPasswordBox.Password;
                string newPassword = NewPasswordBox.Password;
                string confirmPassword = ConfirmPasswordBox.Password;

                // Проверка текущего пароля
                var authenticated = await System.Threading.Tasks.Task.Run(() =>
                    userRepository.Authenticate(currentUser.Phone, currentPassword));

                if (authenticated == null)
                {
                    MessageBox.Show("Неверный текущий пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка нового пароля
                if (string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Введите новый пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Новый пароль должен содержать не менее 6 символов!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Смена пароля
                bool result = await System.Threading.Tasks.Task.Run(() =>
                    userRepository.ChangePassword(currentUser.Id, newPassword));

                if (result)
                {
                    journalRepository.AddEntry(currentUser.Id, currentUser.FullName,
                        "Смена пароля", $"Пользователь {currentUser.FullName} сменил пароль");

                    MessageBox.Show("Пароль успешно изменен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при смене пароля!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}