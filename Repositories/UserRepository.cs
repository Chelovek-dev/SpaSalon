using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class UserRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public User Authenticate(string phone, string password)
        {
            try
            {
                string query = @"SELECT `код мастера` as Id, `фио` as FullName, 
                        `телефон` as Phone, `пароль` as Password, 
                        `должность` as Position 
                        FROM `мастера` 
                        WHERE `телефон` = @phone AND `пароль` = @password";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@phone", phone),
                    new MySqlParameter("@password", password)
                };

                DataTable result = db.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    var user = new User
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        FullName = row["FullName"].ToString(),
                        Phone = row["Phone"].ToString(),
                        Password = row["Password"].ToString(),
                        Position = row["Position"].ToString()
                    };
                    user.Role = DetermineRole(user.Position);
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка аутентификации: {ex.Message}", ex);
            }
        }

        private string DetermineRole(string position)
        {
            if (string.IsNullOrEmpty(position)) return "Master";

            position = position.ToLower();
            // Только Администратор или Мастер
            if (position.Contains("администратор") || position.Contains("admin") || position.Contains("старший"))
                return "Admin";
            return "Master";
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            try
            {
                string query = "UPDATE `мастера` SET `пароль` = @password WHERE `код мастера` = @userId";
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@password", newPassword),
                    new MySqlParameter("@userId", userId)
                };
                return db.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка смены пароля: {ex.Message}", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                string query = @"SELECT `код мастера` as Id, `фио` as FullName, 
                                `телефон` as Phone, `должность` as Position 
                                FROM `мастера` WHERE `код мастера` = @userId";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@userId", userId)
                };

                DataTable result = db.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    var user = new User
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        FullName = row["FullName"].ToString(),
                        Phone = row["Phone"].ToString(),
                        Position = row["Position"].ToString()
                    };
                    user.Role = DetermineRole(user.Position);
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения пользователя: {ex.Message}", ex);
            }
        }

        public List<User> GetAllMasters()
        {
            var masters = new List<User>();
            string query = "SELECT `код мастера` as Id, `фио` as FullName, `должность` as Position FROM `мастера` ORDER BY `фио`";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                masters.Add(new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    FullName = row["FullName"].ToString(),
                    Position = row["Position"].ToString(),
                    Role = DetermineRole(row["Position"].ToString())
                });
            }
            return masters;
        }
    }
}