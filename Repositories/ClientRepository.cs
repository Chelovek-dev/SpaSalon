using MySql.Data.MySqlClient;
using SpaSalon.Database;
using SpaSalon.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpaSalon.Repositories
{
    public class ClientRepository
    {
        private DatabaseHelper db = new DatabaseHelper();

        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();
            string query = "SELECT `код клиента`, `фио`, `телефон`, `дата рождения` FROM `клиенты` ORDER BY `фио`";

            DataTable result = db.ExecuteQuery(query);

            foreach (DataRow row in result.Rows)
            {
                clients.Add(new Client
                {
                    Id = Convert.ToInt32(row["код клиента"]),
                    FullName = row["фио"].ToString(),
                    Phone = row["телефон"].ToString(),
                    BirthDate = row["дата рождения"] != DBNull.Value ? Convert.ToDateTime(row["дата рождения"]) : (DateTime?)null,
                    RegistrationDate = DateTime.Now
                });
            }
            return clients;
        }

        public List<Client> SearchClients(string searchText)
        {
            var clients = new List<Client>();
            string query = @"SELECT `код клиента`, `фио`, `телефон`, `дата рождения` 
                            FROM `клиенты` 
                            WHERE `фио` LIKE @search OR `телефон` LIKE @search 
                            ORDER BY `фио`";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@search", $"%{searchText}%")
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            foreach (DataRow row in result.Rows)
            {
                clients.Add(new Client
                {
                    Id = Convert.ToInt32(row["код клиента"]),
                    FullName = row["фио"].ToString(),
                    Phone = row["телефон"].ToString(),
                    BirthDate = row["дата рождения"] != DBNull.Value ? Convert.ToDateTime(row["дата рождения"]) : (DateTime?)null,
                    RegistrationDate = DateTime.Now
                });
            }
            return clients;
        }

        // ==================== ПУНКТ 5: Проверка существования телефона ====================
        /// <summary>
        /// Проверяет, существует ли клиент с таким номером телефона
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <param name="excludeId">ID клиента, которого нужно исключить из проверки (для редактирования)</param>
        /// <returns>true - если телефон уже существует, false - если свободен</returns>
        public bool IsPhoneExists(string phone, int excludeId = 0)
        {
            string query = "SELECT COUNT(*) FROM `клиенты` WHERE `телефон` = @phone AND `код клиента` != @excludeId";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@phone", phone),
                new MySqlParameter("@excludeId", excludeId)
            };
            DataTable result = db.ExecuteQuery(query, parameters);
            int count = Convert.ToInt32(result.Rows[0][0]);
            return count > 0;
        }

        // ==================== ДОБАВЛЕНИЕ КЛИЕНТА С ПРОВЕРКОЙ ====================
        public bool AddClient(Client client)
        {
            // Проверка на дублирование телефона
            if (IsPhoneExists(client.Phone, 0))
            {
                throw new Exception("Клиент с таким номером телефона уже существует!");
            }

            string query = @"INSERT INTO `клиенты` (`фио`, `телефон`, `дата рождения`) 
                            VALUES (@name, @phone, @birthDate)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", client.FullName),
                new MySqlParameter("@phone", client.Phone),
                new MySqlParameter("@birthDate", client.BirthDate ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ==================== ОБНОВЛЕНИЕ КЛИЕНТА С ПРОВЕРКОЙ ====================
        public bool UpdateClient(Client client)
        {
            // Проверка на дублирование телефона (исключая текущего клиента)
            if (IsPhoneExists(client.Phone, client.Id))
            {
                throw new Exception("Клиент с таким номером телефона уже существует!");
            }

            string query = @"UPDATE `клиенты` 
                            SET `фио` = @name, `телефон` = @phone, `дата рождения` = @birthDate 
                            WHERE `код клиента` = @id";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", client.Id),
                new MySqlParameter("@name", client.FullName),
                new MySqlParameter("@phone", client.Phone),
                new MySqlParameter("@birthDate", client.BirthDate ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ==================== УДАЛЕНИЕ КЛИЕНТА ====================
        public bool DeleteClient(int id)
        {
            string query = "DELETE FROM `клиенты` WHERE `код клиента` = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id)
            };
            return db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}